using Beverages;
using Condiments;

namespace Condiments;
public enum LiqueurType { Nut, Chocolate }

public class Liqueur : CondimentDecorator
{
    private readonly LiqueurType _type;
    public Liqueur( IBeverage beverage, LiqueurType type ) : base( beverage )
    {
        _type = type;
    }

    protected override string GetCondimentDescription() => $"{_type} Liqueur";
    protected override double GetCondimentCost() => 50;
}
