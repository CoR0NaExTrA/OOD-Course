namespace modern_graphics_lib;

public class CPoint
{
    public int x, y;
    public CPoint( int x, int y )
    {
        this.x = x;
        this.y = y;
    }
}

public class CModernGraphicsRenderer : IDisposable
{
    private readonly TextWriter m_out;
    private bool m_drawing = false;

    public CModernGraphicsRenderer( TextWriter strm )
    {
        m_out = strm;
    }

    public void BeginDraw()
    {
        if ( m_drawing )
            throw new InvalidOperationException( "Drawing has already begun" );
        m_out.WriteLine( "<draw>" );
        m_drawing = true;
    }

    public void DrawLine( CPoint start, CPoint end )
    {
        if ( !m_drawing )
            throw new InvalidOperationException( "DrawLine is allowed between BeginDraw()/EndDraw() only" );

        m_out.WriteLine( $"  <line fromX=\"{start.x}\" fromY=\"{start.y}\" toX=\"{end.x}\" toY=\"{end.y}\"/>" );
    }

    public void EndDraw()
    {
        if ( !m_drawing )
            throw new InvalidOperationException( "Drawing has not been started" );
        m_out.WriteLine( "</draw>" );
        m_drawing = false;
    }

    public void Dispose()
    {
        if ( m_drawing )
        {
            EndDraw();
        }
    }
}
