using Beverages;
using Condiments;

namespace Beverage.Condiments;
public enum SyrupType { Chocolate, Maple }

public class Syrup : CondimentDecorator
{
    private readonly SyrupType _type;

    public Syrup( IBeverage beverage, SyrupType type ) : base( beverage )
    {
        _type = type;
    }

    protected override string GetCondimentDescription() => $"{_type} syrup";
    protected override double GetCondimentCost() => 15;
}
