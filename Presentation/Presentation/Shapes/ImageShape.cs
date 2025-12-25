using Presentation.Core;
using SkiaSharp;
using System;
using System.IO;

namespace Presentation.Shapes;

public class ImageShape : Shape
{
    public string ImageId { get; set; }
    public string SourceFilenameHint { get; set; }

    private readonly ImageRepository repo;

    public ImageShape( SKRect bounds, ImageRepository repo, string imageId, string sourceFilenameHint = null )
        : base( ShapeType.Rectangle, bounds )
    {
        this.repo = repo;
        ImageId = imageId;
        SourceFilenameHint = sourceFilenameHint;
        Type = ShapeType.Rectangle;
    }

    public override void Draw( SKCanvas c )
    {
        var bytes = repo?.GetImageBytes( ImageId );
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
    }

    public override bool HitTest( SKPoint p )
    {
        return Bounds.Contains( p.X, p.Y );
    }
}
