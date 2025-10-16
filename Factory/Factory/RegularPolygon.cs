using System.Drawing;

namespace Factory;
public class RegularPolygon : Shape
{
    public int VertexCount { get; }
    public Point Center { get; }
    public double Radius { get; }


    public RegularPolygon( int n, Point center, double r, Color color ) : base( color )
    {
        if ( n < 3 )
            throw new ArgumentException( "Polygon must have at least 3 vertices" );
        VertexCount = n;
        Center = center;
        Radius = r;
    }


    public override void Draw( ICanvas canvas )
    {
        canvas.SetColor( _color );
        var pts = new Point[ VertexCount ];
        for ( int i = 0; i < VertexCount; i++ )
        {
            double ang = 2.0 * Math.PI * i / VertexCount - Math.PI / 2.0; // start at top
            pts[ i ] = new Point( Center.X + Radius * Math.Cos( ang ), Center.Y + Radius * Math.Sin( ang ) );
        }
        for ( int i = 0; i < VertexCount; i++ )
        {
            var a = pts[ i ];
            var b = pts[ ( i + 1 ) % VertexCount ];
            canvas.DrawLine( a, b );
        }
    }
}
