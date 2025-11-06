using System.Text;
using System.Text.RegularExpressions;
using DocumentEditor.Model;
using DocumentEditor.Commands;
using DocumentEditor.Image;
using DocumentEditor.Paragraph;

namespace DocumentEditor;
class Program
{
    static void PrintHelp()
    {
        Console.WriteLine( "Доступные команды:" );
        Console.WriteLine( "InsertParagraph <позиция>|end <текст параграфа>" );
        Console.WriteLine( "InsertImage <позиция>|end <ширина> <высота> <путь к файлу изображения>" );
        Console.WriteLine( "SetTitle <заголовок документа>" );
        Console.WriteLine( "List" );
        Console.WriteLine( "ReplaceText <позиция> <текст параграфа>" );
        Console.WriteLine( "ResizeImage <позиция> <ширина> <высота>" );
        Console.WriteLine( "DeleteItem <позиция>" );
        Console.WriteLine( "Help" );
        Console.WriteLine( "Undo" );
        Console.WriteLine( "Redo" );
        Console.WriteLine( "Save <путь к .html>" );
        Console.WriteLine( "Exit" );
    }

    static void Main( string[] args )
    {
        Console.OutputEncoding = Encoding.UTF8;
        var workingDir = Path.Combine( Directory.GetCurrentDirectory(), "doc_work" );
        Directory.CreateDirectory( workingDir );
        var doc = new Document( workingDir );

        Console.WriteLine( "Document editor started. Type 'Help' for commands." );
        while ( true )
        {
            Console.Write( "> " );
            var line = Console.ReadLine();
            if ( line == null )
                break;
            line = line.Trim();
            if ( line.Length == 0 )
                continue;

            try
            {
                var parts = SplitKeepingQuoted( line );
                var cmd = parts[ 0 ].ToLowerInvariant();

                if ( cmd == "help" )
                { 
                    PrintHelp(); 
                    continue; 
                }

                if ( cmd == "exit" )
                {
                    break;
                }

                if ( cmd == "list" )
                {
                    Console.WriteLine( $"Title: {doc.GetTitle()}" );
                    for ( int i = 0; i < doc.GetItemsCount(); i++ )
                    {
                        var it = doc.GetItem( i );
                        if ( it is ParagraphItem p )
                            Console.WriteLine( $"{i + 1}. Paragraph: {p.GetText()}" );
                        else if ( it is ImageItem img )
                            Console.WriteLine( $"{i + 1}. Image: {img.GetWidth()} {img.GetHeight()} {img.GetPath()} {( img.IsMarkedDeleted ? "[marked deleted]" : "" )}" );
                    }
                    continue;
                }

                if ( cmd == "insertparagraph" )
                {
                    if ( parts.Length < 3 )
                    { Console.WriteLine( "Usage: InsertParagraph <position>|end <text>" ); continue; }
                    var posStr = parts[ 1 ];
                    var text = Rejoin( parts, 2 );
                    int? pos = null;
                    if ( !posStr.Equals( "end", StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        if ( !int.TryParse( posStr, out var pos1 ) )
                        { Console.WriteLine( "Invalid position" ); continue; }
                        // user positions are 1-based
                        pos = pos1 - 1;
                        if ( pos < 0 || pos > doc.GetItemsCount() )
                        {
                            Console.WriteLine( "Error: position out of range" );
                            continue;
                        }
                    }
                    var command = new InsertParagraphCommand( doc, text, pos );
                    doc.ExecuteCommand( command );
                    continue;
                }

                if ( cmd == "insertimage" )
                {
                    if ( parts.Length < 5 )
                    { Console.WriteLine( "Usage: InsertImage <position>|end <width> <height> <path>" ); continue; }
                    var posStr = parts[ 1 ];
                    if ( !int.TryParse( parts[ 2 ], out var width ) || !int.TryParse( parts[ 3 ], out var height ) )
                    {
                        Console.WriteLine( "Invalid width/height" );
                        continue;
                    }
                    var pathParts = parts.Skip( 4 ).ToArray();
                    var path = Rejoin( pathParts, 0 );
                    int? pos = null;
                    if ( !posStr.Equals( "end", StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        if ( !int.TryParse( posStr, out var pos1 ) )
                        { Console.WriteLine( "Invalid position" ); continue; }
                        pos = pos1 - 1;
                        if ( pos < 0 || pos > doc.GetItemsCount() )
                        { Console.WriteLine( "Error: position out of range" ); continue; }
                    }
                    try
                    {
                        var cmdIns = new InsertImageCommand( doc, path, width, height, pos );
                        doc.ExecuteCommand( cmdIns );
                    }
                    catch ( ArgumentException ae )
                    {
                        Console.WriteLine( $"Error: {ae.Message}" );
                    }
                    catch ( FileNotFoundException )
                    {
                        Console.WriteLine( "Error: source image file not found" );
                    }
                    continue;
                }

                if ( cmd == "settitle" )
                {
                    if ( parts.Length < 2 )
                    { Console.WriteLine( "Usage: SetTitle <title>" ); continue; }
                    var title = Rejoin( parts, 1 );
                    var cmdSet = new SetTitleCommand( doc, title );
                    doc.ExecuteCommand( cmdSet );
                    continue;
                }

                if ( cmd == "replacetext" )
                {
                    if ( parts.Length < 3 )
                    { Console.WriteLine( "Usage: ReplaceText <position> <text>" ); continue; }
                    if ( !int.TryParse( parts[ 1 ], out var pos1 ) )
                    { Console.WriteLine( "Invalid position" ); continue; }
                    var pos = pos1 - 1;
                    if ( pos < 0 || pos >= doc.GetItemsCount() )
                    { Console.WriteLine( "Error: position out of range" ); continue; }
                    var item = doc.GetItem( pos );
                    if ( item is not ParagraphItem p )
                    { Console.WriteLine( "Error: item at position is not a paragraph" ); continue; }
                    var text = Rejoin( parts, 2 );
                    var cmdRepl = new ReplaceTextCommand( doc, p, text );
                    doc.ExecuteCommand( cmdRepl );
                    continue;
                }

                if ( cmd == "resizeimage" )
                {
                    if ( parts.Length != 4 )
                    { Console.WriteLine( "Usage: ResizeImage <position> <width> <height>" ); continue; }
                    if ( !int.TryParse( parts[ 1 ], out var pos1 ) || !int.TryParse( parts[ 2 ], out var w ) || !int.TryParse( parts[ 3 ], out var h ) )
                    { Console.WriteLine( "Invalid args" ); continue; }
                    var pos = pos1 - 1;
                    if ( pos < 0 || pos >= doc.GetItemsCount() )
                    { Console.WriteLine( "Error: position out of range" ); continue; }
                    var item = doc.GetItem( pos );
                    if ( item is not ImageItem img )
                    { Console.WriteLine( "Error: item at position is not an image" ); continue; }
                    var cmdResize = new ResizeImageCommand( doc, img, w, h );
                    doc.ExecuteCommand( cmdResize );
                    continue;
                }

                if ( cmd == "deleteitem" )
                {
                    if ( parts.Length != 2 )
                    { Console.WriteLine( "Usage: DeleteItem <position>" ); continue; }
                    if ( !int.TryParse( parts[ 1 ], out var pos1 ) )
                    { Console.WriteLine( "Invalid position" ); continue; }
                    var pos = pos1 - 1;
                    if ( pos < 0 || pos >= doc.GetItemsCount() )
                    { Console.WriteLine( "Error: position out of range" ); continue; }
                    var cmdDel = new DeleteItemCommand( doc, pos );
                    doc.ExecuteCommand( cmdDel );
                    continue;
                }

                if ( cmd == "undo" )
                {
                    doc.Undo();
                    continue;
                }

                if ( cmd == "redo" )
                {
                    doc.Redo();
                    continue;
                }

                if ( cmd == "save" )
                {
                    if ( parts.Length != 2 )
                    { Console.WriteLine( "Usage: Save <path.html>" ); continue; }
                    var path = parts[ 1 ];
                    doc.SaveAsHtml( path );
                    continue;
                }

                Console.WriteLine( "Unknown command. Type Help." );
            }

            catch ( ArgumentOutOfRangeException )
            {
                Console.WriteLine( "Error: position out of range" );
            }

            catch ( Exception ex )
            {
                Console.WriteLine( $"Error: {ex.Message}" );
            }
        }
    }

    // Helper: split like shell but keep quoted strings together
    static string[] SplitKeepingQuoted( string input )
    {
        var tokens = new List<string>();
        var rx = new Regex( @"[\""].+?[\""]|[^ ]+" );
        foreach ( Match m in rx.Matches( input ) )
        {
            var v = m.Value;
            if ( v.StartsWith( "\"" ) && v.EndsWith( "\"" ) && v.Length >= 2 )
                v = v.Substring( 1, v.Length - 2 );
            tokens.Add( v );
        }
        return tokens.ToArray();
    }

    static string Rejoin( string[] parts, int start )
    {
        return string.Join( ' ', parts.Skip( start ) );
    }
}
