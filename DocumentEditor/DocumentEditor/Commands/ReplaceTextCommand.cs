using DocumentEditor.Model;
using DocumentEditor.Paragraph;

namespace DocumentEditor.Commands;
// ReplaceTextCommand with merging for same paragraph object
public class ReplaceTextCommand : ICommand
{
    private readonly Document _doc;
    private readonly ParagraphItem _paragraph; // direct reference
    private readonly string _newText;
    private string _oldText;

    public ReplaceTextCommand( Document doc, ParagraphItem paragraph, string newText )
    {
        _doc = doc;
        _paragraph = paragraph;
        _newText = newText;
    }

    public void Execute()
    {
        _oldText = _paragraph.GetText();
        _paragraph.SetText( _newText );
    }

    public void Unexecute()
    {
        _paragraph.SetText( _oldText );
    }

    public bool CanMergeWith( ICommand other )
    {
        if ( other is ReplaceTextCommand r && ReferenceEquals( _doc, r._doc ) )
        {
            // Merge only if operate on same paragraph object
            return _paragraph.Id == r._paragraph.Id;
        }
        return false;
    }

    public void MergeWith( ICommand other )
    {
        if ( !( other is ReplaceTextCommand r ) )
            throw new InvalidOperationException();
        // Apply new text to document (since other wasn't executed)
        _paragraph.SetText( r._newText );
        // Note: oldText must remain the original before first Execute to allow a single Undo to restore original state.
    }

    public string Describe() => $"ReplaceText on paragraph {_paragraph.Id} -> '{_newText}'";
}