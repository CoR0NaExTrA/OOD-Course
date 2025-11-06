using app;
using modern_graphics_lib;

namespace Tests;

public class ModernGraphicsRendererAdapterTests
{
    [Fact]
    public void LineTo_ShouldOutputValidXmlLine()
    {
        using var sw = new StringWriter();
        using ( var renderer = new CModernGraphicsRenderer( sw ) )
        using ( var adapter = new ModernGraphicsRendererAdapter( renderer ) )
        {
            adapter.MoveTo( 10, 20 );
            adapter.LineTo( 30, 40 );
        }

        string output = sw.ToString();
        Assert.Contains( "<draw>", output );
        Assert.Contains( "<line fromX=\"10\"", output );
        Assert.Contains( "toX=\"30\"", output );
        Assert.Contains( "</draw>", output );
    }

    [Fact]
    public void MultipleLines_ShouldAccumulateCorrectly()
    {
        using var sw = new StringWriter();
        using ( var renderer = new CModernGraphicsRenderer( sw ) )
        using ( var adapter = new ModernGraphicsRendererAdapter( renderer ) )
        {
            adapter.MoveTo( 0, 0 );
            adapter.LineTo( 10, 10 );
            adapter.LineTo( 20, 20 );
        }

        string output = sw.ToString();
        Assert.Contains( "<line fromX=\"0\" fromY=\"0\" toX=\"10\" toY=\"10\"/>", output );
        Assert.Contains( "<line fromX=\"10\" fromY=\"10\" toX=\"20\" toY=\"20\"/>", output );
    }
}

public class ModernGraphicsRendererClassAdapterTests
{
    [Fact]
    public void Draw_ShouldProduceValidXml()
    {
        using var sw = new StringWriter();
        using ( var adapter = new ModernGraphicsRendererClassAdapter( sw ) )
        {
            adapter.MoveTo( 10, 20 );
            adapter.LineTo( 30, 40 );
        }

        string output = sw.ToString();
        Assert.Contains( "<draw>", output );
        Assert.Contains( "<line fromX=\"10\" fromY=\"20\" toX=\"30\" toY=\"40\"/>", output );
        Assert.Contains( "</draw>", output );
    }

    [Fact]
    public void MultipleLines_ShouldAccumulate()
    {
        using var sw = new StringWriter();
        using ( var adapter = new ModernGraphicsRendererClassAdapter( sw ) )
        {
            adapter.MoveTo( 0, 0 );
            adapter.LineTo( 50, 50 );
            adapter.LineTo( 100, 100 );
        }

        string output = sw.ToString();
        Assert.Contains( "<line fromX=\"0\" fromY=\"0\" toX=\"50\" toY=\"50\"/>", output );
        Assert.Contains( "<line fromX=\"50\" fromY=\"50\" toX=\"100\" toY=\"100\"/>", output );
    }
}