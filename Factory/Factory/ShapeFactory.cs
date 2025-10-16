using System.Drawing;
using System.Globalization;

namespace Factory;
public class ShapeFactory : IShapeFactory
{
    public Shape CreateShape( string descr )
    {
        var parts = descr.Split( new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length == 0 )
            return null;


        var type = parts[ 0 ].ToLowerInvariant();
        int idx = 1;


        // optional color next
        Color color = Color.Black;
        if ( parts.Length > 1 && Enum.TryParse<Color>( parts[ 1 ], true, out var c ) )
        {
            color = c;
            idx = 2;
        }


        // helpers
        double p( int i ) => double.Parse( parts[ idx + i ], CultureInfo.InvariantCulture );


        switch ( type )
        {
            case "rectangle":
            case "rect":
                // rect left top right bottom
                if ( parts.Length < idx + 4 )
                    throw new Exception( "Rectangle needs 4 numeric params: left top right bottom" );
                var left = p( 0 );
                var top = p( 1 );
                var right = p( 2 );
                var bottom = p( 3 );
                return new Rectangle( new Point( left, top ), new Point( right, bottom ), color );


            case "triangle":
                // triangle x1 y1 x2 y2 x3 y3
                if ( parts.Length < idx + 6 )
                    throw new Exception( "Triangle needs 6 numeric params" );
                return new Triangle(
                new Point( p( 0 ), p( 1 ) ),
                new Point( p( 2 ), p( 3 ) ),
                new Point( p( 4 ), p( 5 ) ),
                color );


            case "ellipse":
                // ellipse cx cy rx ry
                if ( parts.Length < idx + 4 )
                    throw new Exception( "Ellipse needs 4 numeric params: cx cy rx ry" );
                return new Ellipse( new Point( p( 0 ), p( 1 ) ), p( 2 ), p( 3 ), color );


            case "polygon":
            case "regularpolygon":
            case "poly":
                // polygon n cx cy radius (regular polygon)
                if ( parts.Length < idx + 4 )
                    throw new Exception( "RegularPolygon needs 4 params: vertexCount cx cy radius" );
                int n = int.Parse( parts[ idx ], CultureInfo.InvariantCulture );
                double cx = p( 1 );
                double cy = p( 2 );
                double r = p( 3 );
                return new RegularPolygon( n, new Point( cx, cy ), r, color );


            case "polypts":
                // polypts color x1 y1 x2 y2 ...
                throw new NotImplementedException( "polypts not implemented. Use polygon (regular) or explicit shapes." );


            default:
                throw new Exception( $"Unknown shape type '{parts[ 0 ]}'" );
        }
    }
}
