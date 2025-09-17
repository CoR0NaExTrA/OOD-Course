using System.Text;
using System.Globalization;
using Shapes.Interface.gfx;

namespace Shapes.Class.gfx;

public class SvgCanvas : ICanvas, IDisposable
{
    private readonly StringBuilder _sb = new();
    private readonly string _filePath;
    private string _currentColor = "#000000";

    public SvgCanvas( string filePath, int width = 800, int height = 600 )
    {
        _filePath = filePath ?? throw new ArgumentNullException( nameof( filePath ) );

        if ( File.Exists( _filePath ) )
        {
            var existing = File.ReadAllText( _filePath );
            var idx = existing.LastIndexOf( "</svg>", StringComparison.OrdinalIgnoreCase );
            if ( idx >= 0 )
            {
                _sb.Append( existing.Substring( 0, idx ) );
                if ( !_sb.ToString().EndsWith( Environment.NewLine ) )
                    _sb.AppendLine();
            }
            else
            {
                _sb.AppendLine( $"<svg xmlns='http://www.w3.org/2000/svg' width='{width}' height='{height}'>" );
                _sb.AppendLine( existing );
            }
        }
        else
        {
            _sb.AppendLine( $"<svg xmlns='http://www.w3.org/2000/svg' width='{width}' height='{height}'>" );
        }
    }

    public void SetColor( string color ) => _currentColor = color;

    public void MoveTo( double x, double y ) {}

    public void LineTo( double x, double y ) {}

    public void DrawEllipse( double cx, double cy, double rx, double ry )
    {
        _sb.AppendLine( $"<ellipse cx='{cx.ToString( CultureInfo.InvariantCulture )}' cy='{cy.ToString( CultureInfo.InvariantCulture )}' rx='{rx.ToString( CultureInfo.InvariantCulture )}' ry='{ry.ToString( CultureInfo.InvariantCulture )}' fill='none' stroke='{_currentColor}' />" );
    }

    public void DrawText( double left, double top, double fontSize, string text )
    {
        var escaped = System.Security.SecurityElement.Escape( text ) ?? "";
        _sb.AppendLine( $"<text x='{left.ToString( CultureInfo.InvariantCulture )}' y='{top.ToString( CultureInfo.InvariantCulture )}' font-size='{fontSize.ToString( CultureInfo.InvariantCulture )}' fill='{_currentColor}'>{escaped}</text>" );
    }

    public void Dispose()
    {
        _sb.AppendLine( "</svg>" );
        File.WriteAllText( _filePath, _sb.ToString() );
    }
}
