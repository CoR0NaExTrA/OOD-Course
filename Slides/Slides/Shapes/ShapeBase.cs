// File: Shapes/ShapeBase.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public abstract class ShapeBase : IShape
{
    protected Rect _frame;
    protected LineStyle _lineStyle;
    protected FillStyle _fillStyle;

    public virtual Rect Frame
    {
        get => _frame;
        set 
        { 
            _frame = value; 
            FrameChanged?.Invoke( this, EventArgs.Empty ); 
        }
    }
    public virtual LineStyle LineStyle
    {
        get => _lineStyle;
        set 
        { 
            _lineStyle = value ?? throw new ArgumentNullException( nameof( LineStyle ) ); 
            StyleChanged?.Invoke( this, EventArgs.Empty ); 
        }
    }
    public virtual FillStyle FillStyle
    {
        get => _fillStyle;
        set
        {
            _fillStyle = value ?? throw new ArgumentNullException( nameof( FillStyle ) ); 
            StyleChanged?.Invoke( this, EventArgs.Empty ); 
        }
    }
    public abstract void Draw( ICanvas canvas );
    public event EventHandler FrameChanged;
    public event EventHandler StyleChanged;
}