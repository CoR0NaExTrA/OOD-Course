namespace TiledImage.Drawing;

public static class ImageExtensions
{
    public static Image LoadImage( string pixels )
    {
        if ( string.IsNullOrEmpty( pixels ) )
        {
            return new Image( Size.Empty );
        }

        string[] lines = pixels.Replace( "\r\n", "\n" ).Split( '\n' );
        int height = lines.Length;
        int width = 0;

        foreach ( var line in lines )
        {
            if ( line.Length > width )
            {
                width = line.Length;
            }
        }

        Size size = new Size( width, height );
        Image img = new Image( size );

        for ( int y = 0; y < height; y++ )
        {
            string line = lines[ y ];
            for ( int x = 0; x < width; x++ )
            {
                img.SetPixel( new Point( x, y ), x < line.Length ? line[ x ] : ' ' );
            }
        }

        return img;
    }
}