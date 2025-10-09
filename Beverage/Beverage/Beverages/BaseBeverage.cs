namespace Beverages;
public abstract class BaseBeverage : IBeverage
{
    private readonly string _description;

    protected BaseBeverage( string description )
    {
        _description = description;
    }

    public virtual string GetDescription() => _description;
    public abstract double GetCost();
}
