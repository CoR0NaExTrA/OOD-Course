// File: Shapes/RectangleShape.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public class RectangleShape : ShapeBase
{
    public RectangleShape( Rect frame, LineStyle line, FillStyle fill ) 
    { 
        _frame = frame; 
        _lineStyle = line; 
        _fillStyle = fill; 
    }
    public override void Draw( ICanvas canvas )
    {
        if ( FillStyle.Enabled )
        {
            canvas.SetFillColor( FillStyle.Color );
            var p = new (double x, double y)[] { (Frame.Left, Frame.Top), (Frame.Right, Frame.Top), (Frame.Right, Frame.Bottom), (Frame.Left, Frame.Bottom) };
            canvas.FillPolygon( p );
        }
        if ( LineStyle.Enabled )
        {
            canvas.SetLineColor( LineStyle.Color );
            canvas.SetLineThickness( LineStyle.Thickness );
            canvas.DrawLine( Frame.Left, Frame.Top, Frame.Right, Frame.Top );
            canvas.DrawLine( Frame.Right, Frame.Top, Frame.Right, Frame.Bottom );
            canvas.DrawLine( Frame.Right, Frame.Bottom, Frame.Left, Frame.Bottom );
            canvas.DrawLine( Frame.Left, Frame.Bottom, Frame.Left, Frame.Top );
        }
    }
}