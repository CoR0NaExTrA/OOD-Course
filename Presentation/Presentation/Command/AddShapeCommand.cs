using Presentation.Shapes;

namespace Presentation.Command;

public class AddShapeCommand : ICommand
{
    private readonly Document doc;
    private readonly Shape shape;

    public AddShapeCommand( Document doc, Shape shape )
    {
        this.doc = doc;
        this.shape = shape;
    }

    public void Execute() => doc.Shapes.Add( shape );

    public void Undo() => doc.Shapes.Remove( shape );
}