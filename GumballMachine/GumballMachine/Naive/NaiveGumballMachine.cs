namespace Gumball.Naive;

public class NaiveGumballMachine
{
    public enum State
    {
        SoldOut,
        NoQuarter,
        HasQuarter,
        Sold
    }

    private State _state;
    private int _count;

    public NaiveGumballMachine( int count )
    {
        _count = count;
        _state = count > 0 ? State.NoQuarter : State.SoldOut;
    }

    public string InsertQuarter()
    {
        return _state switch
        {
            State.SoldOut => "You can't insert a quarter, the machine is sold out",
            State.NoQuarter => (_state = State.HasQuarter, "You inserted a quarter").Item2,
            State.HasQuarter => "You can't insert another quarter",
            State.Sold => "Please wait, we're already giving you a gumball",
            _ => ""
        };
    }

    public string EjectQuarter()
    {
        return _state switch
        {
            State.HasQuarter => (_state = State.NoQuarter, "Quarter returned").Item2,
            State.NoQuarter => "You haven't inserted a quarter",
            State.Sold => "Sorry you already turned the crank",
            State.SoldOut => "You can't eject, you haven't inserted a quarter yet",
            _ => ""
        };
    }

    public string TurnCrank()
    {
        return _state switch
        {
            State.SoldOut => "You turned but there's no gumballs",
            State.NoQuarter => "You turned but there's no quarter",
            State.HasQuarter => TurnWithQuarter(),
            State.Sold => "Turning twice doesn't get you another gumball",
            _ => ""
        };
    }

    private string TurnWithQuarter()
    {
        _state = State.Sold;
        return "You turned...\n" + Dispense();
    }

    private string Dispense()
    {
        if ( _state != State.Sold )
            return "No gumball dispensed";

        _count--;
        if ( _count == 0 )
        {
            _state = State.SoldOut;
            return "A gumball comes rolling out the slot\nOops, out of gumballs";
        }
        else
        {
            _state = State.NoQuarter;
            return "A gumball comes rolling out the slot";
        }
    }

    public string ToString() =>
        $"Inventory: {_count} gumballs\nState: {_state}";
}
