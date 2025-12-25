using SkiaSharp;

namespace Presentation.Shapes;

public class EllipseShape : Shape
{
    public EllipseShape( SKRect bounds ) : base( ShapeType.Ellipse, bounds ) { }

    public override void Draw( SKCanvas c )
    {
        using ( var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true } )
        {
            paint.Color = SKColors.LightSkyBlue;
            c.DrawOval( Bounds, paint );
        }
        using ( var paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1 } )
        {
            paint.Color = SKColors.DarkBlue;
            c.DrawOval( Bounds, paint );
        }
    }

    public override bool HitTest( SKPoint p )
    {
        var r = Bounds;
        float rx = r.Width / 2f;
        float ry = r.Height / 2f;
        float cx = r.MidX;
        float cy = r.MidY;
        if ( rx <= 0 || ry <= 0 )
            return false;
        float nx = ( p.X - cx ) / rx;
        float ny = ( p.Y - cy ) / ry;
        return nx * nx + ny * ny <= 1.0f;
    }
}
