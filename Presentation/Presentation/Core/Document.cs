using SkiaSharp;
using System.Text.Json;
using Presentation.Core;
using Presentation.Shapes;

public class Document
{
    public List<Shape> Shapes { get; } = new();

    public static ImageRepository ImageRepo { get; } = new ImageRepository();

    public void SaveToFile( string path )
    {
        var list = new List<SerializableShapeEx>();

        var docDir = Path.GetDirectoryName( path ) ?? Directory.GetCurrentDirectory();
        var docName = Path.GetFileNameWithoutExtension( path );
        var imagesDirName = docName + "_images";
        var imagesDir = Path.Combine( docDir, imagesDirName );
        Directory.CreateDirectory( imagesDir );

        foreach ( var s in Shapes )
        {
            if ( s is ImageShape img )
            {
                var tempPath = ImageRepo.GetImageTempPath( img.ImageId );
                var ext = ".img";
                if ( !string.IsNullOrEmpty( img.SourceFilenameHint ) )
                    ext = Path.GetExtension( img.SourceFilenameHint ) ?? ext;
                var fname = $"{Guid.NewGuid().ToString( "N" )}{ext}";
                var dest = Path.Combine( imagesDir, fname );
                try
                {
                    if ( !string.IsNullOrEmpty( tempPath ) && File.Exists( tempPath ) )
                    {
                        File.Copy( tempPath, dest, true );
                    }
                    else
                    {
                        var bytes = ImageRepo.GetImageBytes( img.ImageId );
                        if ( bytes != null )
                            File.WriteAllBytes( dest, bytes );
                    }
                }
                catch
                {
                }

                var rel = Path.Combine( imagesDirName, fname ).Replace( '\\', '/' );

                list.Add( new SerializableShapeEx
                {
                    Kind = "image",
                    Left = img.Bounds.Left,
                    Top = img.Bounds.Top,
                    Width = img.Bounds.Width,
                    Height = img.Bounds.Height,
                    RelativeImagePath = rel
                } );
            }
            else
            {
                list.Add( new SerializableShapeEx
                {
                    Kind = "shape",
                    ShapeType = s.Type,
                    Left = s.Bounds.Left,
                    Top = s.Bounds.Top,
                    Width = s.Bounds.Width,
                    Height = s.Bounds.Height
                } );
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize( list, options );
        File.WriteAllText( path, json );
    }
    public void LoadFromFile( string path )
    {
        Shapes.Clear();

        var json = File.ReadAllText( path );
        var list = JsonSerializer.Deserialize<List<SerializableShapeEx>>( json );

        var docDir = Path.GetDirectoryName( path ) ?? Directory.GetCurrentDirectory();

        foreach ( var s in list )
        {
            var rect = SKRect.Create( s.Left, s.Top, s.Width, s.Height );
            if ( s.Kind == "image" && !string.IsNullOrEmpty( s.RelativeImagePath ) )
            {
                var abs = Path.Combine( docDir, s.RelativeImagePath );
                abs = Path.GetFullPath( abs );
                if ( File.Exists( abs ) )
                {
                    var id = ImageRepo.AddImageFromFile( abs );
                    var shape = new ImageShape( rect, id, Path.GetFileName( abs ) );
                    Shapes.Add( shape );
                }
                else
                {
                    var shape = new ImageShape( rect, null, null );
                    Shapes.Add( shape );
                }
            }
            else
            {
                Shape shape = s.ShapeType switch
                {
                    ShapeType.Rectangle => new RectShape( rect ),
                    ShapeType.Ellipse => new EllipseShape( rect ),
                    ShapeType.Triangle => new TriangleShape( rect ),
                    _ => new RectShape( rect )
                };
                Shapes.Add( shape );
            }
        }
    }
}
