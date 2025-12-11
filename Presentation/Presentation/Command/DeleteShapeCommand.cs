using Presentation.Shapes;

namespace Presentation.Command;

public class DeleteShapeCommand : ICommand
{
    private readonly Document doc;
    private readonly Shape shape;
    private int oldIndex;

    public DeleteShapeCommand( Document doc, Shape shape )
    {
        this.doc = doc ?? throw new ArgumentNullException( nameof( doc ) );
        this.shape = shape ?? throw new ArgumentNullException( nameof( shape ) );
    }

    public void Execute()
    {
        oldIndex = doc.Shapes.IndexOf( shape );

        if ( oldIndex >= 0 )
            doc.Shapes.RemoveAt( oldIndex );

        shape.IsSelected = false;
    }

    public void Undo()
    {
        if ( oldIndex >= 0 )
            doc.Shapes.Insert( oldIndex, shape );
    }
}
