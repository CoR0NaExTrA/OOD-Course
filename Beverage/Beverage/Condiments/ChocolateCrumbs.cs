using Beverages;
using Condiments;

namespace Beverage.Condiments;
public class ChocolateCrumbs : CondimentDecorator
{
    private readonly int _grams;

    public ChocolateCrumbs( IBeverage beverage, int grams ) : base( beverage )
    {
        _grams = grams;
    }

    protected override string GetCondimentDescription() => $"Chocolate crumbs {_grams}g";
    protected override double GetCondimentCost() => 2 * _grams;
}
