using DocumentEditor.Commands;
using DocumentEditor.Model;
using DocumentEditor.Utils;
using Xunit;

public class TempFolder : IDisposable
{
    public string Path { get; }
    public TempFolder()
    {
        Path = System.IO.Path.Combine( System.IO.Path.GetTempPath(), "doceditor_" + Guid.NewGuid().ToString( "N" ) );
        Directory.CreateDirectory( Path );
    }
    public void Dispose()
    {
        try
        { Directory.Delete( Path, true ); }
        catch { }
    }
}

public class HtmlUtilTests
{
    [Fact]
    public void Escape_ShouldReplaceSpecialCharacters()
    {
        var input = "<div class=\"test\">Tom & Jerry</div>";
        var result = HtmlUtil.Escape( input );
        Assert.Equal( "&lt;div class=&quot;test&quot;&gt;Tom &amp; Jerry&lt;/div&gt;", result );
    }

    [Fact]
    public void Escape_ShouldReturnEmptyString_WhenNull()
    {
        Assert.Equal( "", HtmlUtil.Escape( null ) );
    }

    [Fact]
    public void Escape_ShouldReturnSame_WhenNoSpecialChars()
    {
        Assert.Equal( "Hello", HtmlUtil.Escape( "Hello" ) );
    }
}
public class DocumentItemTests
{
    [Fact]
    public void Paragraph_ShouldStoreAndReturnText()
    {
        var p = new ParagraphItem( "Hello" );
        Assert.Equal( "Hello", p.GetText() );
        p.SetText( "World" );
        Assert.Equal( "World", p.GetText() );
    }

    [Fact]
    public void Image_ShouldStoreDimensionsAndPath()
    {
        var img = new ImageItem( "images/pic.png", 100, 200 );
        Assert.Equal( "images/pic.png", img.GetPath() );
        Assert.Equal( 100, img.GetWidth() );
        Assert.Equal( 200, img.GetHeight() );
    }

    [Fact]
    public void Image_ShouldResizeProperly()
    {
        var img = new ImageItem( "images/pic.png", 100, 200 );
        img.Resize( 50, 60 );
        Assert.Equal( 50, img.GetWidth() );
        Assert.Equal( 60, img.GetHeight() );
    }
}

public class DocumentTests
{
    [Fact]
    public void InsertParagraph_ShouldAddParagraph()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        doc.InsertParagraph( "Hello" );
        Assert.Equal( 1, doc.GetItemsCount() );

        var item = doc.GetItem( 0 );
        Assert.IsType<ParagraphItem>( item );
        Assert.Equal( "Hello", ( ( ParagraphItem )item ).GetText() );
    }

    [Fact]
    public void InsertImageFromPath_ShouldCopyFileAndAddItem()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var srcFile = Path.Combine( tempDir.Path, "src.png" );
        File.WriteAllText( srcFile, "fakeimage" );

        var img = doc.InsertImageFromPath( srcFile, 100, 200 );

        Assert.True( File.Exists( Path.Combine( doc.GetImagesDir(), Path.GetFileName( img.GetPath() ) ) ) );
        Assert.Equal( 100, img.GetWidth() );
        Assert.Equal( 200, img.GetHeight() );
    }

    [Fact]
    public void SaveAsHtml_ShouldCreateHtmlAndCopyImages()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var srcFile = Path.Combine( tempDir.Path, "src.png" );
        File.WriteAllText( srcFile, "fakeimage" );

        doc.SetTitle( "Test Title" );
        doc.InsertParagraph( "Some text" );
        doc.InsertImageFromPath( srcFile, 100, 200 );
        var htmlPath = Path.Combine( tempDir.Path, "output.html" );

        doc.SaveAsHtml( htmlPath );

        Assert.True( File.Exists( htmlPath ) );
        var html = File.ReadAllText( htmlPath );
        Assert.Contains( "Test Title", html );
        Assert.Contains( "<p>Some text</p>", html );
        Assert.Contains( "<img", html );
    }
}

//Тесты для команд
public class InsertParagraphCommandTests
{
    [Fact]
    public void Execute_ShouldAddParagraph()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var cmd = new InsertParagraphCommand( doc, "Hello", null );
        cmd.Execute();

        Assert.Equal( 1, doc.GetItemsCount() );
        Assert.IsType<ParagraphItem>( doc.GetItem( 0 ) );
    }

    [Fact]
    public void Unexecute_ShouldRemoveParagraph()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var cmd = new InsertParagraphCommand( doc, "Hello", null );
        cmd.Execute();
        cmd.Unexecute();

        Assert.Equal( 0, doc.GetItemsCount() );
    }
}

public class InsertImageCommandTests
{
    [Fact]
    public void Execute_ShouldInsertImageAndCopyFile()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var src = Path.Combine( tempDir.Path, "test.png" );
        File.WriteAllText( src, "img" );

        var cmd = new InsertImageCommand( doc, src, 100, 200, null );
        cmd.Execute();

        Assert.Equal( 1, doc.GetItemsCount() );
        var img = Assert.IsType<ImageItem>( doc.GetItem( 0 ) );
        Assert.True( File.Exists( Path.Combine( doc.GetWorkingDir(), img.GetPath().Replace( '/', Path.DirectorySeparatorChar ) ) ) );
    }

    [Fact]
    public void Unexecute_ShouldMarkImageDeleted()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var src = Path.Combine( tempDir.Path, "test.png" );
        File.WriteAllText( src, "img" );

        var cmd = new InsertImageCommand( doc, src, 100, 200, null );
        cmd.Execute();
        var img = ( ImageItem )doc.GetItem( 0 );

        cmd.Unexecute();

        Assert.True( img.IsMarkedDeleted );
    }
}

public class ReplaceTextCommandTests
{
    [Fact]
    public void Execute_ShouldChangeText()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );
        var p = ( ParagraphItem )doc.InsertParagraph( "Old text" );

        var cmd = new ReplaceTextCommand( doc, p, "New text" );
        cmd.Execute();

        Assert.Equal( "New text", p.GetText() );
        cmd.Unexecute();
        Assert.Equal( "Old text", p.GetText() );
    }

    [Fact]
    public void MergeWith_ShouldApplyLatestText()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );
        var p = ( ParagraphItem )doc.InsertParagraph( "Start" );

        var cmd1 = new ReplaceTextCommand( doc, p, "A" );
        cmd1.Execute();
        var cmd2 = new ReplaceTextCommand( doc, p, "B" );

        Assert.True( cmd1.CanMergeWith( cmd2 ) );
        cmd1.MergeWith( cmd2 );

        Assert.Equal( "B", p.GetText() );
    }
}

public class ResizeImageCommandTests
{
    [Fact]
    public void Execute_ShouldResizeImage()
    {
        using var tempDir = new TempFolder();
        var doc = new Document( tempDir.Path );

        var img = new ImageItem( "images/test.png", 100, 200 );
        var cmd = new ResizeImageCommand( doc, img, 50, 60 );
        cmd.Execute();

        Assert.Equal( 50, img.GetWidth() );
        Assert.Equal( 60, img.GetHeight() );
        cmd.Unexecute();
        Assert.Equal( 100, img.GetWidth() );
        Assert.Equal( 200, img.GetHeight() );
    }
}

