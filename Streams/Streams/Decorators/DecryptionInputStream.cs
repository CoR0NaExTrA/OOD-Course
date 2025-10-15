using Transform.Streams;
using Transform.Utils;

namespace Transform.Decorators;
public class DecryptInputStreamDecorator : IInputDataStream
{
    private readonly IInputDataStream _inner;
    private readonly byte[] _reverseTable;

    public DecryptInputStreamDecorator( IInputDataStream inner, int key )
    {
        _inner = inner;
        var table = SubstitutionTableGenerator.GenerateTable( key );
        _reverseTable = SubstitutionTableGenerator.GenerateReverseTable( table );
    }

    public bool IsEOF => _inner.IsEOF;

    public byte ReadByte() => _reverseTable[ _inner.ReadByte() ];

    public int ReadBlock( byte[] dstBuffer, int size )
    {
        int read = _inner.ReadBlock( dstBuffer, size );
        for ( int i = 0; i < read; i++ )
            dstBuffer[ i ] = _reverseTable[ dstBuffer[ i ] ];
        return read;
    }

    public void Dispose() => _inner.Dispose();
}

