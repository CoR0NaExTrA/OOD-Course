namespace Transform.Streams;
    public interface IOutputDataStream : IDisposable
{
    void WriteByte( byte data );
    void WriteBlock( byte[] srcData, int size );
    void Close();
}
