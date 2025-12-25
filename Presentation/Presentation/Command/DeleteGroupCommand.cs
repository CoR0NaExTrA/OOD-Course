using Presentation.Command;
using Presentation.Shapes;

public class DeleteGroupCommand : ICommand
{
    private readonly Document doc;
    private readonly List<(Shape shape, int index)> removed = new();

    public DeleteGroupCommand( Document doc, IEnumerable<Shape> shapes )
    {
        this.doc = doc;
        foreach ( var s in shapes )
            removed.Add( (s, doc.Shapes.IndexOf( s )) );
    }

    public void Execute()
    {
        foreach ( var (s, _) in removed )
            doc.Shapes.Remove( s );
    }

    public void Undo()
    {
        foreach ( var (s, i) in removed.OrderBy( r => r.index ) )
            doc.Shapes.Insert( i, s );
    }
}
