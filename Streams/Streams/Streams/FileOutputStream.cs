namespace Transform.Streams;
public class FileOutputStream : IOutputDataStream
{
    private readonly FileStream _stream;
    private bool _closed = false;

    public FileOutputStream( string path )
    {
        _stream = new FileStream( path, FileMode.Create, FileAccess.Write );
    }

    public void WriteByte( byte data )
    {
        if ( _closed )
            throw new InvalidOperationException( "Stream closed" );
        _stream.WriteByte( data );
    }

    public void WriteBlock( byte[] srcData, int size )
    {
        if ( _closed )
            throw new InvalidOperationException( "Stream closed" );
        _stream.Write( srcData, 0, size );
    }

    public void Close()
    {
        _closed = true;
        _stream.Close();
    }
    public void Dispose() => _stream.Dispose();
}