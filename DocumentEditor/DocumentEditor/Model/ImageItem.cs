using DocumentEditor.Interfaces;

namespace DocumentEditor.Model;
public class ImageItem : DocumentItem, IImage
{
    private string _relativePath; // relative to document HTML directory (e.g. images/img1.png)
    private int _width, _height;
    public bool IsMarkedDeleted { get; set; } = false;

    public ImageItem( string relativePath, int width, int height )
    {
        _relativePath = relativePath;
        _width = width;
        _height = height;
    }

    public string GetPath() => _relativePath;
    public int GetWidth() => _width;
    public int GetHeight() => _height;
    public void Resize( int width, int height )
    {
        _width = width;
        _height = height;
    }
}