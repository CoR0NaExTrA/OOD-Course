using Transform.Streams;
using Transform.Utils;

namespace Transform.Decorators;
public class EncryptOutputStreamDecorator : IOutputDataStream
{
    private readonly IOutputDataStream _inner;
    private readonly byte[] _table;

    public EncryptOutputStreamDecorator( IOutputDataStream inner, int key )
    {
        _inner = inner;
        _table = SubstitutionTableGenerator.GenerateTable( key );
    }

    public void WriteByte( byte data )
        => _inner.WriteByte( _table[ data ] );

    public void WriteBlock( byte[] srcData, int size )
    {
        var encrypted = new byte[ size ];
        for ( int i = 0; i < size; i++ )
            encrypted[ i ] = _table[ srcData[ i ] ];
        _inner.WriteBlock( encrypted, size );
    }

    public void Close() => _inner.Close();

    public void Dispose() => _inner.Dispose();
}
