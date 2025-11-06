namespace DocumentEditor.Image;
public interface IImage
{
    string GetPath(); // path relative to document folder (images/...)
    int GetWidth();
    int GetHeight();
    void Resize( int width, int height );
    bool IsMarkedDeleted { get; set; } // helper for resource lifecycle
}