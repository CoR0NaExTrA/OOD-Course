using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Presentation.Core;

public class ImageRepository
{
    private readonly string tempFolder;
    private readonly Dictionary<string, string> idToPath = new();
    private readonly Dictionary<string, byte[]> cache = new();

    public ImageRepository()
    {
        tempFolder = Path.Combine( Path.GetTempPath(), "ShapesEditor_Images" );
        Directory.CreateDirectory( tempFolder );
    }

    public string AddImageFromFile( string filePath )
    {
        var id = Guid.NewGuid().ToString( "N" );
        var ext = Path.GetExtension( filePath ) ?? ".img";
        var dest = Path.Combine( tempFolder, id + ext );
        File.Copy( filePath, dest, true );
        idToPath[ id ] = dest;
        return id;
    }

    public string AddImageBytes( byte[] bytes, string extension = ".img" )
    {
        var id = Guid.NewGuid().ToString( "N" );
        var dest = Path.Combine( tempFolder, id + extension );
        File.WriteAllBytes( dest, bytes );
        idToPath[ id ] = dest;
        return id;
    }

    public byte[] GetImageBytes( string id )
    {
        if ( id == null )
            return null;
        if ( cache.TryGetValue( id, out var b ) )
            return b;
        if ( !idToPath.TryGetValue( id, out var path ) )
            return null;
        try
        {
            var bytes = File.ReadAllBytes( path );
            cache[ id ] = bytes;
            return bytes;
        }
        catch
        {
            return null;
        }
    }

    public string GetImageTempPath( string id )
    {
        if ( id == null )
            return null;
        idToPath.TryGetValue( id, out var p );
        return p;
    }

    public void UnloadFromMemory( string id )
    {
        if ( id == null )
            return;
        cache.Remove( id );
    }

    public void Remove( string id )
    {
        if ( id == null )
            return;
        cache.Remove( id );
        if ( idToPath.TryGetValue( id, out var path ) )
        {
            try
            { if ( File.Exists( path ) ) File.Delete( path ); }
            catch { }
            idToPath.Remove( id );
        }
    }

    public void CleanupUnused( IEnumerable<string> usedIds )
    {
        var usedSet = new HashSet<string>( usedIds ?? Enumerable.Empty<string>() );
        var keys = idToPath.Keys.ToList();
        foreach ( var id in keys )
        {
            if ( !usedSet.Contains( id ) )
            {
                try
                {
                    var p = idToPath[ id ];
                    if ( File.Exists( p ) )
                        File.Delete( p );
                }
                catch { }
                idToPath.Remove( id );
                cache.Remove( id );
            }
        }
        var cacheKeys = cache.Keys.ToList();
        foreach ( var k in cacheKeys )
        {
            if ( !usedSet.Contains( k ) )
                cache.Remove( k );
        }
    }
}
