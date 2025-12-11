using Presentation.Shapes;
using SkiaSharp;

namespace Presentation.Command;

public class ResizeShapeCommand : ICommand
{
    private readonly Shape shape;
    private readonly SKRect before;
    private readonly SKRect after;

    public ResizeShapeCommand( Shape shape, SKRect before, SKRect after )
    {
        this.shape = shape;
        this.before = before;
        this.after = after;
    }

    public void Execute() => shape.Bounds = after;

    public void Undo() => shape.Bounds = before;
}
