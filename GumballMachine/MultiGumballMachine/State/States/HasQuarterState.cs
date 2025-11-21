namespace MultiGumballMachine.State.States;

using MultiGumballMachine.State;

public class HasQuarterState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "waiting for turn of crank";

    public HasQuarterState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter()
    {
        if ( _machine.GetCoinCount() >= 5 )
        {
            return "You can't insert another quarter, the machine already has 5 quarters";
        }

        _machine.AddCoin();
        return "You inserted a quarter";
    }

    public string EjectQuarter()
    {
        _machine.ReturnAllCoins();
        _machine.SetNoQuarterState();
        return "Quarter(s) returned";
    }

    public string TurnCrank()
    {
        _machine.SetSoldState();
        return "You turned...";
    }

    public string Dispense() => "No gumball dispensed";
}
