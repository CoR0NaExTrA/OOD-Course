using SkiaSharp;

namespace Presentation.Shapes;

public class RectShape : Shape
{
    public RectShape( SKRect bounds ) : base( ShapeType.Rectangle, bounds ) { }

    public override void Draw( SKCanvas c )
    {
        using ( var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true } )
        {
            paint.Color = SKColors.LightGreen;
            c.DrawRect( Bounds, paint );
        }
        using ( var paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1 } )
        {
            paint.Color = SKColors.DarkGreen;
            c.DrawRect( Bounds, paint );
        }
    }

    public override bool HitTest( SKPoint p )
    {
        return Bounds.Contains( p.X, p.Y );
    }
}
