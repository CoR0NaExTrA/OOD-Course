using Beverages;

namespace Condiments;
public class Cinnamon : CondimentDecorator
{
    public Cinnamon( IBeverage beverage ) : base( beverage ) { }

    protected override string GetCondimentDescription() => "Cinnamon";
    protected override double GetCondimentCost() => 20;
}