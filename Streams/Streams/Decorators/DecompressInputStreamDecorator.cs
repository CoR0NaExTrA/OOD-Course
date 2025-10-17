using Transform.Streams;
using Transform.Utils;

namespace Transform.Decorators;
public class DecompressInputStreamDecorator : IInputDataStream
{
    private readonly IInputDataStream _inner;
    private byte[] _buffer = Array.Empty<byte>();
    private int _position = 0;

    public DecompressInputStreamDecorator( IInputDataStream inner )
    {
        _inner = inner;
    }

    public bool IsEOF => _position >= _buffer.Length && _inner.IsEOF;

    public byte ReadByte()
    {
        if ( _position >= _buffer.Length )
            LoadNextChunk();
        return _buffer[ _position++ ];
    }

    private void LoadNextChunk()
    {
        var temp = new List<byte>();
        var chunk = new byte[ 4096 ];
        int read = _inner.ReadBlock( chunk, chunk.Length );
        if ( read == 0 )
        {
            _buffer = Array.Empty<byte>();
            return;
        }
        var compressed = new byte[ read ];
        Array.Copy( chunk, compressed, read );
        _buffer = RleCodec.Decompress( compressed );
        _position = 0;
    }

    public int ReadBlock( byte[] dstBuffer, int size )
    {
        int count = 0;
        while ( count < size && !IsEOF )
            dstBuffer[ count++ ] = ReadByte();
        return count;
    }

    public void Dispose() => _inner.Dispose();
}
