using System.Drawing;
using Factory.Canvas;

namespace Factory.ShapeFactory;
public abstract class Shape
{
    protected readonly Color _color;
    public Shape( Color color ) { _color = color; }
    public Color GetColor() => _color;
    public abstract void Draw( ICanvas canvas );
}
