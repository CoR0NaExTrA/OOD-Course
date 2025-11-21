namespace Gumball.State.States;

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
        if ( _machine.GetBallCount() == 0 )
        {
            _machine.SetSoldOutState();
            return "A gumball comes rolling out the slot...\nOops, out of gumballs";
        }
        _machine.SetNoQuarterState();
        return "A gumball comes rolling out the slot...";
    }
}
