using MultiGumballMachine.State.States;

namespace MultiGumballMachine.State;

public class StateMultiGumballMachine : IGumballMachine
{
    private const int MaxCoins = 5;

    private readonly IState _soldState;
    private readonly IState _soldOutState;
    private readonly IState _noQuarterState;
    private readonly IState _hasQuarterState;
    private readonly IState _soldOutWithCoinsState;

    private IState _state;
    private int _count;
    private int _coins;

    public StateMultiGumballMachine( int numBalls )
    {
        _count = numBalls;
        _coins = 0;

        _soldState = new SoldState( this );
        _soldOutState = new SoldOutState( this );
        _noQuarterState = new NoQuarterState( this );
        _hasQuarterState = new HasQuarterState( this );
        _soldOutWithCoinsState = new SoldOutWithCoinsState( this );

        _state = _count > 0 ? _noQuarterState : _soldOutState;
    }

    public string InsertQuarter()
    {
        var res = _state.InsertQuarter();
        return res;
    }

    public string EjectQuarter()
    {
        return _state.EjectQuarter();
    }

    public string TurnCrank()
    {
        var turnMsg = _state.TurnCrank();

        if ( _state == _soldState || turnMsg.StartsWith( "You turned" ) )
        {
            if ( _coins > 0 && _count > 0 )
            {
                _coins--;
                _state = _soldState;
                var disp = _state.Dispense();
                if ( _count == 0 )
                {
                    if ( _coins > 0 )
                    {
                        _state = _soldOutWithCoinsState;
                    }
                    else
                    {
                        _state = _soldOutState;
                    }
                }
                else
                {
                    if ( _coins > 0 )
                        _state = _hasQuarterState;
                    else
                        _state = _noQuarterState;
                }

                ReleaseBall();

                return string.Join( Environment.NewLine, new[] { turnMsg, disp } ).TrimEnd();
            }
            else
            {
                var disp = _state.Dispense();
                return string.Join( Environment.NewLine, new[] { turnMsg, disp } ).TrimEnd();
            }
        }

        return turnMsg;
    }

    public void ReleaseBall()
    {
        if ( _count > 0 )
            _count--;
    }

    public int GetBallCount() => _count;

    public int GetCoinCount() => _coins;

    public void AddCoin()
    {
        if ( _coins < MaxCoins )
            _coins++;
    }

    public void ReturnAllCoins()
    {
        _coins = 0;
    }

    public void SetSoldOutState() => _state = _soldOutState;
    public void SetNoQuarterState() => _state = _noQuarterState;
    public void SetHasQuarterState() => _state = _hasQuarterState;
    public void SetSoldState() => _state = _soldState;
    public void SetSoldOutWithCoinsState() => _state = _soldOutWithCoinsState;

    public void Refill( int numBalls )
    {
        if ( numBalls < 0 )
            return;

        if ( _state == _soldState )
        {
            Console.WriteLine( "Cannot refill during dispensing" );
            return;
        }

        _count = numBalls;

        if ( _count > 0 )
        {
            _state = _coins > 0 ? _hasQuarterState : _noQuarterState;
        }
        else
        {
            _state = _coins > 0 ? _soldOutWithCoinsState : _soldOutState;
        }
    }


    public override string ToString()
    {
        var stateName = _state.Name;
        return string.Format(
            $@"Mighty Gumball, Inc.
            C#-enabled Standing Gumball Model #2025
            Inventory: {_count} gumball{( _count != 1 ? "s" : "" )}
            Coins: {_coins}
            Machine is {stateName}
            " );
    }
}
