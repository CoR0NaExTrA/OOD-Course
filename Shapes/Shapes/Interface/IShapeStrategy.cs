namespace Shapes.Interface.shapes;
public interface IShapeStrategy
{
    string Type { get; }
    void Draw( gfx.ICanvas canvas, string color );
    void Move( double dx, double dy );
    void Change( IReadOnlyList<string> parameters );
    string ParametersToString();
}
