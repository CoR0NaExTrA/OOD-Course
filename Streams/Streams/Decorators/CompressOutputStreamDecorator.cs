using Transform.Streams;
using Transform.Utils;

namespace Transform.Decorators;
public class CompressOutputStreamDecorator : IOutputDataStream
{
    private readonly IOutputDataStream _inner;

    public CompressOutputStreamDecorator( IOutputDataStream inner )
    {
        _inner = inner;
    }

    public void WriteByte( byte data ) => WriteBlock( new[] { data }, 1 );

    public void WriteBlock( byte[] srcData, int size )
    {
        var compressed = RleCodec.Compress( srcData[ ..size ] );
        _inner.WriteBlock( compressed, compressed.Length );
    }

    public void Close() => _inner.Close();

    public void Dispose() => _inner.Dispose();
}

