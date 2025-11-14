// File: Canvas/ConsoleCanvas.cs

using Slides.Models;

namespace Slides.Canvas;

public class ConsoleCanvas : ICanvas
{
    private ColorRGBA _fillColor; 
    private ColorRGBA _lineColor; 
    private double _thickness;

    public void SetFillColor( ColorRGBA color ) 
    { 
        _fillColor = color; 
        Console.WriteLine( $"SetFillColor {_fillColor}" ); 
    }
    public void SetLineColor( ColorRGBA color ) 
    { 
        _lineColor = color; 
        Console.WriteLine( $"SetLineColor {_lineColor}" ); 
    }
    public void SetLineThickness( double thickness ) 
    { 
        _thickness = thickness; 
        Console.WriteLine( $"SetLineThickness {thickness:0.##}" ); 
    }
    public void DrawLine( double x1, double y1, double x2, double y2 ) 
    { 
        Console.WriteLine( $"DrawLine ({x1:0.##},{y1:0.##}) -> ({x2:0.##},{y2:0.##})" ); 
    }
    public void DrawEllipse( Rect frame ) 
    { 
        Console.WriteLine( $"DrawEllipse {frame}" ); 
    }
    public void FillEllipse( Rect frame ) 
    { 
        Console.WriteLine( $"FillEllipse {frame}" ); 
    }
    public void FillPolygon( (double x, double y)[] points ) 
    { 
        Console.Write( "FillPolygon " ); 
        Console.WriteLine( string.Join( " ", points ) ); 
    }
}