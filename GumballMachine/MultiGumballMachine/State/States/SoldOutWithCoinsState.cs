namespace MultiGumballMachine.State.States;

using MultiGumballMachine.State;

public class SoldOutWithCoinsState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "sold out (coins stored)";

    public SoldOutWithCoinsState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter() => "You can't insert a quarter, the machine is sold out";

    public string EjectQuarter()
    {
        _machine.ReturnAllCoins();
        _machine.SetSoldOutState();
        return "Returned all coins";
    }

    public string TurnCrank() => "You turned but there's no gumballs";

    public string Dispense() => "No gumball dispensed";
}
