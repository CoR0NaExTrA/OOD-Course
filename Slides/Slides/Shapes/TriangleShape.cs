// File: Shapes/TriangleShape.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public class TriangleShape : ShapeBase
{
    public TriangleShape( Rect frame, LineStyle line, FillStyle fill ) 
    { 
        _frame = frame; 
        _lineStyle = line; 
        _fillStyle = fill; 
    }

    public override void Draw( ICanvas canvas )
    {
        var top = ( Frame.Left + Frame.Right ) / 2.0;
        var points = new (double x, double y)[] { (top, Frame.Top), (Frame.Right, Frame.Bottom), (Frame.Left, Frame.Bottom) };
        if ( FillStyle.Enabled )
        { 
            canvas.SetFillColor( FillStyle.Color ); 
            canvas.FillPolygon( points ); 
        }
        if ( LineStyle.Enabled )
        {
            canvas.SetLineColor( LineStyle.Color );
            canvas.SetLineThickness( LineStyle.Thickness );
            canvas.DrawLine( points[ 0 ].x, points[ 0 ].y, points[ 1 ].x, points[ 1 ].y );
            canvas.DrawLine( points[ 1 ].x, points[ 1 ].y, points[ 2 ].x, points[ 2 ].y );
            canvas.DrawLine( points[ 2 ].x, points[ 2 ].y, points[ 0 ].x, points[ 0 ].y );
        }
    }
}