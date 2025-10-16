using System.Globalization;
using System.Text;

namespace Factory;
public class SvgCanvas : ICanvas
{
    private readonly StreamWriter _w;
    private readonly int _width;
    private readonly int _height;
    private Color _currentColor = Color.Black;
    private readonly List<string> _elements = new List<string>();


    public SvgCanvas( string path, int width, int height )
    {
        _width = width;
        _height = height;
        _w = new StreamWriter( path, false, Encoding.UTF8 );
        _w.WriteLine( $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{_width}\" height=\"{_height}\">\n" );
    }


    public void SetColor( Color c )
    {
        _currentColor = c;
        // also emit a comment line to stdout for primitive commands
        Console.WriteLine( $"SetColor {_currentColor}" );
    }


    private string ColorToSvg( Color c )
    {
        switch ( c )
        {
            case Color.Green:
                return "green";
            case Color.Red:
                return "red";
            case Color.Blue:
                return "blue";
            case Color.Yellow:
                return "yellow";
            case Color.Pink:
                return "pink";
            case Color.Black:
                return "black";
            default:
                return "black";
        }
    }


    public void DrawLine( Point from, Point to )
    {
        Console.WriteLine( $"DrawLine {from} -> {to}" );
        var s = string.Format( CultureInfo.InvariantCulture,
        "<line x1=\"{0:0.##}\" y1=\"{1:0.##}\" x2=\"{2:0.##}\" y2=\"{3:0.##}\" stroke=\"{4}\" stroke-width=\"1\" />",
        from.X, from.Y, to.X, to.Y, ColorToSvg( _currentColor ) );
        _elements.Add( s );
    }


    public void DrawEllipse( double left, double top, double width, double height )
    {
        Console.WriteLine( $"DrawEllipse left={left} top={top} w={width} h={height}" );
        double cx = left + width / 2.0;
        double cy = top + height / 2.0;
        double rx = width / 2.0;
        double ry = height / 2.0;
        var s = string.Format( CultureInfo.InvariantCulture,
        "<ellipse cx=\"{0:0.##}\" cy=\"{1:0.##}\" rx=\"{2:0.##}\" ry=\"{3:0.##}\" stroke=\"{4}\" fill=\"none\" stroke-width=\"1\" />",
        cx, cy, rx, ry, ColorToSvg( _currentColor ) );
        _elements.Add( s );
    }


    public void Dispose()
    {
        foreach ( var e in _elements )
            _w.WriteLine( e );
        _w.WriteLine( "</svg>" );
        _w.Flush();
        _w.Dispose();
    }
}