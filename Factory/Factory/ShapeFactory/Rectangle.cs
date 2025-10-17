using System.Drawing;
using Factory.Canvas;

namespace Factory.ShapeFactory;
public class Rectangle : Shape
{
    public Point LeftTop { get; }
    public Point RightBottom { get; }


    public Rectangle( Point lt, Point rb, Color color ) : base( color )
    {
        LeftTop = lt;
        RightBottom = rb;
    }


    public override void Draw( ICanvas canvas )
    {
        canvas.SetColor( _color );
        // draw four edges
        var lt = LeftTop;
        var rb = RightBottom;
        var rt = new Point( rb.X, lt.Y );
        var lb = new Point( lt.X, rb.Y );
        canvas.DrawLine( lt, rt );
        canvas.DrawLine( rt, rb );
        canvas.DrawLine( rb, lb );
        canvas.DrawLine( lb, lt );
    }
}