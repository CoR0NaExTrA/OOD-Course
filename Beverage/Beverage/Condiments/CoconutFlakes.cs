using Beverages;
using Condiments;

namespace Beverage.Condiments;
public class CoconutFlakes : CondimentDecorator
{
    private readonly int _grams;

    public CoconutFlakes( IBeverage beverage, int grams ) : base( beverage )
    {
        _grams = grams;
    }

    protected override string GetCondimentDescription() => $"Coconut flakes {_grams}g";
    protected override double GetCondimentCost() => 1 * _grams;
}