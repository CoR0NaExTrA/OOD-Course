using Beverages;

namespace Condiments;
public class Lemon : CondimentDecorator
{
    private readonly int _quantity;
    public Lemon( IBeverage beverage, int quantity = 1) : base( beverage )
    {
        _quantity = quantity;
    }

    protected override string GetCondimentDescription() => $"Lemon x{_quantity}";
    protected override double GetCondimentCost() => 10 * _quantity;
}
