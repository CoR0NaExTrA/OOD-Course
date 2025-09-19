using System.Globalization;
using Shapes.Interface.gfx;
using Shapes.Interface.shapes;

namespace Shapes.Class.shapes;

public class TriangleStrategy : IShapeStrategy
{
    private double _x1, _y1, _x2, _y2, _x3, _y3;

    public TriangleStrategy( double x1, double y1, double x2, double y2, double x3, double y3 )
    {
        _x1 = x1;
        _y1 = y1;
        _x2 = x2;
        _y2 = y2;
        _x3 = x3;
        _y3 = y3;
    }

    public string GetTypeName() => "triangle";

    public void Draw( ICanvas canvas, string color )
    {
        canvas.SetColor( color );
        canvas.MoveTo( _x1, _y1 );
        canvas.LineTo( _x2, _y2 );
        canvas.LineTo( _x3, _y3 );
        canvas.LineTo( _x1, _y1 );
    }

    public void Move( double dx, double dy )
    {
        _x1 += dx;
        _y1 += dy;
        _x2 += dx;
        _y2 += dy;
        _x3 += dx;
        _y3 += dy;
    }

    public void Change( IReadOnlyList<string> parameters )
    {
        if ( parameters.Count != 6 )
            throw new ArgumentException( "Triangle requires 6 parameters: x1 y1 x2 y2 x3 y3" );

        _x1 = double.Parse( parameters[ 0 ], CultureInfo.InvariantCulture );
        _y1 = double.Parse( parameters[ 1 ], CultureInfo.InvariantCulture );
        _x2 = double.Parse( parameters[ 2 ], CultureInfo.InvariantCulture );
        _y2 = double.Parse( parameters[ 3 ], CultureInfo.InvariantCulture );
        _x3 = double.Parse( parameters[ 4 ], CultureInfo.InvariantCulture );
        _y3 = double.Parse( parameters[ 5 ], CultureInfo.InvariantCulture );
    }

    public string ParametersToString()
    {
        return $"{_x1.ToString( CultureInfo.InvariantCulture )} " +
               $"{_y1.ToString( CultureInfo.InvariantCulture )} " +
               $"{_x2.ToString( CultureInfo.InvariantCulture )} " +
               $"{_y2.ToString( CultureInfo.InvariantCulture )} " +
               $"{_x3.ToString( CultureInfo.InvariantCulture )} " +
               $"{_y3.ToString( CultureInfo.InvariantCulture )}";
    }
}
