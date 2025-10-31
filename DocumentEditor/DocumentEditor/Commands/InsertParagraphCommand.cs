using DocumentEditor.Interfaces;
using DocumentEditor.Model;

namespace DocumentEditor.Commands;
// InsertParagraphCommand
public class InsertParagraphCommand : ICommand
{
    private readonly Document _doc;
    private readonly string _text;
    private readonly int? _position; // 0-based insert position (if null => end)
    private ParagraphItem _created;

    public InsertParagraphCommand( Document doc, string text, int? position )
    {
        _doc = doc;
        _text = text;
        _position = position;
    }

    public void Execute()
    {
        _created = ( ParagraphItem )_doc.InsertParagraph( _text, _position );
    }

    public void Unexecute()
    {
        // find by id and remove
        var item = _doc.GetItemById( _created.Id );
        if ( item == null )
            return;
        // find index
        var listIndex = GetIndexOf( item );
        if ( listIndex >= 0 )
            _doc.DeleteItemAt( listIndex );
    }

    public bool CanMergeWith( ICommand other ) => false;
    public void MergeWith( ICommand other ) => throw new NotSupportedException();
    public string Describe() => $"InsertParagraph '{_text}'";
    private int GetIndexOf( DocumentItem item )
    {
        for ( int i = 0; i < _doc.GetItemsCount(); i++ )
        {
            if ( _doc.GetItem( i ).Id == item.Id )
                return i;
        }
        return -1;
    }
}
