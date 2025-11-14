// File: Models/Rect.cs
namespace Slides.Models;

public struct Rect
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Rect( double x, double y, double w, double h ) 
    { 
        X = x; 
        Y = y; 
        Width = w; 
        Height = h; 
    }
    public double Left => X;
    public double Top => Y;
    public double Right => X + Width;
    public double Bottom => Y + Height;
    public static Rect Empty => new Rect( 0, 0, 0, 0 );
    public override string ToString() => $"Rect(X={X:0.##},Y={Y:0.##},W={Width:0.##},H={Height:0.##})";
}