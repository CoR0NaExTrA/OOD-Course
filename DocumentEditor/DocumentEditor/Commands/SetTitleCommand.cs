using DocumentEditor.Interfaces;
using DocumentEditor.Model;

namespace DocumentEditor.Commands;
// SetTitleCommand with merge ability
class SetTitleCommand : ICommand
{
    private readonly Document _doc;
    private readonly string _newTitle;
    private string _oldTitle;
    public SetTitleCommand( Document doc, string newTitle )
    {
        _doc = doc;
        _newTitle = newTitle;
    }
    public void Execute()
    {
        _oldTitle = _doc.GetTitle();
        _doc.SetTitle( _newTitle );
    }
    public void Unexecute()
    {
        _doc.SetTitle( _oldTitle );
    }
    public bool CanMergeWith( ICommand other )
    {
        if ( other is SetTitleCommand st && ReferenceEquals( _doc, st._doc ) )
        {
            // Only merge if no other modifying actions between them (caller ensures sequentiality)
            return true;
        }
        return false;
    }
    public void MergeWith( ICommand other )
    {
        if ( !( other is SetTitleCommand st ) )
            throw new InvalidOperationException();
        // Keep original _oldTitle, but update _newTitle to other's newTitle (so final effect is setting to other's value)
        // To implement, we need to set the doc to newTitle now (other was presumably not executed separately),
        // but in our Execute flow we called Execute on first, then when adding second, Document.ExecuteCommand checks last.CanMergeWith and then calls last.MergeWith(cmd) WITHOUT calling cmd.Execute().
        // So MergeWith should apply the final change to document.
        _doc.SetTitle( st._newTitle );
        // update internal new title representation if needed (not necessary beyond doc state)
    }
    public string Describe() => $"SetTitle -> '{_newTitle}'";
}
