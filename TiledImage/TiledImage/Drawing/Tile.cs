using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledImage.Drawing;
public class Tile : ITile
{
    public const int SIZE = 8;
    private static int _instanceCount = 0;
    public static int InstanceCount => _instanceCount;

    private readonly char[] _pixels = new char[ SIZE * SIZE ];

    public Tile( char color = ' ' )
    {
        for ( int i = 0; i < _pixels.Length; i++ )
        {
            _pixels[ i ] = color;
        }

        Interlocked.Increment(ref _instanceCount);
    }

    public Tile( char[] pixels )
    {
        _pixels = pixels;
    }

    public void Dispose()
    {
        Interlocked.Decrement(ref _instanceCount);
    }

    public void SetPixel( Point p, char color )
    {
        if ( p.X >= 0 && p.Y >= 0 && p.X < SIZE && p.Y < SIZE )
        {
            _pixels[ p.Y * SIZE + p.X ] = color;
        }
    }

    public char GetPixel( Point p )
    {
        if ( p.X >= 0 && p.Y >= 0 && p.X < SIZE && p.Y < SIZE )
        {
            return _pixels[ p.Y * SIZE + p.X ];
        }
        return ' ';
    }

    public object Clone()
    {
        char[] pixels = new char[ SIZE * SIZE ];
        _pixels.CopyTo( pixels, 0 );
        return new Tile( pixels );
    }
}
