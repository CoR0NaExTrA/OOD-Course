using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledImage.CoW;

namespace TiledImage;
public sealed class Image
{
    public Size TileSize { get; }

    public int Width { get; }

    public int Height { get; }

    private readonly ITile[,] _tiles;

    public Image( Size size, char color = ' ' )
    {
        Width = size.Width;
        Height = size.Height;
        Size tileSize = new Size(
            CheckAndDivide( size.Width, "Width" ),
            CheckAndDivide( size.Height, "Height" ) );

        if ( tileSize.Width <= 0 || tileSize.Height <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( tileSize ), "Image dimensions must be positive" );
        }

        TileSize = tileSize;
        _tiles = new ITile[ tileSize.Width, tileSize.Height ];

        ITile proxy = new CowTileProxy( color );
        for ( int y = 0; y < tileSize.Height; y++ )
        {
            for ( int x = 0; x < tileSize.Width; x++ )
            {
                _tiles[ x, y ] = ( proxy.Clone() as ITile )!;
            }
        }
    }

    private static int CheckAndDivide( int pixels, string name )
    {
        if ( pixels <= 0 )
        {
            throw new ArgumentOutOfRangeException( name, $"{name} must be positive" );
        }

        return pixels / ITile.SIZE + ( pixels % ITile.SIZE == 0 ? 0 : 1 );
    }

    public char GetPixel( Point p )
    {
        var (tile, local) = PixelToTile( p );
        return tile?.GetPixel( local ) ?? ' ';
    }

    public void SetPixel( Point p, char color )
    {
        var (tile, local) = PixelToTile( p );
        tile?.SetPixel( local, color );
    }

    internal (ITile? tile, Point local) PixelToTile( Point p )
    {
        if ( p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Height )
        {
            return (null, default);
        }

        int tx = p.X / ITile.SIZE;
        int ty = p.Y / ITile.SIZE;
        int lx = p.X % ITile.SIZE;
        int ly = p.Y % ITile.SIZE;

        return (_tiles[ tx, ty ], new Point( lx, ly ));
    }

    public void PrintTo( TextWriter writer )
    {
        for ( int py = 0; py < Height; py++ )
        {
            for ( int px = 0; px < Width; px++ )
            {
                writer.Write( GetPixel( new Point( px, py ) ) );
            }

            writer.WriteLine();
        }
    }

    public override string ToString()
    {
        using var sw = new StringWriter();
        PrintTo( sw );
        return sw.ToString();
    }
}
