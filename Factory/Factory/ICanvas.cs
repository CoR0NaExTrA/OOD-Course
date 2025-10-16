namespace Factory;
public interface ICanvas : IDisposable
{
    void SetColor( Color c );
    void DrawLine( Point from, Point to );
    void DrawEllipse( double left, double top, double width, double height );
}
