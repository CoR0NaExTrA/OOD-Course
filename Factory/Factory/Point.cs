namespace Factory;
public struct Point
{
    public double X { get; }
    public double Y { get; }
    public Point( double x, double y ) { X = x; Y = y; }
    public override string ToString() => string.Format( System.Globalization.CultureInfo.InvariantCulture, "({0:0.##},{1:0.##})", X, Y );
}
