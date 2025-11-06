namespace app;

using graphics_lib;
using modern_graphics_lib;
using shape_drawing_lib;

// 🔹 Адаптер объекта: позволяет использовать ModernGraphicsRenderer как ICanvas
public class ModernGraphicsRendererAdapter : ICanvas, IDisposable
{
    private readonly CModernGraphicsRenderer m_renderer;
    private CPoint m_currentPoint = new CPoint( 0, 0 );
    private bool m_drawingStarted = false;

    public ModernGraphicsRendererAdapter( CModernGraphicsRenderer renderer )
    {
        m_renderer = renderer ?? throw new ArgumentNullException( nameof( renderer ) );
        m_renderer.BeginDraw();
        m_drawingStarted = true;
    }

    public void MoveTo( int x, int y )
    {
        m_currentPoint = new CPoint( x, y );
    }

    public void LineTo( int x, int y )
    {
        var newPoint = new CPoint( x, y );
        m_renderer.DrawLine( m_currentPoint, newPoint );
        m_currentPoint = newPoint;
    }

    public void Dispose()
    {
        if ( m_drawingStarted )
        {
            m_renderer.EndDraw();
            m_drawingStarted = false;
        }
    }
}

public class ModernGraphicsRendererClassAdapter : CModernGraphicsRenderer, ICanvas, IDisposable
{
    private CPoint m_currentPoint = new CPoint( 0, 0 );
    private bool m_drawingStarted = false;

    public ModernGraphicsRendererClassAdapter( System.IO.TextWriter output )
        : base( output )
    {
        BeginDraw();
        m_drawingStarted = true;
    }

    public void MoveTo( int x, int y )
    {
        m_currentPoint = new CPoint( x, y );
    }

    public void LineTo( int x, int y )
    {
        var newPoint = new CPoint( x, y );
        DrawLine( m_currentPoint, newPoint );
        m_currentPoint = newPoint;
    }

    public void Dispose()
    {
        if ( m_drawingStarted )
        {
            EndDraw();
            m_drawingStarted = false;
        }
    }
}

public static class ProgramApp
{
    public static void PaintPicture( CCanvasPainter painter )
    {
        var triangle = new CTriangle( new Point( 10, 15 ), new Point( 100, 200 ), new Point( 150, 250 ) );
        var rectangle = new CRectangle( new Point( 30, 40 ), 18, 24 );

        painter.Draw( triangle );
        painter.Draw( rectangle );
    }

    public static void PaintPictureOnCanvas()
    {
        var simpleCanvas = new CCanvas();
        var painter = new CCanvasPainter( simpleCanvas );
        PaintPicture( painter );
    }

    public static void PaintPictureOnModernGraphicsRenderer()
    {
        using var renderer = new CModernGraphicsRenderer( Console.Out );
        using var adapter = new ModernGraphicsRendererAdapter( renderer );
        var painter = new CCanvasPainter( adapter );
        PaintPicture( painter );
    }

    public static void PaintPictureOnModernGraphicsRendererClassAdapter()
    {
        using var adapter = new ModernGraphicsRendererClassAdapter( Console.Out );
        var painter = new CCanvasPainter( adapter );
        PaintPicture( painter );
    }
}

class Program
{
    static void Main()
    {
        Console.Write( "Should we use new API (y)? " );
        string userInput = Console.ReadLine();

        if ( userInput?.ToLower() == "y" )
        {
            Console.Write( "Use class adapter (y)? " );
            var input2 = Console.ReadLine();
            if ( input2?.ToLower() == "y" )
                ProgramApp.PaintPictureOnModernGraphicsRendererClassAdapter();
            else
                ProgramApp.PaintPictureOnModernGraphicsRenderer();
        }
        else
        {
            ProgramApp.PaintPictureOnCanvas();
        }
    }
}
