namespace MultiGumballMachine.State.States;

using MultiGumballMachine.State;

public class NoQuarterState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "waiting for quarter";

    public NoQuarterState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter()
    {
        _machine.AddCoin();
        _machine.SetHasQuarterState();
        return "You inserted a quarter";
    }

    public string EjectQuarter() => "You haven't inserted a quarter";

    public string TurnCrank() => "You turned but there's no quarter";

    public string Dispense() => "You need to pay first";
}
