namespace Transform.Tests;

public class StreamDecoratorTests
{
    private static byte[] GenerateTestData( int size = 256 )
    {
        var data = new byte[ size ];
        for ( int i = 0; i < size; i++ )
            data[ i ] = ( byte )( i % 10 + 65 ); // Повторяющийся паттерн для сжатия
        return data;
    }

    private static string CreateTempFile( string prefix, byte[] data = null )
    {
        string path = Path.GetTempFileName();
        if ( data != null )
            File.WriteAllBytes( path, data );
        return path;
    }

    [Fact]
    public void FileInputOutputStream_WriteAndRead_ShouldMatchOriginalData()
    {
        var inputData = GenerateTestData();
        string filePath = CreateTempFile( "raw_test" );

        using ( var output = new Streams.FileOutputStream( filePath ) )
            output.WriteBlock( inputData, inputData.Length );

        using var input = new Streams.FileInputStream( filePath );
        var buffer = new byte[ inputData.Length ];
        int read = input.ReadBlock( buffer, buffer.Length );

        Assert.Equal( inputData.Length, read );
        Assert.Equal( inputData, buffer );
    }

    [Fact]
    public void EncryptDecrypt_ShouldReturnOriginalData()
    {
        var inputData = GenerateTestData();
        const int key = 42;
        string encFile = CreateTempFile( "enc_test" );

        // Шифруем
        using ( var enc = new Decorators.EncryptOutputStreamDecorator( new Streams.FileOutputStream( encFile ), key ) )
            enc.WriteBlock( inputData, inputData.Length );

        // Расшифровываем
        using var dec = new Decorators.DecryptInputStreamDecorator( new Streams.FileInputStream( encFile ), key );
        var result = new byte[ inputData.Length ];
        int read = dec.ReadBlock( result, result.Length );

        Assert.Equal( inputData.Length, read );
        Assert.Equal( inputData, result );
    }

    [Fact]
    public void CompressDecompress_ShouldReturnOriginalData()
    {
        var inputData = GenerateTestData();
        string compFile = CreateTempFile( "comp_test" );

        // Сжимаем
        using ( var comp = new Decorators.CompressOutputStreamDecorator( new Streams.FileOutputStream( compFile ) ) )
            comp.WriteBlock( inputData, inputData.Length );

        // Декомпрессия
        using var decomp = new Decorators.DecompressInputStreamDecorator( new Streams.FileInputStream( compFile ) );
        var result = new byte[ inputData.Length ];
        int read = decomp.ReadBlock( result, result.Length );

        // Может быть меньше (если конец потока)
        var actual = result.Take( read ).ToArray();

        Assert.Equal( inputData, actual );
    }

    [Fact]
    public void EncryptThenCompress_And_DecompressThenDecrypt_ShouldReturnOriginalData()
    {
        var inputData = GenerateTestData();
        const int key = 99;
        string filePath = CreateTempFile( "chain_test" );

        // Сначала шифруем и сжимаем
        using ( var stream = new Decorators.CompressOutputStreamDecorator(
                   new Decorators.EncryptOutputStreamDecorator( new Streams.FileOutputStream( filePath ), key ) ) )
        {
            stream.WriteBlock( inputData, inputData.Length );
        }

        // Потом читаем — сначала дешифруем, потом декомпрессируем
        using var input = new Decorators.DecompressInputStreamDecorator(
                              new Decorators.DecryptInputStreamDecorator( new Streams.FileInputStream( filePath ), key ) );

        var result = new byte[ inputData.Length ];
        int read = input.ReadBlock( result, result.Length );
        var actual = result.Take( read ).ToArray();

        Assert.Equal( inputData, actual );
    }

    [Fact]
    public void DecompressAppliedOverDecrypt_ShouldWorkCorrectly()
    {
        var inputData = GenerateTestData();
        const int key = 123;
        string filePath = CreateTempFile( "mix_test" );

        // ✅ Сначала сжимаем, потом шифруем
        using ( var output = new Decorators.CompressOutputStreamDecorator(
                   new Decorators.EncryptOutputStreamDecorator( new Streams.FileOutputStream( filePath ), key ) ) )
        {
            output.WriteBlock( inputData, inputData.Length );
        }

        // ✅ Потом при чтении — расшифровываем и распаковываем
        using var input = new Decorators.DecompressInputStreamDecorator(
                              new Decorators.DecryptInputStreamDecorator( new Streams.FileInputStream( filePath ), key ) );

        var result = new byte[ inputData.Length ];
        int read = input.ReadBlock( result, result.Length );
        var actual = result.Take( read ).ToArray();

        Assert.Equal( inputData, actual );
    }

    [Fact]
    public void RleCodec_CompressThenDecompress_ShouldBeLossless()
    {
        var data = GenerateTestData();
        var compressed = Utils.RleCodec.Compress( data );
        var decompressed = Utils.RleCodec.Decompress( compressed );

        Assert.Equal( data, decompressed );
    }

    [Fact]
    public void CompressDecompress_WithSmallData_ShouldReturnOriginalData()
    {
        var inputData = new byte[] { 1, 1, 1, 2, 2, 3, 3, 3, 3 }; // Легко сжимаемые данные
        string compFile = CreateTempFile( "comp_small_test" );

        // Сжимаем
        using ( var comp = new Decorators.CompressOutputStreamDecorator( new Streams.FileOutputStream( compFile ) ) )
            comp.WriteBlock( inputData, inputData.Length );

        // Декомпрессия
        using var decomp = new Decorators.DecompressInputStreamDecorator( new Streams.FileInputStream( compFile ) );
        var result = new byte[ inputData.Length ];
        int read = decomp.ReadBlock( result, result.Length );

        Assert.Equal( inputData.Length, read );
        Assert.Equal( inputData, result );
    }

    [Fact]
    public void CompressDecompress_WithRandomData_ShouldReturnOriginalData()
    {
        var rng = new Random( 42 );
        var inputData = new byte[ 1000 ];
        rng.NextBytes( inputData );

        string compFile = CreateTempFile( "comp_random_test" );

        // Сжимаем
        using ( var comp = new Decorators.CompressOutputStreamDecorator( new Streams.FileOutputStream( compFile ) ) )
            comp.WriteBlock( inputData, inputData.Length );

        // Декомпрессия
        using var decomp = new Decorators.DecompressInputStreamDecorator( new Streams.FileInputStream( compFile ) );
        var result = new byte[ inputData.Length ];
        int totalRead = 0;

        // Читаем частями чтобы проверить буферизацию
        while ( totalRead < inputData.Length && !decomp.IsEOF )
        {
            int read = decomp.ReadBlock( result, Math.Min( 100, inputData.Length - totalRead ) );
            if ( read == 0 )
                break;
            totalRead += read;
        }

        Assert.Equal( inputData.Length, totalRead );
        Assert.Equal( inputData, result );
    }
}
