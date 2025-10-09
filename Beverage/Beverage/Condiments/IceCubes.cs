using Beverages;

namespace Condiments;
public enum IceCubeType { Water, Dry }

public class IceCubes : CondimentDecorator
{
    private readonly int _quantity;
    private readonly IceCubeType _type;

    public IceCubes( IBeverage beverage, int quantity, IceCubeType type = IceCubeType.Water )
        : base( beverage )
    {
        _quantity = quantity;
        _type = type;
    }

    protected override string GetCondimentDescription() =>
        $"{_type} ice cubes x{_quantity}";

    protected override double GetCondimentCost() =>
        ( _type == IceCubeType.Dry ? 10 : 5 ) * _quantity;
}
