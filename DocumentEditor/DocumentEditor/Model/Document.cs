using DocumentEditor.Interfaces;
using DocumentEditor.Utils;
using System.Text;

namespace DocumentEditor.Model;
// Документ - основной класс, инкапсулирует историю, элементы, операции

public class Document : IDisposable
{
    private readonly List<DocumentItem> _items = new();
    private string _title = "";
    private readonly string _workingDir; // каталог рабочей версии (куда будут копироваться изображения при InsertImage)
    private readonly string _imagesDir;

    // История команд: список выполненных команд в порядке выполнения (макс 10)
    private readonly List<ICommand> _history = new();
    // Стек отменённых команд (для Redo). Если после Undo выполнена новая команда, этот стек очищается.
    private readonly Stack<ICommand> _undone = new();

    private const int MaxHistory = 10;

    public Document( string workingDir )
    {
        _workingDir = Path.GetFullPath( workingDir );
        Directory.CreateDirectory( _workingDir );
        _imagesDir = Path.Combine( _workingDir, "images" );
        Directory.CreateDirectory( _imagesDir );
    }

    public void Dispose()
    {
        // nothing special
    }

    // IDocument-like API
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
        {
            _items.Add( p );
        }
        return p;
    }

    public ImageItem InsertImageFromPath( string sourcePath, int width, int height, int? position = null )
    {
        if ( !File.Exists( sourcePath ) )
            throw new FileNotFoundException( "Source image not found", sourcePath );

        // Generate unique name but preserve extension
        var ext = Path.GetExtension( sourcePath );
        var fileName = Path.GetFileName( sourcePath );
        var destRel = Path.Combine( "images", fileName );
        var destFull = Path.Combine( _workingDir, destRel );
        int counter = 1;
        while ( File.Exists( destFull ) )
        {
            fileName = $"{Path.GetFileNameWithoutExtension( sourcePath )}_{counter++}{Path.GetExtension( sourcePath )}";
            destRel = Path.Combine( "images", fileName );
            destFull = Path.Combine( _workingDir, destRel );
        }
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

    public DocumentItem GetItem( int index ) // index 0-based
    {
        if ( index < 0 || index >= _items.Count )
            throw new ArgumentOutOfRangeException( nameof( index ) );
        return _items[ index ];
    }

    public DocumentItem GetItemById( Guid id )
    {
        return _items.FirstOrDefault( i => i.Id == id );
    }

    public void DeleteItemAt( int index )
    {
        if ( index < 0 || index >= _items.Count )
            throw new ArgumentOutOfRangeException( nameof( index ) );
        _items.RemoveAt( index );
    }

    public string GetTitle() => _title;
    public void SetTitle( string title ) => _title = title;

    // History management
    public bool CanUndo() => _history.Count > 0;
    public bool CanRedo() => _undone.Count > 0;

    public void ExecuteCommand( ICommand cmd )
    {
        // If there's undone commands, then per rules we must delete them (and maybe physically delete resources as required).
        if ( _undone.Count > 0 )
        {
            // Remove undone commands in reverse order of undo (stack order)
            while ( _undone.Count > 0 )
            {
                var undoneCmd = _undone.Pop();
                // Deleting an undone command should perform resource-deletion rules:
                // If the undone command was an InsertImage that was undone earlier (so resource marked for deletion at Undo),
                // then deleting that undone command must physically delete the resource.
                if ( undoneCmd is IResourceAffecting ra )
                {
                    ra.OnHistoryRemoved( removedFromUndone: true );
                }
            }
        }

        // Try coalescing with last history entry if possible
        var last = _history.Count > 0 ? _history.Last() : null;
        if ( last != null && last.CanMergeWith( cmd ) )
        {
            last.MergeWith( cmd );
            // merged command replaces last in history (it already modified state via Execute() earlier)
            // But need to actually perform Execute() effect of cmd? For our implementation MergeWith should apply its changes directly to document state.
            // So don't call Execute() here.
            return;
        }

        // Execute command and add to history
        cmd.Execute();
        _history.Add( cmd );

        // Enforce history size limit. If overflow, remove oldest commands (in order) and for each removed executed command run OnHistoryRemoved(false)
        while ( _history.Count > MaxHistory )
        {
            var removed = _history[ 0 ];
            _history.RemoveAt( 0 );
            if ( removed is IResourceAffecting ra )
            {
                ra.OnHistoryRemoved( removedFromUndone: false );
            }
        }
    }

    public void Undo()
    {
        if ( !CanUndo() )
        {
            Console.WriteLine( "Nothing to undo" );
            return;
        }
        var cmd = _history.Last();
        _history.RemoveAt( _history.Count - 1 );
        cmd.Unexecute();
        _undone.Push( cmd );
    }

    public void Redo()
    {
        if ( !CanRedo() )
        {
            Console.WriteLine( "Nothing to redo" );
            return;
        }
        var cmd = _undone.Pop();
        cmd.Execute();
        _history.Add( cmd );

        // if exceeding history capacity, remove oldest
        while ( _history.Count > MaxHistory )
        {
            var removed = _history[ 0 ];
            _history.RemoveAt( 0 );
            if ( removed is IResourceAffecting ra )
            {
                ra.OnHistoryRemoved( removedFromUndone: false );
            }
        }
    }

    // Сохранение в HTML
    public void SaveAsHtml( string htmlPath )
    {
        // теперь htmlPath будет внутри doc_work
        var fullHtmlPath = Path.Combine( _workingDir, Path.GetFileName( htmlPath ) );
        var imagesOutDir = _imagesDir; // images уже внутри doc_work

        // копирование изображений
        foreach ( var item in _items.OfType<ImageItem>() )
        {
            if ( item.IsMarkedDeleted )
                continue;

            var srcFull = Path.Combine( _workingDir, item.GetPath().Replace( '/', Path.DirectorySeparatorChar ) );
            var dstFull = Path.Combine( imagesOutDir, Path.GetFileName( item.GetPath() ) );
            File.Copy( srcFull, dstFull, true );
        }

        // генерируем HTML
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
                    var rel = Path.GetFileName( img.GetPath() );
                    sb.AppendLine( $"<img src=\"images/{HtmlUtil.Escape( rel )}\" width=\"{img.GetWidth()}\" height=\"{img.GetHeight()}\" />" );
                    break;
            }
        }

        sb.AppendLine( "</body></html>" );
        File.WriteAllText( fullHtmlPath, sb.ToString(), Encoding.UTF8 );

        Console.WriteLine( $"Saved HTML to {fullHtmlPath}" );
    }

    // Helpers for commands to access internals
    public string GetWorkingDir() => _workingDir;
    public string GetImagesDir() => _imagesDir;

    // When a command that created a resource needs to physically delete file from working dir:
    public void DeleteResourceFileIfExists( string relativePath )
    {
        var full = Path.Combine( _workingDir, relativePath.Replace( '/', Path.DirectorySeparatorChar ) );
        try
        {
            if ( File.Exists( full ) )
            {
                File.Delete( full );
                // try deleting parent images dir only if empty (not necessary but tidy)
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Warning deleting file {full}: {ex.Message}" );
        }
    }
}
