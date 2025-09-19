namespace Shapes.Interface.shapes;
public interface IShapeStrategy
{
    string GetTypeName();
    void Draw( gfx.ICanvas canvas, string color );
    void Move( double dx, double dy );
    void Change( IReadOnlyList<string> parameters );
    string ParametersToString();
}
