using Presentation.Shapes;
using System;
using System.IO;

namespace Presentation.Command;

public class AddImageCommand : ICommand
{
    private readonly Document doc;
    private readonly ImageShape shape;

    public AddImageCommand( Document doc, ImageShape shape )
    {
        this.doc = doc ?? throw new ArgumentNullException( nameof( doc ) );
        this.shape = shape ?? throw new ArgumentNullException( nameof( shape ) );
    }

    public void Execute()
    {
        doc.Shapes.Add( shape );
    }

    public void Undo()
    {
        doc.Shapes.Remove( shape );
    }
}
