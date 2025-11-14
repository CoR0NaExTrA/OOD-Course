namespace shape_drawing_lib;

using graphics_lib;

public struct Point
{
    public int x, y;
    public Point( int x, int y ) 
    { 
        this.x = x; 
        this.y = y; 
    }
}

public interface ICanvasDrawable
{
    void Draw( ICanvas canvas );
}

public class CTriangle : ICanvasDrawable
{
    private readonly Point m_p1, m_p2, m_p3;

    public CTriangle( Point p1, Point p2, Point p3 )
    {
        m_p1 = p1;
        m_p2 = p2;
        m_p3 = p3;
    }

    public void Draw( ICanvas canvas )
    {
        canvas.MoveTo( m_p1.x, m_p1.y );
        canvas.LineTo( m_p2.x, m_p2.y );
        canvas.LineTo( m_p3.x, m_p3.y );
        canvas.LineTo( m_p1.x, m_p1.y );
    }
}

public class CRectangle : ICanvasDrawable
{
    private readonly Point m_leftTop;
    private readonly int m_width;
    private readonly int m_height;

    public CRectangle( Point leftTop, int width, int height )
    {
        m_leftTop = leftTop;
        m_width = width;
        m_height = height;
    }

    public void Draw( ICanvas canvas )
    {
        var rightTop = new Point( m_leftTop.x + m_width, m_leftTop.y );
        var rightBottom = new Point( m_leftTop.x + m_width, m_leftTop.y + m_height );
        var leftBottom = new Point( m_leftTop.x, m_leftTop.y + m_height );

        canvas.MoveTo( m_leftTop.x, m_leftTop.y );
        canvas.LineTo( rightTop.x, rightTop.y );
        canvas.LineTo( rightBottom.x, rightBottom.y );
        canvas.LineTo( leftBottom.x, leftBottom.y );
        canvas.LineTo( m_leftTop.x, m_leftTop.y );
    }
}

public class CCanvasPainter
{
    private readonly ICanvas m_canvas;

    public CCanvasPainter( ICanvas canvas )
    {
        m_canvas = canvas;
    }

    public void Draw( ICanvasDrawable drawable )
    {
        drawable.Draw( m_canvas );
    }
}
