using SkiaSharp;
using System.IO;

namespace Presentation.Shapes;

public class TriangleShape : Shape
{
    public TriangleShape( SKRect bounds ) : base( ShapeType.Triangle, bounds ) { }

    public SKPoint[] GetPoints()
    {
        var r = Bounds;
        return new SKPoint[]
        {
            new SKPoint(r.MidX, r.Top),
            new SKPoint(r.Right, r.Bottom),
            new SKPoint(r.Left, r.Bottom)
        };
    }

    public override void Draw( SKCanvas c )
    {
        var pts = GetPoints();
        using ( var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true } )
        {
            paint.Color = SKColors.LightCoral;
            using ( var path = new SKPath() )
            {
                path.MoveTo( pts[ 0 ] );
                path.LineTo( pts[ 1 ] );
                path.LineTo( pts[ 2 ] );
                path.Close();
                c.DrawPath( path, paint );
            }
        }
        using ( var paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1 } )
        {
            paint.Color = SKColors.DarkRed;
            using ( var path = new SKPath() )
            {
                path.MoveTo( pts[ 0 ] );
                path.LineTo( pts[ 1 ] );
                path.LineTo( pts[ 2 ] );
                path.Close();
                c.DrawPath( path, paint );
            }
        }

        if ( IsSelected )
            DrawSelection( c );
    }

    public override bool HitTest( SKPoint p )
    {
        var pts = GetPoints();
        return PointInTriangle( p, pts[ 0 ], pts[ 1 ], pts[ 2 ] );
    }

    private bool PointInTriangle( SKPoint p, SKPoint a, SKPoint b, SKPoint c )
    {
        float v0x = c.X - a.X, v0y = c.Y - a.Y;
        float v1x = b.X - a.X, v1y = b.Y - a.Y;
        float v2x = p.X - a.X, v2y = p.Y - a.Y;

        float dot00 = v0x * v0x + v0y * v0y;
        float dot01 = v0x * v1x + v0y * v1y;
        float dot02 = v0x * v2x + v0y * v2y;
        float dot11 = v1x * v1x + v1y * v1y;
        float dot12 = v1x * v2x + v1y * v2y;

        float denom = dot00 * dot11 - dot01 * dot01;
        if ( Math.Abs( denom ) < 1e-6 )
            return false;
        float u = ( dot11 * dot02 - dot01 * dot12 ) / denom;
        float v = ( dot00 * dot12 - dot01 * dot02 ) / denom;
        return  u >= 0  &&  v >= 0  &&  u + v < 1 ;
    }
}
