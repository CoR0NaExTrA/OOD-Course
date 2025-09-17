using System.Globalization;
using Shapes.Interface.gfx;
using Shapes.Interface.shapes;

namespace Shapes.Class.shapes;
public class CircleStrategy : IShapeStrategy
{
    private double _x, _y, _r;

    public CircleStrategy( double x, double y, double r )
    {
        if ( r < 0 )
            throw new ArgumentException( "Radius must be non-negative" );
        _x = x;
        _y = y;
        _r = r;
    }

    public string Type => "circle";

    public void Draw( ICanvas canvas, string color )
    {
        canvas.SetColor( color );
        canvas.DrawEllipse( _x, _y, _r, _r );
    }

    public void Move( double dx, double dy )
    {
        _x += dx;
        _y += dy;
    }

    public void Change( IReadOnlyList<string> parameters )
    {
        if ( parameters.Count != 3 )
            throw new ArgumentException( "Circle requires 3 parameters: x y r" );

        _x = double.Parse( parameters[ 0 ], CultureInfo.InvariantCulture );
        _y = double.Parse( parameters[ 1 ], CultureInfo.InvariantCulture );
        _r = double.Parse( parameters[ 2 ], CultureInfo.InvariantCulture );

        if ( _r < 0 )
            throw new ArgumentException( "Radius must be non-negative" );
    }

    public string ParametersToString()
    {
        return $"{_x.ToString( CultureInfo.InvariantCulture )} {_y.ToString( CultureInfo.InvariantCulture )} {_r.ToString( CultureInfo.InvariantCulture )}";
    }
}
