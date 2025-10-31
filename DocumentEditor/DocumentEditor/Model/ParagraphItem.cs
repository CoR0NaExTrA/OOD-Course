using DocumentEditor.Interfaces;

namespace DocumentEditor.Model;
public class ParagraphItem : DocumentItem, IParagraph
{
    private string _text;
    public ParagraphItem( string text ) => _text = text;
    public string GetText() => _text;
    public void SetText( string text ) => _text = text;
}
