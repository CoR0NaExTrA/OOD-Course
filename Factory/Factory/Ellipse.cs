using System.Drawing;

namespace Factory;
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
        // SVG ellipse expects bounding box or cx,cy,rx,ry
        double left = Center.X - Rx;
        double top = Center.Y - Ry;
        double w = Rx * 2;
        double h = Ry * 2;
        canvas.DrawEllipse( left, top, w, h );
    }
}
