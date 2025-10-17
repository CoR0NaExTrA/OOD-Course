namespace Factory.ShapeFactory;

public interface IShapeFactory
{
    Shape CreateShape( string descr );
}