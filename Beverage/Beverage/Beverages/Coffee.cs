namespace Beverages;
public abstract class Coffee : BaseBeverage
{
    protected Coffee( string description ) : base( description ) { }
}

public enum Size { Standard, Double }

public class Latte : Coffee
{
    private readonly Size _size;
    public Latte( Size size = Size.Standard )
        : base( $"{( size == Size.Double ? "Double" : "Standard" )} Latte" )
    {
        _size = size;
    }

    public override double GetCost() => _size == Size.Double ? 130 : 90;
}

public class Cappuccino : Coffee
{
    private readonly Size _size;
    public Cappuccino( Size size = Size.Standard )
        : base( $"{( size == Size.Double ? "Double" : "Standard" )} Cappuccino" )
    {
        _size = size;
    }

    public override double GetCost() => _size == Size.Double ? 120 : 80;
}