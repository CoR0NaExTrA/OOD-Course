namespace Transform.Streams;
public class MemoryInputStream : IInputDataStream
{
    private readonly byte[] _data;
    private int _position = 0;

    public MemoryInputStream( byte[] data )
    {
        _data = data ?? throw new ArgumentNullException( nameof( data ) );
    }

    public bool IsEOF => _position >= _data.Length;

    public byte ReadByte()
    {
        if ( IsEOF )
            throw new InvalidOperationException( "End of stream reached" );
        return _data[ _position++ ];
    }

    public int ReadBlock( byte[] dstBuffer, int size )
    {
        if ( IsEOF )
            return 0;

        int remaining = _data.Length - _position;
        int toRead = Math.Min( size, remaining );
        Array.Copy( _data, _position, dstBuffer, 0, toRead );
        _position += toRead;
        return toRead;
    }

    public void Dispose()
    {
    }
}
