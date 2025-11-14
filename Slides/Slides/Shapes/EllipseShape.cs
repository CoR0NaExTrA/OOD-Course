// File: Shapes/EllipseShape.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public class EllipseShape : ShapeBase
{
    public EllipseShape( Rect frame, LineStyle line, FillStyle fill ) 
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
            canvas.FillEllipse( Frame ); 
        }
        if ( LineStyle.Enabled )
        { 
            canvas.SetLineColor( LineStyle.Color ); 
            canvas.SetLineThickness( LineStyle.Thickness );
            canvas.DrawEllipse( Frame ); 
        }
    }
}