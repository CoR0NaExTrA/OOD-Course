namespace Beverages;
public abstract class Coffee : BaseBeverage
{
    protected readonly Size _size;
    protected Coffee( string type, Size size )
        : base( $"{( size == Size.Double ? "Double" : "Standard" )} {type}" )
    {
        _size = size;
    }
}

public enum Size { Standard, Double }

public class Latte : Coffee
{
    public Latte( Size size = Size.Standard ): base( "Latte", size ) {}

    public override double GetCost() => _size == Size.Double ? 130 : 90;
}

public class Cappuccino : Coffee
{
    public Cappuccino( Size size = Size.Standard ): base( "Cappuccino", size ){}

    public override double GetCost() => _size == Size.Double ? 120 : 80;
}