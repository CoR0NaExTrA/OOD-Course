using Transform.Streams;
using Transform.Utils;

namespace Transform.Decorators;
public class DecompressInputStreamDecorator : IInputDataStream
{
    private readonly IInputDataStream _inner;
    private readonly List<byte> _decompressedBuffer = new List<byte>();
    private int _bufferPosition = 0;
    private bool _isFullyRead = false;
    private static int _instanceCounter = 0;
    private readonly int _instanceId;

    public DecompressInputStreamDecorator( IInputDataStream inner )
    {
        _inner = inner;
        _instanceId = Interlocked.Increment( ref _instanceCounter );
        Logger.Log( $"DecompressInputStreamDecorator #{_instanceId} created" );
    }

    public bool IsEOF
    {
        get
        {
            bool isEof = _bufferPosition >= _decompressedBuffer.Count && _isFullyRead;
            Logger.Log( $"Decompress #{_instanceId} - IsEOF called, returning: {isEof}" );
            return isEof;
        }
    }

    public byte ReadByte()
    {
        Logger.Log( $"Decompress #{_instanceId} - ReadByte called, buffer: {_decompressedBuffer.Count}, pos: {_bufferPosition}" );

        if ( _bufferPosition >= _decompressedBuffer.Count && !_isFullyRead )
        {
            Logger.Log( $"Decompress #{_instanceId} - Loading next chunk" );
            LoadNextChunk();
        }

        if ( _bufferPosition < _decompressedBuffer.Count )
        {
            byte result = _decompressedBuffer[ _bufferPosition++ ];
            Logger.Log( $"Decompress #{_instanceId} - ReadByte returned: {result}" );
            return result;
        }

        Logger.Log( $"Decompress #{_instanceId} - End of stream reached in ReadByte" );
        throw new EndOfStreamException();
    }

    private void LoadNextChunk()
    {
        Logger.Log( $"Decompress #{_instanceId} - LoadNextChunk started" );

        var rawBuffer = new byte[ 4096 ];
        int bytesRead = _inner.ReadBlock( rawBuffer, rawBuffer.Length );
        Logger.Log( $"Decompress #{_instanceId} - Read {bytesRead} bytes from inner stream" );

        if ( bytesRead > 0 )
        {
            var compressedData = new byte[ bytesRead ];
            Array.Copy( rawBuffer, compressedData, bytesRead );

            Logger.Log( $"Decompress #{_instanceId} - Compressed data (first 10 bytes): {BitConverter.ToString( compressedData.Take( 10 ).ToArray() )}" );

            var decompressedChunk = RleCodec.Decompress( compressedData );
            Logger.Log( $"Decompress #{_instanceId} - Decompressed to {decompressedChunk.Length} bytes" );

            _decompressedBuffer.AddRange( decompressedChunk );
        }
        else
        {
            Logger.Log( $"Decompress #{_instanceId} - No more data from inner stream" );
            _isFullyRead = true;
        }
    }

    public int ReadBlock( byte[] dstBuffer, int size )
    {
        Logger.Log( $"Decompress #{_instanceId} - ReadBlock called, size: {size}, buffer: {_decompressedBuffer.Count}, pos: {_bufferPosition}" );

        int totalRead = 0;

        while ( totalRead < size && !IsEOF )
        {
            if ( _bufferPosition >= _decompressedBuffer.Count && !_isFullyRead )
            {
                Logger.Log( $"Decompress #{_instanceId} - Loading chunk for ReadBlock" );
                LoadNextChunk();
            }

            int bytesToCopy = Math.Min(
                size - totalRead,
                _decompressedBuffer.Count - _bufferPosition
            );

            if ( bytesToCopy > 0 )
            {
                for ( int i = 0; i < bytesToCopy; i++ )
                {
                    dstBuffer[ totalRead + i ] = _decompressedBuffer[ _bufferPosition + i ];
                }
                _bufferPosition += bytesToCopy;
                totalRead += bytesToCopy;
                Logger.Log( $"Decompress #{_instanceId} - Copied {bytesToCopy} bytes in ReadBlock, total: {totalRead}" );
            }
            else
            {
                break;
            }
        }

        Logger.Log( $"Decompress #{_instanceId} - ReadBlock returning: {totalRead}" );
        return totalRead;
    }

    public void Dispose()
    {
        Logger.Log( $"Decompress #{_instanceId} - Dispose called" );
        _inner.Dispose();
    }
}