using DocumentEditor.Interfaces;
using DocumentEditor.Model;

namespace DocumentEditor.Commands;
// InsertImageCommand
public class InsertImageCommand : ICommand, IResourceAffecting
{
    private readonly Document _doc;
    private readonly string _sourcePath;
    private readonly int _width, _height;
    private readonly int? _position;
    private ImageItem _created;
    private string _copiedRelativePath; // images/filename
    private bool _isUndone = false;

    public InsertImageCommand( Document doc, string sourcePath, int width, int height, int? position )
    {
        _doc = doc;
        _sourcePath = sourcePath;
        _width = width;
        _height = height;
        _position = position;
        ValidateDimensions();
    }

    private void ValidateDimensions()
    {
        if ( _width < 1 || _width > 10000 || _height < 1 || _height > 10000 )
            throw new ArgumentException( "Image dimensions out of allowed range 1..10000" );
    }

    public void Execute()
    {
        // Copy happens inside Document.InsertImageFromPath (which places file into workingDir/images)
        _created = _doc.InsertImageFromPath( _sourcePath, _width, _height, _position );
        _copiedRelativePath = _created.GetPath();
    }

    public void Unexecute()
    {
        // On undo, we should mark resource for deletion (not physically delete)
        if ( _created == null )
            return;
        var item = _doc.GetItemById( _created.Id );
        if ( item == null )
            return;
        // mark for deletion and remove from document
        if ( item is ImageItem img )
        {
            img.IsMarkedDeleted = true;
        }
        var idx = -1;
        for ( int i = 0; i < _doc.GetItemsCount(); i++ )
            if ( _doc.GetItem( i ).Id == item.Id )
            { idx = i; break; }
        if ( idx >= 0 )
            _doc.DeleteItemAt( idx );

        _isUndone = true;
    }

    public bool CanMergeWith( ICommand other ) => false;
    public void MergeWith( ICommand other ) => throw new NotSupportedException();
    public string Describe() => $"InsertImage '{_sourcePath}' -> {_copiedRelativePath}";

    // When this command is removed from history permanently:
    public void OnHistoryRemoved( bool removedFromUndone )
    {
        // Cases per spec:
        // - If this command is removed while it was in undone stack (removedFromUndone=true) => it had been undone earlier and so resource was marked for deletion.
        //   Now we should physically delete the resource (since branch where it was created removed).
        // - If removedFromUndone=false and it's an executed command removed due history overflow => perform physical deletion as well (the ability to undo is lost).
        _doc.DeleteResourceFileIfExists( _copiedRelativePath );
    }
}
