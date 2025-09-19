using System.Globalization;
using Shapes.Interface.gfx;
using Shapes.Interface.shapes;

namespace Shapes.Class.shapes;

public class RectangleStrategy : IShapeStrategy
{
    private double _x, _y, _width, _height;

    public RectangleStrategy( double x, double y, double width, double height )
    {
        if ( width < 0 || height < 0 )
            throw new ArgumentException( "Width and height must be non-negative" );

        _x = x;
        _y = y;
        _width = width;
        _height = height;
    }

    public string GetTypeName() => "rectangle";

    public void Draw( ICanvas canvas, string color )
    {
        canvas.SetColor( color );
        canvas.MoveTo( _x, _y );
        canvas.LineTo( _x + _width, _y );
        canvas.LineTo( _x + _width, _y + _height );
        canvas.LineTo( _x, _y + _height );
        canvas.LineTo( _x, _y );
    }

    public void Move( double dx, double dy )
    {
        _x += dx;
        _y += dy;
    }

    public void Change( IReadOnlyList<string> parameters )
    {
        if ( parameters.Count != 4 )
            throw new ArgumentException( "Rectangle requires 4 parameters: x y width height" );

        _x = double.Parse( parameters[ 0 ], CultureInfo.InvariantCulture );
        _y = double.Parse( parameters[ 1 ], CultureInfo.InvariantCulture );
        _width = double.Parse( parameters[ 2 ], CultureInfo.InvariantCulture );
        _height = double.Parse( parameters[ 3 ], CultureInfo.InvariantCulture );
    }

    public string ParametersToString()
    {
        return $"{_x.ToString( CultureInfo.InvariantCulture )} " +
               $"{_y.ToString( CultureInfo.InvariantCulture )} " +
               $"{_width.ToString( CultureInfo.InvariantCulture )} " +
               $"{_height.ToString( CultureInfo.InvariantCulture )}";
    }
}
