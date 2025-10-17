namespace Transform.Streams;
public class FileInputStream : IInputDataStream
{
    private readonly FileStream _stream;

    public FileInputStream( string path )
    {
        _stream = new FileStream( path, FileMode.Open, FileAccess.Read );
    }

    public bool IsEOF => _stream.Position >= _stream.Length;

    public byte ReadByte()
    {
        int value = _stream.ReadByte();
        if ( value == -1 )
            throw new EndOfStreamException();
        return ( byte )value;
    }

    public int ReadBlock( byte[] dstBuffer, int size )
    {
        return _stream.Read( dstBuffer, 0, size );
    }
    public void Dispose() => _stream.Dispose();
}