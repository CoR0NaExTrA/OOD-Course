using Gumball.State;

namespace Gumball.State.States;

public class HasQuarterState : IState
{
    private readonly IGumballMachine _machine;
    public string Name => "waiting for turn of crank";

    public HasQuarterState( IGumballMachine machine ) => _machine = machine;

    public string InsertQuarter() => "You can't insert another quarter";

    public string EjectQuarter()
    {
        _machine.SetNoQuarterState();
        return "Quarter returned";
    }

    public string TurnCrank()
    {
        _machine.SetSoldState();
        return "You turned...";
    }

    public string Dispense() => "No gumball dispensed";
}
