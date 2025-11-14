// File: Slide.cs
using Slides.Canvas;
using Slides.Shapes;

public class Slide
{
    private readonly List<IShape> _shapes = new List<IShape>();
    public void AddShape( IShape s ) => _shapes.Add( s );
    public void RemoveShape( IShape s ) => _shapes.Remove( s );
    public void Draw( ICanvas canvas ) 
    { 
        foreach ( var s in _shapes ) s.Draw( canvas ); 
    }
}