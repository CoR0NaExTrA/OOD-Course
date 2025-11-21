using Gumball.State.States;

namespace Gumball.State;

public class GumballMachine : IGumballMachine
{
    private readonly IState _soldState;
    private readonly IState _soldOutState;
    private readonly IState _noQuarterState;
    private readonly IState _hasQuarterState;

    private IState _state;
    private int _count;

    public GumballMachine( int numBalls )
    {
        _count = numBalls;

        _soldState = new SoldState( this );
        _soldOutState = new SoldOutState( this );
        _noQuarterState = new NoQuarterState( this );
        _hasQuarterState = new HasQuarterState( this );

        _state = ( _count > 0 ) ? _noQuarterState : _soldOutState;
    }

    public string InsertQuarter() => _state.InsertQuarter();
    public string EjectQuarter() => _state.EjectQuarter();
    public string TurnCrank() => _state.TurnCrank() + "\n" + _state.Dispense();

    public void ReleaseBall()
    {
        if ( _count > 0 )
            _count--;
    }

    public int GetBallCount() => _count;

    public void SetSoldOutState() => _state = _soldOutState;
    public void SetNoQuarterState() => _state = _noQuarterState;
    public void SetSoldState() => _state = _soldState;
    public void SetHasQuarterState() => _state = _hasQuarterState;

    public string ToString() =>
        $"Inventory: {_count} gumballs\nMachine is {_state.Name}";
}
