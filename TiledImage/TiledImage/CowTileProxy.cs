using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledImage;
using CowTile = CoW.CoW<Tile>;
public sealed class CowTileProxy : ITile
{
    private CowTile _realTile;
    public int CowInstanceCount => _realTile.RefCount;

    public CowTileProxy( char fillChar = ' ' )
    {
        _realTile = new CowTile( new Tile( fillChar ) );
    }

    private CowTileProxy( CowTileProxy thisTile )
    {
        _realTile = new CowTile( thisTile._realTile );
    }

    public void Dispose()
    {
    }

    public char GetPixel( Point p )
    {
        return _realTile!.Value.GetPixel( p );
    }

    public void SetPixel( Point p, char color )
    {
        _realTile!.Modify( t => t.SetPixel( p, color ) );
    }

    public int RealTileCount => Tile.InstanceCount;

    public object Clone()
    {
        return new CowTileProxy( this );
    }
}
