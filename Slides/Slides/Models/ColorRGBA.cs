// File: Models/ColorRGBA.cs
namespace Slides.Models;

public struct ColorRGBA
{
    public byte R, G, B, A;
    public ColorRGBA( byte r, byte g, byte b, byte a = 255 ) 
    { 
        R = r; 
        G = g; 
        B = b; 
        A = a; 
    }
    public string ToSvgColor() => $"rgba({R},{G},{B},{A / 255.0:0.##})";
    public override string ToString() => $"rgba({R},{G},{B},{A})";
}