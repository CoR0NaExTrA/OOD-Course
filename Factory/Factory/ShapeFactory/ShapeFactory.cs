using System.Globalization;
//TODO: Разбить на методы и переименовать, рассказать про фабричный метод и чтобы изменилось если использовать его вместо абстрактной фабрики, UML поправить

namespace Factory.ShapeFactory;
public class ShapeFactory : IShapeFactory
{
    public Shape CreateShape( string descr )
    {
        var parts = SplitDescription( descr );
        if ( parts.Length == 0 )
            return null;

        var (type, color, paramStartIndex) = ParseTypeAndColor( parts );
        var parameters = ExtractNumericParameters( parts, paramStartIndex );

        return CreateShapeByType( type, color, parameters, parts.Length - paramStartIndex );
    }

    private string[] SplitDescription( string descr )
    {
        return descr.Split( new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );
    }

    private (string type, Color color, int paramStartIndex) ParseTypeAndColor( string[] parts )
    {
        var type = parts[ 0 ].ToLowerInvariant();
        Color color = Color.Black;
        int paramStartIndex = 1;

        if ( parts.Length > 1 && Enum.TryParse<Color>( parts[ 1 ], true, out var parsedColor ) )
        {
            color = parsedColor;
            paramStartIndex = 2;
        }

        return (type, color, paramStartIndex);
    }

    private double[] ExtractNumericParameters( string[] parts, int startIndex )
    {
        var parameters = new double[ parts.Length - startIndex ];
        for ( int i = 0; i < parameters.Length; i++ )
        {
            parameters[ i ] = double.Parse( parts[ startIndex + i ], CultureInfo.InvariantCulture );
        }
        return parameters;
    }

    private double GetParameter( double[] parameters, int index )
    {
        return parameters[ index ];
    }

    private Point CreatePoint( double x, double y )
    {
        return new Point( x, y );
    }

    private Shape CreateShapeByType( string type, Color color, double[] parameters, int parameterCount )
    {
        switch ( type )
        {
            case "rectangle":
            case "rect":
                ValidateParameterCount( parameterCount, 4, "Rectangle needs 4 numeric params: left top right bottom" );
                return CreateRectangle( color, parameters );

            case "triangle":
                ValidateParameterCount( parameterCount, 6, "Triangle needs 6 numeric params" );
                return CreateTriangle( color, parameters );

            case "ellipse":
                ValidateParameterCount( parameterCount, 4, "Ellipse needs 4 numeric params: cx cy rx ry" );
                return CreateEllipse( color, parameters );

            case "polygon":
            case "regularpolygon":
                ValidateParameterCount( parameterCount, 4, "RegularPolygon needs 4 params: vertexCount cx cy radius" );
                return CreateRegularPolygon( color, parameters );

            default:
                throw new Exception( $"Unknown shape type '{type}'" );
        }
    }

    private void ValidateParameterCount( int actualCount, int expectedCount, string errorMessage )
    {
        if ( actualCount < expectedCount )
            throw new Exception( errorMessage );
    }

    private Rectangle CreateRectangle( Color color, double[] parameters )
    {
        var left = GetParameter( parameters, 0 );
        var top = GetParameter( parameters, 1 );
        var right = GetParameter( parameters, 2 );
        var bottom = GetParameter( parameters, 3 );
        return new Rectangle( CreatePoint( left, top ), CreatePoint( right, bottom ), color );
    }

    private Triangle CreateTriangle( Color color, double[] parameters )
    {
        return new Triangle(
            CreatePoint( GetParameter( parameters, 0 ), GetParameter( parameters, 1 ) ),
            CreatePoint( GetParameter( parameters, 2 ), GetParameter( parameters, 3 ) ),
            CreatePoint( GetParameter( parameters, 4 ), GetParameter( parameters, 5 ) ),
            color );
    }

    private Ellipse CreateEllipse( Color color, double[] parameters )
    {
        var cx = GetParameter( parameters, 0 );
        var cy = GetParameter( parameters, 1 );
        var rx = GetParameter( parameters, 2 );
        var ry = GetParameter( parameters, 3 );
        return new Ellipse( CreatePoint( cx, cy ), rx, ry, color );
    }

    private RegularPolygon CreateRegularPolygon( Color color, double[] parameters )
    {
        int vertexCount = ( int )GetParameter( parameters, 0 );
        var cx = GetParameter( parameters, 1 );
        var cy = GetParameter( parameters, 2 );
        var radius = GetParameter( parameters, 3 );
        return new RegularPolygon( vertexCount, CreatePoint( cx, cy ), radius, color );
    }
}
