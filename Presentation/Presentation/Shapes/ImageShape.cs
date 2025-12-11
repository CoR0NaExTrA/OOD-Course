using SkiaSharp;
using System;
using System.IO;

namespace Presentation.Shapes;

public class ImageShape : Shape
{
    public string ImageId { get; set; }
    public string SourceFilenameHint { get; set; }

    public ImageShape( SKRect bounds, string imageId, string sourceFilenameHint = null )
        : base( ShapeType.Rectangle, bounds )
    {
        ImageId = imageId;
        SourceFilenameHint = sourceFilenameHint;
        Type = ShapeType.Rectangle;
    }

    public override void Draw( SKCanvas c )
    {
        var bytes = Document.ImageRepo?.GetImageBytes( ImageId );
        if ( bytes != null )
        {
            try
            {
                using ( var ms = new MemoryStream( bytes ) )
                using ( var codec = SKCodec.Create( ms ) )
                using ( var bitmap = SKBitmap.Decode( codec ) )
                {
                    if ( bitmap != null )
                    {
                        var src = new SKRect( 0, 0, bitmap.Width, bitmap.Height );
                        c.DrawBitmap( bitmap, src, Bounds );
                    }
                }
            }
            catch
            {
                using ( var paint = new SKPaint { Style = SKPaintStyle.Fill } )
                {
                    paint.Color = SKColors.LightGray;
                    c.DrawRect( Bounds, paint );
                }
            }
        }
        else
        {
            using ( var paint = new SKPaint { Style = SKPaintStyle.Fill } )
            {
                paint.Color = SKColors.LightGray;
                c.DrawRect( Bounds, paint );
            }
        }

        if ( IsSelected )
            DrawSelection( c );
    }

    public override bool HitTest( SKPoint p )
    {
        return Bounds.Contains( p.X, p.Y );
    }
}
