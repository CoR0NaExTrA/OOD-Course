using Shapes.Interface.gfx;

namespace Shapes.Class.shapes;
public class Picture
{
    private readonly IDictionary<string, Shape> _shapes = new Dictionary<string, Shape>();

    public void AddShape( Shape shape )
    {
        if ( _shapes.ContainsKey( shape.Id ) )
            throw new InvalidOperationException( $"Shape with id {shape.Id} already exists" );

        _shapes[ shape.Id ] = shape;
    }

    public void DeleteShape( string id )
    {
        if ( !_shapes.Remove( id ) )
            throw new KeyNotFoundException( $"Shape {id} not found" );
    }

    public void MovePicture( double dx, double dy )
    {
        foreach ( var shape in _shapes.Values )
            shape.Move( dx, dy );
    }

    public void DrawPicture( ICanvas canvas )
    {
        foreach ( var shape in _shapes.Values )
            shape.Draw( canvas );
    }

    public IEnumerable<Shape> List() => _shapes.Values;
}
