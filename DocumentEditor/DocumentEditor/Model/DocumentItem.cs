namespace DocumentEditor.Model;
// Документный элемент

public abstract class DocumentItem
{
    public Guid Id { get; } = Guid.NewGuid();
}
