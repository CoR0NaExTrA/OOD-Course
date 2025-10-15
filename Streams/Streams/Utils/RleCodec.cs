namespace Transform.Utils;
public static class RleCodec
{
    public static byte[] Compress( byte[] data )
    {
        var result = new List<byte>();
        for ( int i = 0; i < data.Length; )
        {
            byte value = data[ i ];
            int count = 1;
            while ( i + count < data.Length && data[ i + count ] == value && count < 255 )
                count++;
            result.Add( ( byte )count );
            result.Add( value );
            i += count;
        }
        return result.ToArray();
    }

    public static byte[] Decompress( byte[] data )
    {
        var result = new List<byte>();
        for ( int i = 0; i < data.Length; i += 2 )
        {
            byte count = data[ i ];
            byte value = data[ i + 1 ];
            for ( int j = 0; j < count; j++ )
                result.Add( value );
        }
        return result.ToArray();
    }
}