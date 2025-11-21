namespace MultiGumballMachine.State.States;

using MultiGumballMachine.State;

public class SoldState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "delivering a gumball";

    public SoldState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter() => "Please wait, we're already giving you a gumball";

    public string EjectQuarter() => "Sorry you already turned the crank";

    public string TurnCrank() => "Turning twice doesn't get you another gumball";

    public string Dispense()
    {
        _machine.ReleaseBall();
        return "A gumball comes rolling out the slot...";
    }
}
