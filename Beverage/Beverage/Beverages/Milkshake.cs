namespace Beverages;

public enum PortionSize { Small, Medium, Large }

public class Milkshake : BaseBeverage
{
    private readonly PortionSize _size;
    public Milkshake( PortionSize size = PortionSize.Medium ) : base( $"{size} Milkshake" )
    {
        _size = size;
    }

    public override double GetCost() =>
        _size switch
        {
            PortionSize.Small => 50,
            PortionSize.Medium => 60,
            PortionSize.Large => 80,
            _ => 60
        };
}
