using DocumentEditor.Interfaces;
using DocumentEditor.Model;

namespace DocumentEditor.Commands;
// ResizeImageCommand with merging for same image object
public class ResizeImageCommand : ICommand
{
    private readonly Document _doc;
    private readonly ImageItem _image;
    private readonly int _newW, _newH;
    private int _oldW, _oldH;

    public ResizeImageCommand( Document doc, ImageItem image, int newW, int newH )
    {
        _doc = doc;
        _image = image;
        _newW = newW;
        _newH = newH;
    }

    public void Execute()
    {
        _oldW = _image.GetWidth();
        _oldH = _image.GetHeight();
        _image.Resize( _newW, _newH );
    }

    public void Unexecute()
    {
        _image.Resize( _oldW, _oldH );
    }

    public bool CanMergeWith( ICommand other )
    {
        if ( other is ResizeImageCommand r && ReferenceEquals( _doc, r._doc ) )
        {
            return _image.Id == r._image.Id;
        }
        return false;
    }

    public void MergeWith( ICommand other )
    {
        if ( !( other is ResizeImageCommand r ) )
            throw new InvalidOperationException();
        _image.Resize( r._newW, r._newH );
    }

    public string Describe() => $"ResizeImage {_image.Id} -> {_newW}x{_newH}";
}
