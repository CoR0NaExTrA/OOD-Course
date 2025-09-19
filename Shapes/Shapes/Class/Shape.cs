// Shapes.Class/shapes/Shape.cs
using System;
using System.Text.RegularExpressions;
using Shapes.Interface.gfx;
using Shapes.Interface.shapes;

namespace Shapes.Class.shapes;
public class Shape
{
    public string Id { get; }
    private string _color;
    private IShapeStrategy _strategy;

    public Shape( string id, string color, IShapeStrategy strategy )
    {
        Id = id ?? throw new ArgumentNullException( nameof( id ) );
        _color = NormalizeColor( color );
        _strategy = strategy ?? throw new ArgumentNullException( nameof( strategy ) );
    }

    public void Draw( ICanvas canvas ) => _strategy.Draw( canvas, _color );

    public void Move( double dx, double dy ) => _strategy.Move( dx, dy );

    public void ChangeColor( string color ) => _color = NormalizeColor( color );

    public void ChangeShape( IShapeStrategy newStrategy ) => _strategy = newStrategy ?? throw new ArgumentNullException( nameof( newStrategy ) );

    public string Type => _strategy.GetTypeName();
    public string Color => _color;
    public string Parameters => _strategy.ParametersToString();

    private static string NormalizeColor( string color )
    {
        if ( string.IsNullOrWhiteSpace( color ) )
            throw new ArgumentException( "Color cannot be empty", nameof( color ) );

        color = color.Trim();
        if ( !color.StartsWith( "#" ) || color.Length != 7 )
            throw new FormatException( "Color must be in format #RRGGBB" );

        var hex = color.Substring( 1 );
        if ( !Regex.IsMatch( hex, @"\A[0-9A-Fa-f]{6}\z" ) )
            throw new FormatException( "Color must be in format #RRGGBB" );

        return "#" + hex.ToUpperInvariant();
    }
}
