using DocumentEditor.Model;

namespace DocumentEditor.Image;
public class ImageItem : DocumentItem, IImage
{
    private string _relativePath;
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