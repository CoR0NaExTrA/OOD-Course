using System.Drawing;
using Factory.Canvas;

namespace Factory.ShapeFactory;
public class Ellipse : Shape
{
    public Point Center { get; }
    public double Rx { get; }
    public double Ry { get; }


    public Ellipse( Point center, double rx, double ry, Color color ) : base( color )
    {
        Center = center;
        Rx = rx;
        Ry = ry;
    }


    public override void Draw( ICanvas canvas )
    {
        canvas.SetColor( _color );
        double left = Center.X - Rx;
        double top = Center.Y - Ry;
        double w = Rx * 2;
        double h = Ry * 2;
        canvas.DrawEllipse( left, top, w, h );
    }
}
