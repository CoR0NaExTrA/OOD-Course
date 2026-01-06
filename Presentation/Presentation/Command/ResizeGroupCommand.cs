using Presentation.Shapes;
using SkiaSharp;

namespace Presentation.Command;
public class ResizeGroupCommand : ICommand
{
    private readonly List<(Shape shape, SKRect before, SKRect after)> items;

    public ResizeGroupCommand(
        Dictionary<Shape, SKRect> beforeMap,
        IEnumerable<Shape> shapes )
    {
        items = shapes
            .Select( s => (s, beforeMap[ s ], s.Bounds) )
            .ToList();
    }

    public void Execute()
    {
        foreach ( var (s, _, after) in items )
            s.Bounds = after;
    }

    public void Undo()
    {
        foreach ( var (s, before, _) in items )
            s.Bounds = before;
    }
}
