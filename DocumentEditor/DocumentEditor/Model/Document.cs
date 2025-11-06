using DocumentEditor.Commands;
using DocumentEditor.Image;
using DocumentEditor.Paragraph;
using DocumentEditor.Utils;
using System.Text;

namespace DocumentEditor.Model;

public class Document : IDisposable
{
    private readonly List<DocumentItem> _items = new();
    private string _title = "";
    private readonly string _workingDir;
    private readonly string _imagesDir;

    private readonly CommandHistory _history;

    public Document( string workingDir )
    {
        _workingDir = Path.GetFullPath( workingDir );
        Directory.CreateDirectory( _workingDir );
        _imagesDir = Path.Combine( _workingDir, "images" );
        Directory.CreateDirectory( _imagesDir );

        _history = new CommandHistory( this );
    }

    public void Dispose() { }

    // === Работа с историей ===
    public void ExecuteCommand( ICommand cmd ) => _history.ExecuteCommand( cmd );
    public void Undo() => _history.Undo();
    public void Redo() => _history.Redo();
    public bool CanUndo() => _history.CanUndo();
    public bool CanRedo() => _history.CanRedo();

    // === Основные методы документа ===
    public DocumentItem InsertParagraph( string text, int? position = null )
    {
        var p = new ParagraphItem( text );
        if ( position.HasValue )
        {
            var pos = position.Value;
            if ( pos < 0 || pos > _items.Count )
                throw new ArgumentOutOfRangeException( nameof( position ) );
            _items.Insert( pos, p );
        }
        else
            _items.Add( p );
        return p;
    }

    public ImageItem InsertImageFromPath( string sourcePath, int width, int height, int? position = null )
    {
        if ( !File.Exists( sourcePath ) )
            throw new FileNotFoundException( "Source image not found", sourcePath );

        var ext = Path.GetExtension( sourcePath );
        var fileName = $"img_{Guid.NewGuid():N}{ext}";
        var destRel = Path.Combine( "images", fileName );
        var destFull = Path.Combine( _workingDir, destRel );
        Directory.CreateDirectory( Path.GetDirectoryName( destFull ) ?? _workingDir );
        File.Copy( sourcePath, destFull );
        var img = new ImageItem( destRel.Replace( '\\', '/' ), width, height );

        if ( position.HasValue )
        {
            var pos = position.Value;
            if ( pos < 0 || pos > _items.Count )
                throw new ArgumentOutOfRangeException( nameof( position ) );
            _items.Insert( pos, img );
        }
        else
            _items.Add( img );

        return img;
    }

    public int GetItemsCount() => _items.Count;
    public DocumentItem GetItem( int index ) => _items[ index ];
    public DocumentItem GetItemById( Guid id ) => _items.FirstOrDefault( i => i.Id == id );
    public void DeleteItemAt( int index ) => _items.RemoveAt( index );
    public string GetTitle() => _title;
    public void SetTitle( string title ) => _title = title;
    public string GetWorkingDir() => _workingDir;
    public string GetImagesDir() => _imagesDir;

    public void DeleteResourceFileIfExists( string relativePath )
    {
        var full = Path.Combine( _workingDir, relativePath.Replace( '/', Path.DirectorySeparatorChar ) );
        if ( File.Exists( full ) )
        {
            try
            { File.Delete( full ); }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Warning deleting file {full}: {ex.Message}" );
            }
        }
    }

    // === Сохранение ===
    public void SaveAsHtml( string htmlPath )
    {
        var fullHtmlPath = Path.Combine( _workingDir, Path.GetFileName( htmlPath ) );
        var sb = new StringBuilder();
        sb.AppendLine( "<!doctype html>" );
        sb.AppendLine( "<html><head>" );
        sb.AppendLine( $"  <meta charset=\"utf-8\"/>" );
        sb.AppendLine( $"  <title>{HtmlUtil.Escape( _title )}</title>" );
        sb.AppendLine( "</head><body>" );
        sb.AppendLine( $"<h1>{HtmlUtil.Escape( _title )}</h1>" );

        foreach ( var item in _items )
        {
            switch ( item )
            {
                case ParagraphItem p:
                    sb.AppendLine( $"<p>{HtmlUtil.Escape( p.GetText() )}</p>" );
                    break;
                case ImageItem img when !img.IsMarkedDeleted:
                    sb.AppendLine( $"<img src=\"{HtmlUtil.Escape( img.GetPath() )}\" width=\"{img.GetWidth()}\" height=\"{img.GetHeight()}\" />" );
                    break;
            }
        }

        sb.AppendLine( "</body></html>" );
        File.WriteAllText( fullHtmlPath, sb.ToString(), Encoding.UTF8 );
        Console.WriteLine( $"Saved HTML to {fullHtmlPath}" );
    }
}
