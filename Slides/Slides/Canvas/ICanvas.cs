// File: Interfaces/ICanvas.cs
using Slides.Models;

namespace Slides.Canvas;
public interface ICanvas
{
    void SetFillColor( ColorRGBA color );
    void SetLineColor( ColorRGBA color );
    void SetLineThickness( double thickness );
    void DrawLine( double x1, double y1, double x2, double y2 );
    void DrawEllipse( Rect frame );
    void FillEllipse( Rect frame );
    void FillPolygon( (double x, double y)[] points );
}