namespace Beverages;

public enum TeaType { Black, Green, Oolong, Herbal }

public class Tea : BaseBeverage
{
    private readonly TeaType _type;
    public Tea( TeaType type = TeaType.Black ) : base( $"{type} Tea" )
    {
        _type = type;
    }

    public override double GetCost() => 30;
}