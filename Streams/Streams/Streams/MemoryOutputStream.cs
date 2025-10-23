namespace Transform.Streams;
public class MemoryOutputStream : IOutputDataStream
{
    private readonly List<byte> _buffer = new();
    private bool _closed = false;

    public IReadOnlyList<byte> Data => _buffer;

    public void WriteByte( byte data )
    {
        if ( _closed )
            throw new InvalidOperationException( "Stream is closed" );
        _buffer.Add( data );
    }

    public void WriteBlock( byte[] srcData, int size )
    {
        if ( _closed )
            throw new InvalidOperationException( "Stream is closed" );
        for ( int i = 0; i < size; i++ )
            _buffer.Add( srcData[ i ] );
    }

    public void Close()
    {
        _closed = true;
    }

    public void Dispose()
    {
    }
}
