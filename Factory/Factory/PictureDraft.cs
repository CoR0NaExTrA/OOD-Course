using Factory.ShapeFactory;

namespace Factory;
public class PictureDraft
{
    private readonly List<Shape> _shapes = new List<Shape>();
    public void AddShape( Shape s ) => _shapes.Add( s );
    public int GetShapeCount() => _shapes.Count;
    public Shape GetShape( int index ) => _shapes[ index ];
    public IEnumerable<Shape> Shapes => _shapes;
}
