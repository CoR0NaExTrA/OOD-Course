// File: Canvas/SvgCanvas.cs
using Slides.Models;
using System.Globalization;
using System.Text;

namespace Slides.Canvas;

public class SvgCanvas : ICanvas
{
    private readonly StringBuilder _content = new StringBuilder();
    private ColorRGBA _fill;
    private ColorRGBA _stroke;
    private double _thickness;
    private readonly int _width;
    private readonly int _height;
    public SvgCanvas( int width, int height ) 
    { 
        _width = width; _height = height; 
    }

    public void SetFillColor( ColorRGBA color ) 
    { 
        _fill = color; 
    }
    public void SetLineColor( ColorRGBA color ) 
    { 
        _stroke = color; 
    }
    public void SetLineThickness( double thickness ) 
    { 
        _thickness = thickness; 
    }

    private static string SvgColor( ColorRGBA c ) => $"{c.R},{c.G},{c.B}";
    private static string SvgOpacity( ColorRGBA c ) => ( c.A / 255.0 ).ToString( CultureInfo.InvariantCulture );

    public void DrawLine( double x1, double y1, double x2, double y2 )
    {
        var line = $"<line x1=\"{x1:0.##}\" y1=\"{y1:0.##}\" x2=\"{x2:0.##}\" y2=\"{y2:0.##}\" stroke=\"rgb({SvgColor( _stroke )})\" stroke-opacity=\"{SvgOpacity( _stroke )}\" stroke-width=\"{_thickness:0.##}\" />\n";
        _content.Append( line );
    }

    public void DrawEllipse( Rect frame )
    {
        double cx = frame.Left + frame.Width / 2.0;
        double cy = frame.Top + frame.Height / 2.0;
        double rx = frame.Width / 2.0;
        double ry = frame.Height / 2.0;
        var el = $"<ellipse cx=\"{cx:0.##}\" cy=\"{cy:0.##}\" rx=\"{rx:0.##}\" ry=\"{ry:0.##}\" fill=\"none\" stroke=\"rgb({SvgColor( _stroke )})\" stroke-opacity=\"{SvgOpacity( _stroke )}\" stroke-width=\"{_thickness:0.##}\" />\n";
        _content.Append( el );
    }

    public void FillEllipse( Rect frame )
    {
        double cx = frame.Left + frame.Width / 2.0;
        double cy = frame.Top + frame.Height / 2.0;
        double rx = frame.Width / 2.0;
        double ry = frame.Height / 2.0;
        var el = $"<ellipse cx=\"{cx:0.##}\" cy=\"{cy:0.##}\" rx=\"{rx:0.##}\" ry=\"{ry:0.##}\" fill=\"rgb({SvgColor( _fill )})\" fill-opacity=\"{SvgOpacity( _fill )}\" stroke=\"none\" />\n";
        _content.Append( el );
    }

    public void FillPolygon( (double x, double y)[] points )
    {
        var pts = string.Join( " ", points.Select( p => $"{p.x:0.##},{p.y:0.##}" ) );
        var poly = $"<polygon points=\"{pts}\" fill=\"rgb({SvgColor( _fill )})\" fill-opacity=\"{SvgOpacity( _fill )}\" stroke=\"none\" />\n";
        _content.Append( poly );
    }

    public void Save( string path )
    {
        var sb = new StringBuilder();
        sb.AppendLine( $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{_width}\" height=\"{_height}\">\n" );
        sb.Append( _content.ToString() );
        sb.AppendLine( "</svg>" );
        File.WriteAllText( path, sb.ToString() );
    }
}