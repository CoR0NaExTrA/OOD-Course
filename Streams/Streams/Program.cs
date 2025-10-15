using Transform.Streams;
using Transform.Decorators;

namespace Transform;
class Program
{
    static void Main( string[] args )
    {
        if ( args.Length < 2 )
        {
            Console.WriteLine( "Usage: transform [options] <input> <output>" );
            return;
        }

        var options = new List<string>();
        string inputFile = args[ ^2 ];
        string outputFile = args[ ^1 ];

        for ( int i = 0; i < args.Length - 2; i++ )
            options.Add( args[ i ] );

        using IInputDataStream input = BuildInputPipeline( inputFile, options );
        using IOutputDataStream output = BuildOutputPipeline( outputFile, options );

        TransformData( input, output );
    }

    static IInputDataStream BuildInputPipeline( string path, List<string> options )
    {
        IInputDataStream stream = new FileInputStream( path );

        for ( int i = options.Count - 1; i >= 0; i-- )
        {
            switch ( options[ i ] )
            {
                case "--decompress":
                    stream = new DecompressInputStreamDecorator( stream );
                    break;

                case "--decrypt":
                    int key = int.Parse( options[ i + 1 ] );
                    stream = new DecryptInputStreamDecorator( stream, key );
                    i--;
                    break;
            }
        }

        return stream;
    }

    static IOutputDataStream BuildOutputPipeline( string path, List<string> options )
    {
        IOutputDataStream stream = new FileOutputStream( path );
        for ( int i = 0; i < options.Count; i++ )
        {
            switch ( options[ i ] )
            {
                case "--compress":
                    stream = new CompressOutputStreamDecorator( stream );
                    break;
                case "--encrypt":
                    int key = int.Parse( options[ ++i ] );
                    stream = new EncryptOutputStreamDecorator( stream, key );
                    break;
            }
        }
        return stream;
    }

    static void TransformData( IInputDataStream input, IOutputDataStream output )
    {
        const int BUFFER_SIZE = 4096;
        var buffer = new byte[ BUFFER_SIZE ];
        while ( !input.IsEOF )
        {
            int bytesRead = input.ReadBlock( buffer, BUFFER_SIZE );
            if ( bytesRead > 0 )
                output.WriteBlock( buffer, bytesRead );
        }
        output.Close();
    }
}
