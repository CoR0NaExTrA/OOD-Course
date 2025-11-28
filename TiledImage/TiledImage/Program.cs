using TiledImage.Drawing;

namespace TiledImage;

class Program
{
    static void Main(string[] args)
    {
        Image img = ImageExtensions.LoadImage(
            """
                 CCCC             
                CC  CC   ##    ## 
                CC      ####  ####
                CC  CC   ##    ## 
                 CCCC             
            """ 
        );

        img.PrintTo( Console.Out );

        Image image = new Image( new Size( 60, 25 ) );
        IDrawer drawer = new Drawer();
        drawer.DrawCircle( image, new Point( 20, 4 ), 3, 'O' );
        drawer.FillCircle( image, new Point( 20, 4 ), 2, ' ' ); // hollow head (optional)

        // Eyes
        drawer.DrawLine( image, new Point( 19, 4 ), new Point( 19, 4 ), '.' );
        drawer.DrawLine( image, new Point( 21, 4 ), new Point( 21, 4 ), '.' );

        // Body (vertical spine)
        drawer.DrawLine( image, new Point( 20, 7 ), new Point( 20, 13 ), '|' );

        // Arms
        drawer.DrawLine( image, new Point( 20, 9 ), new Point( 14, 11 ), '/' );
        drawer.DrawLine( image, new Point( 20, 9 ), new Point( 26, 11 ), '\\' );

        // Legs
        drawer.DrawLine( image, new Point( 20, 14 ), new Point( 15, 19 ), '/' );
        drawer.DrawLine( image, new Point( 20, 14 ), new Point( 25, 19 ), '\\' );

        // Hands (tiny circles)
        drawer.FillCircle( image, new Point( 13, 11 ), 1, 'o' );
        drawer.FillCircle( image, new Point( 27, 11 ), 1, 'o' );

        // Feet (tiny circles)
        drawer.FillCircle( image, new Point( 14, 19 ), 1, 'o' );
        drawer.FillCircle( image, new Point( 26, 19 ), 1, 'o' );
        image.PrintTo( Console.Out );
    }
}