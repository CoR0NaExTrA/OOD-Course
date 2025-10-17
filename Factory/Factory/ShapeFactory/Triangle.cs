using System.Drawing;
using Factory.Canvas;

namespace Factory.ShapeFactory;
public class Triangle : Shape
{
    public Point A { get; }
    public Point B { get; }
    public Point C { get; }


    public Triangle( Point a, Point b, Point c, Color color ) : base( color )
    {
        A = a;
        B = b;
        C = c;
    }


    public override void Draw( ICanvas canvas )
    {
        canvas.SetColor( _color );
        canvas.DrawLine( A, B );
        canvas.DrawLine( B, C );
        canvas.DrawLine( C, A );
    }
}