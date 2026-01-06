using SkiaSharp;
using System;

namespace Presentation.Shapes;

public enum ShapeType { Rectangle, Ellipse, Triangle }

public abstract class Shape
{
    public SKRect Bounds { get; set; }
    public ShapeType Type { get; protected set; }

    protected Shape( ShapeType type, SKRect bounds )
    {
        Type = type;
        Bounds = bounds;
    }

    public abstract void Draw( SKCanvas c );
    public abstract bool HitTest( SKPoint p );
    public virtual void DrawSelection( SKCanvas c )
    {
        var r = Bounds;
        using ( var paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 2, IsAntialias = true } )
        {
            paint.Color = SKColors.Blue;
            c.DrawRect( r, paint );
        }

        float handleSize = 8;
        foreach ( var pt in GetHandleCenters() )
        {
            var rect = SKRect.Create( pt.X - handleSize / 2, pt.Y - handleSize / 2, handleSize, handleSize );
            using ( var p = new SKPaint { Style = SKPaintStyle.Fill } )
            {
                p.Color = SKColors.White;
                c.DrawRect( rect, p );
            }
            using ( var p = new SKPaint { Style = SKPaintStyle.Stroke } )
            {
                p.Color = SKColors.Black;
                p.StrokeWidth = 1;
                c.DrawRect( rect, p );
            }
        }
    }

    public virtual SKPoint[] GetHandleCenters()
    {
        var r = Bounds;
        var cx = ( r.Left + r.Right ) / 2;
        var cy = ( r.Top + r.Bottom ) / 2;
        return
        [
            new SKPoint(r.Left, r.Top),
            new SKPoint(cx, r.Top),
            new SKPoint(r.Right, r.Top),
            new SKPoint(r.Right, cy),
            new SKPoint(r.Right, r.Bottom),
            new SKPoint(cx, r.Bottom),
            new SKPoint(r.Left, r.Bottom),
            new SKPoint(r.Left, cy)
        ];
    }

    public virtual void MoveBy( float dx, float dy, SKRect canvasBounds )
    {
        var nb = Bounds;
        nb.Offset( dx, dy );

        if ( nb.Left < 0 )
            nb.Offset( -nb.Left, 0 );
        if ( nb.Top < 0 )
            nb.Offset( 0, -nb.Top );
        if ( nb.Right > canvasBounds.Width )
            nb.Offset( canvasBounds.Width - nb.Right, 0 );
        if ( nb.Bottom > canvasBounds.Height )
            nb.Offset( 0, canvasBounds.Height - nb.Bottom );

        Bounds = nb;
    }

    public virtual void ResizeFromHandle( int handleIndex, SKPoint mousePos, SKRect canvasBounds )
    {
        var r = Bounds;

        float left = r.Left, top = r.Top, right = r.Right, bottom = r.Bottom;

        switch ( handleIndex )
        {
            case 0:
                left = Math.Max(0, mousePos.X);
                top = Math.Max(0, mousePos.Y);
                break;
            case 1:
                top = Math.Max( 0, mousePos.Y );
                break;
            case 2: 
                right = Math.Min( canvasBounds.Width, mousePos.X );
                top = Math.Max( 0, mousePos.Y );
                break;
            case 3:
                right = Math.Min( canvasBounds.Width, mousePos.X );
                break;
            case 4:
                right = Math.Min( canvasBounds.Width, mousePos.X );
                bottom = Math.Min( canvasBounds.Height, mousePos.Y );
                break;
            case 5:
                bottom = Math.Min( canvasBounds.Height, mousePos.Y );
                break;
            case 6:
                left = Math.Max( 0, mousePos.X );
                bottom = Math.Min( canvasBounds.Height, mousePos.Y );
                break;
            case 7:
                left = Math.Max( 0, mousePos.X );
                break;
        }

        const float minSize = 10;
        if ( right - left < minSize )
            right = left + minSize;
        if ( bottom - top < minSize )
            bottom = top + minSize;

        var nb = SKRect.Create( left, top, right - left, bottom - top );

        if ( nb.Left < 0 )
            nb.Offset( -nb.Left, 0 );
        if ( nb.Top < 0 )
            nb.Offset( 0, -nb.Top );
        if ( nb.Right > canvasBounds.Width )
            nb.Offset( canvasBounds.Width - nb.Right, 0 );
        if ( nb.Bottom > canvasBounds.Height )
            nb.Offset( 0, canvasBounds.Height - nb.Bottom );

        Bounds = nb;
    }
}
