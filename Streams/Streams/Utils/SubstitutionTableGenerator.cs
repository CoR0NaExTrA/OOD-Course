namespace Transform.Utils;
public static class SubstitutionTableGenerator
{
    public static byte[] GenerateTable( int key )
    {
        var table = Enumerable.Range( 0, 256 ).Select( i => ( byte )i ).ToArray();
        var rng = new Random( key );
        for ( int i = 255; i > 0; i-- )
        {
            int j = rng.Next( i + 1 );
            (table[ i ], table[ j ]) = (table[ j ], table[ i ]);
        }
        return table;
    }

    public static byte[] GenerateReverseTable( byte[] table )
    {
        var reverse = new byte[ 256 ];
        for ( int i = 0; i < 256; i++ )
            reverse[ table[ i ] ] = ( byte )i;
        return reverse;
    }
}
