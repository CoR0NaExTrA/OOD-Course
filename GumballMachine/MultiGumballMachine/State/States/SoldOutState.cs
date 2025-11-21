namespace MultiGumballMachine.State.States;

using MultiGumballMachine.State;

public class SoldOutState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "sold out";

    public SoldOutState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter() => "You can't insert a quarter, the machine is sold out";

    public string EjectQuarter() => "You can't eject, you haven't inserted a quarter yet";

    public string TurnCrank() => "You turned but there's no gumballs";

    public string Dispense() => "No gumball dispensed";
}
