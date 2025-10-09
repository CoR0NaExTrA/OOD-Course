using Beverages;

namespace Condiments;
public class Cream : CondimentDecorator
{
    public Cream( IBeverage beverage ) : base( beverage ) { }

    protected override string GetCondimentDescription() => "Cream";
    protected override double GetCondimentCost() => 25;
}