// File: Styles/LineStyle.cs
using Slides.Models;

namespace Slides.Styles;

public class LineStyle : IEquatable<LineStyle>
{
    public bool Enabled { get; set; }
    public ColorRGBA Color { get; set; }
    public double Thickness { get; set; }
    public LineStyle( bool enabled = true, ColorRGBA? color = null, double thickness = 1.0 ) 
    { 
        Enabled = enabled; 
        Color = color ?? new ColorRGBA( 0, 0, 0, 255 ); 
        Thickness = thickness; 
    }
    public LineStyle Clone() => new LineStyle( Enabled, Color, Thickness );
    public bool Equals( LineStyle other )
    {
        if ( other == null )
        {
            return false;
        }
        return Enabled == other.Enabled && 
               Thickness == other.Thickness && 
               Color.R == other.Color.R && 
               Color.G == other.Color.G && 
               Color.B == other.Color.B && 
               Color.A == other.Color.A;
    }
    public override bool Equals( object obj ) => Equals( obj as LineStyle );
    public override int GetHashCode() => HashCode.Combine( Enabled, Color.R, Color.G, Color.B, Color.A, Thickness );
}