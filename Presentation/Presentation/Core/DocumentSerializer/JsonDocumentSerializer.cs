using Presentation.Shapes;
using SkiaSharp;
using System.Text.Json;

namespace Presentation.Core.DocumentSerializer;
public class JsonDocumentSerializer : IDocumentSerializer
{
    public void Save( Document doc, string path )
    {
        var list = new List<SerializableShapeEx>();

        var docDir = Path.GetDirectoryName( path ) ?? Directory.GetCurrentDirectory();
        var docName = Path.GetFileNameWithoutExtension( path );
        var imagesDirName = docName + "_images";
        var imagesDir = Path.Combine( docDir, imagesDirName );
        Directory.CreateDirectory( imagesDir );

        foreach ( var s in doc.Shapes )
        {
            if ( s is ImageShape img )
            {
                var tempPath = doc.ImageRepo.GetImageTempPath( img.ImageId );
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
                        var bytes = doc.ImageRepo.GetImageBytes( img.ImageId );
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

    public Document Load( string path )
    {
        var doc = new Document();
        doc.Shapes.Clear();

        var json = File.ReadAllText( path );
        var list = JsonSerializer.Deserialize<List<SerializableShapeEx>>( json );

        var docDir = Path.GetDirectoryName( path ) ?? Directory.GetCurrentDirectory();

        foreach ( var s in list )
        {
            //поправить проверку shapetype
            var rect = SKRect.Create( s.Left, s.Top, s.Width, s.Height );
            if ( s.Kind == "image" && !string.IsNullOrEmpty( s.RelativeImagePath ) )
            {
                var abs = Path.Combine( docDir, s.RelativeImagePath );
                abs = Path.GetFullPath( abs );
                if ( File.Exists( abs ) )
                {
                    var id = doc.ImageRepo.AddImageFromFile( abs );
                    var shape = new ImageShape( rect, doc.ImageRepo, id, Path.GetFileName( abs ) );
                    doc.Shapes.Add( shape );
                }
                else
                {
                    var shape = new ImageShape( rect, null, null );
                    doc.Shapes.Add( shape );
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
                doc.Shapes.Add( shape );
            }
        }
        return doc;
    }
}

