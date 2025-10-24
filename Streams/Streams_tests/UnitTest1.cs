using System;
using System.IO;
using System.Linq;
using Xunit;
using Transform.Streams;
using Transform.Decorators;
using Transform.Utils;

namespace Transform.Tests;

public class StreamDecoratorTests
{
    private static byte[] GenerateTestData( int size = 256 )
    {
        var data = new byte[ size ];
        for ( int i = 0; i < size; i++ )
            data[ i ] = ( byte )( i % 10 + 65 );
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

        using ( var output = new FileOutputStream( filePath ) )
            output.WriteBlock( inputData, inputData.Length );

        using var input = new FileInputStream( filePath );
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

        using ( var enc = new EncryptOutputStreamDecorator( new FileOutputStream( encFile ), key ) )
            enc.WriteBlock( inputData, inputData.Length );

        using var dec = new DecryptInputStreamDecorator( new FileInputStream( encFile ), key );
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

        using ( var comp = new CompressOutputStreamDecorator( new FileOutputStream( compFile ) ) )
            comp.WriteBlock( inputData, inputData.Length );

        using var decomp = new DecompressInputStreamDecorator( new FileInputStream( compFile ) );
        var result = new byte[ inputData.Length ];
        int read = decomp.ReadBlock( result, result.Length );

        var actual = result.Take( read ).ToArray();

        Assert.Equal( inputData, actual );
    }

    [Fact]
    public void EncryptThenCompress_And_DecompressThenDecrypt_ShouldReturnOriginalData()
    {
        var inputData = GenerateTestData();
        const int key = 99;
        string filePath = CreateTempFile( "chain_test" );

        using ( var stream = new CompressOutputStreamDecorator(
                   new EncryptOutputStreamDecorator( new FileOutputStream( filePath ), key ) ) )
        {
            stream.WriteBlock( inputData, inputData.Length );
        }

        using var input = new DecompressInputStreamDecorator(
                              new DecryptInputStreamDecorator( new FileInputStream( filePath ), key ) );

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

        using ( var output = new CompressOutputStreamDecorator(
                   new EncryptOutputStreamDecorator( new FileOutputStream( filePath ) , key ) ) )
        {
            output.WriteBlock( inputData, inputData.Length );
        }

        using var input = new DecompressInputStreamDecorator(
                              new DecryptInputStreamDecorator( new FileInputStream( filePath ), key ) );

        var result = new byte[ inputData.Length ];
        int read = input.ReadBlock( result, result.Length );
        var actual = result.Take( read ).ToArray();

        Assert.Equal( inputData, actual );
    }

    [Fact]
    public void RleCodec_CompressThenDecompress_ShouldBeLossless()
    {
        var data = GenerateTestData();
        var compressed = RleCodec.Compress( data );
        var decompressed = RleCodec.Decompress( compressed );

        Assert.Equal( data, decompressed );
    }

    [Fact]
    public void RleCodec_Decompress_ShouldThrow_OnOddLengthData()
    {
        var invalid = new byte[] { 3, 65, 2 };
        Assert.Throws<ArgumentException>( () => RleCodec.Decompress( invalid ) );
    }

    [Fact]
    public void MemoryStreams_ShouldWriteAndReadCorrectly()
    {
        var data = GenerateTestData( 100 );
        using var memOut = new MemoryOutputStream();
        memOut.WriteBlock( data, data.Length );
        memOut.Close();

        using var memIn = new MemoryInputStream( memOut.Data.ToArray() );
        var buffer = new byte[ data.Length ];
        int read = memIn.ReadBlock( buffer, buffer.Length );

        Assert.Equal( data, buffer );
    }

    [Fact]
    public void DecryptWithWrongKey_ShouldFail()
    {
        var inputData = GenerateTestData();
        string file = CreateTempFile( "wrongkey_test" );

        using ( var enc = new EncryptOutputStreamDecorator( new FileOutputStream( file ), 111 ) )
            enc.WriteBlock( inputData, inputData.Length );

        using var dec = new DecryptInputStreamDecorator( new FileInputStream( file ), 222 );
        var result = new byte[ inputData.Length ];
        dec.ReadBlock( result, result.Length );

        Assert.NotEqual( inputData, result );
    }
}
