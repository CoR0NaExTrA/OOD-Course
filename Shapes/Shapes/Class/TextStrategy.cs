using System.Globalization;
using Shapes.Interface.gfx;
using Shapes.Interface.shapes;

namespace Shapes.Class.shapes;

public class TextStrategy : IShapeStrategy
{
    private double _x, _y, _fontSize;
    private string _text;

    public TextStrategy( double x, double y, double fontSize, string text )
    {
        _x = x;
        _y = y;
        _fontSize = fontSize;
        _text = text;
    }

    public string GetTypeName() => "text";

    public void Draw( ICanvas canvas, string color )
    {
        canvas.SetColor( color );
        canvas.DrawText( _x, _y, _fontSize, _text );
    }

    public void Move( double dx, double dy )
    {
        _x += dx;
        _y += dy;
    }

    public void Change( IReadOnlyList<string> parameters )
    {
        if ( parameters.Count < 4 )
            throw new ArgumentException( "Text requires at least 4 parameters: x y fontSize text..." );

        _x = double.Parse( parameters[ 0 ], CultureInfo.InvariantCulture );
        _y = double.Parse( parameters[ 1 ], CultureInfo.InvariantCulture );
        _fontSize = double.Parse( parameters[ 2 ], CultureInfo.InvariantCulture );
        _text = string.Join( " ", parameters.Skip( 3 ) );
    }

    public string ParametersToString()
    {
        return $"{_x.ToString( CultureInfo.InvariantCulture )} " +
               $"{_y.ToString( CultureInfo.InvariantCulture )} " +
               $"{_fontSize.ToString( CultureInfo.InvariantCulture )} " +
               $"{_text}";
    }
}
