// File: Styles/FillStyle.cs
using Slides.Models;

namespace Slides.Styles;

public class FillStyle : IEquatable<FillStyle>
{
    public bool Enabled { get; set; }
    public ColorRGBA Color { get; set; }
    public FillStyle( bool enabled = true, ColorRGBA? color = null ) 
    { 
        Enabled = enabled; 
        Color = color ?? new ColorRGBA( 0, 0, 0, 255 ); 
    }
    public FillStyle Clone() => new FillStyle( Enabled, Color );
    public bool Equals( FillStyle other )
    {
        if ( other == null )
        {
            return false;
        }
        return Enabled == other.Enabled && 
               Color.R == other.Color.R && 
               Color.G == other.Color.G && 
               Color.B == other.Color.B && 
               Color.A == other.Color.A;
    }
    public override bool Equals( object obj ) => Equals( obj as FillStyle );
    public override int GetHashCode() => HashCode.Combine( Enabled, Color.R, Color.G, Color.B, Color.A );
}