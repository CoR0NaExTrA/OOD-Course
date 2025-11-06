namespace graphics_lib;
public interface ICanvas
{
    void MoveTo( int x, int y );
    void LineTo( int x, int y );
}

public class CCanvas : ICanvas
{
    public void MoveTo( int x, int y )
    {
        Console.WriteLine( $"MoveTo ({x}, {y})" );
    }

    public void LineTo( int x, int y )
    {
        Console.WriteLine( $"LineTo ({x}, {y})" );
    }
}
