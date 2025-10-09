using Beverages;

namespace Condiments;
public class ChocolateBar : CondimentDecorator
{
    private readonly int _pieces;
    public ChocolateBar( IBeverage beverage, int pieces ) : base( beverage )
    {
        _pieces = pieces > 5 ? 5 : pieces;
    }

    protected override string GetCondimentDescription() => $"Chocolate bar x{_pieces}";
    protected override double GetCondimentCost() => 10 * _pieces;
}
