using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledImage.Drawing;
public sealed class Drawer : IDrawer
{
    public void DrawLine( Image image, Point from, Point to, char color )
    {
        int x0 = from.X, y0 = from.Y;
        int x1 = to.X, y1 = to.Y;

        int dx = Math.Abs( x1 - x0 );
        int dy = Math.Abs( y1 - y0 );
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while ( true )
        {
            image.SetPixel( new Point( x0, y0 ), color );

            if ( x0 == x1 && y0 == y1 )
                break;

            int e2 = 2 * err;
            if ( e2 > -dy )
            {
                err -= dy;
                x0 += sx;
            }

            if ( e2 < dx )
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public void DrawCircle( Image image, Point center, int radius, char color )
    {
        if ( radius < 0 )
            throw new ArgumentOutOfRangeException( nameof( radius ) );
        if ( radius == 0 )
        {
            image.SetPixel( center, color );
            return;
        }

        int x = 0;
        int y = radius;
        int d = 3 - 2 * radius; // Initial decision parameter

        PlotCirclePoints( image, center, x, y, color );

        while ( y >= x )
        {
            x++;

            if ( d > 0 )
            {
                y--;
                d = d + 4 * ( x - y ) + 10;
            }
            else
            {
                d = d + 4 * x + 6;
            }

            PlotCirclePoints( image, center, x, y, color );
        }
    }

    public void FillCircle( Image image, Point center, int radius, char color )
    {
        if ( radius < 0 )
            throw new ArgumentOutOfRangeException( nameof( radius ) );
        if ( radius == 0 )
        {
            image.SetPixel( center, color );
            return;
        }

        int x = 0;
        int y = radius;
        int d = 3 - 2 * radius;

        DrawSymmetricHorizontalLines( image, center, x, y, color );

        while ( y >= x )
        {
            x++;

            if ( d > 0 )
            {
                y--;
                d = d + 4 * ( x - y ) + 10;
            }
            else
            {
                d = d + 4 * x + 6;
            }

            DrawSymmetricHorizontalLines( image, center, x, y, color );
        }
    }

    private static void PlotCirclePoints( Image image, Point center, int x, int y, char color )
    {
        image.SetPixel( new Point( center.X + x, center.Y + y ), color );
        image.SetPixel( new Point( center.X - x, center.Y + y ), color );
        image.SetPixel( new Point( center.X + x, center.Y - y ), color );
        image.SetPixel( new Point( center.X - x, center.Y - y ), color );
        image.SetPixel( new Point( center.X + y, center.Y + x ), color );
        image.SetPixel( new Point( center.X - y, center.Y + x ), color );
        image.SetPixel( new Point( center.X + y, center.Y - x ), color );
        image.SetPixel( new Point( center.X - y, center.Y - x ), color );
    }

    private static void DrawSymmetricHorizontalLines( Image image, Point center, int x, int y, char color )
    {
        DrawHorizontalLine( image, center.X - x, center.X + x, center.Y + y, color );
        DrawHorizontalLine( image, center.X - x, center.X + x, center.Y - y, color );

        DrawHorizontalLine( image, center.X - y, center.X + y, center.Y + x, color );
        DrawHorizontalLine( image, center.X - y, center.X + y, center.Y - x, color );
    }

    private static void DrawHorizontalLine( Image image, int xFrom, int xTo, int y, char color )
    {
        if ( y < 0 || y >= image.Height )
            return;

        int start = Math.Max( xFrom, 0 );
        int end = Math.Min( xTo, image.Width - 1 );

        for ( int x = start; x <= end; x++ )
            image.SetPixel( new Point( x, y ), color );
    }
}
