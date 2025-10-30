namespace DocumentEditor.Model;
// Документный элемент

abstract class DocumentItem
{
    public Guid Id { get; } = Guid.NewGuid();
}
