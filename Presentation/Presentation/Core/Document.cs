using Presentation.Command;
using Presentation.Core;
using Presentation.Shapes;

public class Document
{
    public List<Shape> Shapes { get; } = new();

    public ImageRepository ImageRepo { get; } = new ImageRepository();

    public UndoRedoManager History { get; } = new();
}
