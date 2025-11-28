using TiledImage.Drawing;

public class TileTests
{
    [Fact]
    public void Tile_SetPixel_ChangesValue()
    {
        var tile = new Tile( ' ' );

        tile.SetPixel( new Point( 3, 5 ), 'X' );
        char actual = tile.GetPixel( new Point( 3, 5 ) );

        Assert.Equal( 'X', actual );

        tile.Dispose();
    }

    [Fact]
    public void Tile_GetPixel_OutOfBounds_ReturnsSpace()
    {
        var tile = new Tile( '.' );

        Assert.Equal( ' ', tile.GetPixel( new Point( -1, 3 ) ) );
        Assert.Equal( ' ', tile.GetPixel( new Point( 20, 20 ) ) );

        tile.Dispose();
    }

    [Fact]
    public void Tile_Clone_CreatesIndependentCopy()
    {
        var tile = new Tile( '#' );

        tile.SetPixel( new Point( 2, 2 ), 'A' );
        var clone = ( Tile )tile.Clone();

        clone.SetPixel( new Point( 2, 2 ), 'B' );

        Assert.Equal( 'A', tile.GetPixel( new Point( 2, 2 ) ) );
        Assert.Equal( 'B', clone.GetPixel( new Point( 2, 2 ) ) );

        tile.Dispose();
        clone.Dispose();
    }
}

public class ImageTests
{
    [Fact]
    public void Image_GetPixel_ReturnsCorrectValue()
    {
        Image img = new Image( new Size( 10, 10 ), 'X' );

        Assert.Equal( 'X', img.GetPixel( new Point( 3, 3 ) ) );
    }

    [Fact]
    public void Image_SetPixel_UpdatesOnlyOnePixel()
    {
        Image img = new Image( new Size( 10, 10 ), ' ' );

        img.SetPixel( new Point( 5, 5 ), '@' );

        Assert.Equal( '@', img.GetPixel( new Point( 5, 5 ) ) );
        Assert.Equal( ' ', img.GetPixel( new Point( 0, 0 ) ) );
    }

    [Fact]
    public void Image_CoW_TilesBecomeIndependentAfterModification()
    {
        Image img = new Image( new Size( 10, 10 ), 'C' );

        var p1 = new Point( 1, 1 );
        var p2 = new Point( 9, 9 );

        char old1 = img.GetPixel( p1 );
        char old2 = img.GetPixel( p2 );

        Assert.Equal( 'C', old1 );
        Assert.Equal( 'C', old2 );

        img.SetPixel( p1, 'X' );
        img.SetPixel( p2, 'Y' );

        Assert.Equal( 'X', img.GetPixel( p1 ) );
        Assert.Equal( 'Y', img.GetPixel( p2 ) );
    }

    [Fact]
    public void Image_PrintTo_PrintsCorrectly()
    {
        Image img = new Image( new Size( 4, 2 ), 'A' );

        using var sw = new StringWriter();
        img.PrintTo( sw );

        string expected =
            "AAAA\r\n" +
            "AAAA\r\n";

        Assert.Equal( expected, sw.ToString() );
    }
}
