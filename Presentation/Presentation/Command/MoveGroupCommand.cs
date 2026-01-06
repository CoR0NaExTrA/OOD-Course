using Presentation.Shapes;
using SkiaSharp;

namespace Presentation.Command;

public class MoveGroupCommand : ICommand
{
    private readonly List<(Shape shape, SKRect before, SKRect after)> items;

    public MoveGroupCommand(
        IEnumerable<Shape> shapes,
        Dictionary<Shape, SKRect> beforeMap )
    {
        items = shapes
            .Select( s => (s, beforeMap[ s ], s.Bounds) )
            .ToList();
    }

    public void Execute()
    {
        foreach ( var (shape, _, after) in items )
            shape.Bounds = after;
    }

    public void Undo()
    {
        foreach ( var (shape, before, _) in items )
            shape.Bounds = before;
    }
}
