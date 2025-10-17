namespace Transform.Streams;
public interface IInputDataStream : IDisposable
{
    bool IsEOF { get; }
    byte ReadByte();
    int ReadBlock( byte[] dstBuffer, int size );
}