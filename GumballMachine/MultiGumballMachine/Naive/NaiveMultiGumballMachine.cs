using System;
using System.Text;

namespace MultiGumballMachine.Naive;

public class NaiveMultiGumballMachine
{
    private const int MaxCoins = 5;
    private int _count; // шарики
    private int _coins; // монеты

    public NaiveMultiGumballMachine( int count )
    {
        _count = count;
        _coins = 0;
    }

    public string InsertQuarter()
    {
        if ( _coins >= MaxCoins )
        {
            return "You can't insert another quarter, the machine already has 5 quarters";
        }

        if ( _count == 0 )
        {
            return "You can't insert a quarter, the machine is sold out";
        }

        _coins++;
        return "You inserted a quarter";
    }

    // Возвращает и обнуляет все монеты
    public string EjectQuarter()
    {
        if ( _coins == 0 )
            return "You haven't inserted a quarter";

        var returned = _coins;
        _coins = 0;
        return $"Returned {returned} quarter{( returned > 1 ? "s" : "" )}";
    }

    public string TurnCrank()
    {
        if ( _coins == 0 )
        {
            return "You turned but there's no quarter";
        }

        if ( _count == 0 )
        {
            // No balls — can't dispense; but coins may exist
            return "You turned but there's no gumballs";
        }

        // Dispense one gumball, consume one coin
        _coins--;
        _count--;
        var sb = new StringBuilder();
        sb.AppendLine( "You turned..." );
        sb.AppendLine( "A gumball comes rolling out the slot..." );

        if ( _count == 0 )
        {
            if ( _coins > 0 )
            {
                sb.AppendLine( "Oops, out of gumballs" );
                sb.AppendLine( "Machine is sold out but still has coins" );
                // remain in state where coins are stored — user can EjectQuarter to get them back
            }
            else
            {
                sb.AppendLine( "Oops, out of gumballs" );
            }
        }

        return sb.ToString().TrimEnd();
    }

    public void Refill( int numBalls )
    {
        if ( numBalls < 0 )
            return;
        _count = numBalls;
    }

    public int GetBallCount() => _count;
    public int GetCoinCount() => _coins;

    public override string ToString()
    {
        var state = GetStateDescription();
        return string.Format(
$@"Mighty Gumball, Inc.
C#-enabled Standing Gumball Model #2025
Inventory: {_count} gumball{( _count != 1 ? "s" : "" )}
Coins: {_coins}
Machine is {state}
" );
    }

    private string GetStateDescription()
    {
        if ( _count == 0 && _coins == 0 )
            return "sold out";
        if ( _count == 0 && _coins > 0 )
            return "sold out (coins stored)";
        if ( _coins == 0 )
            return "waiting for quarter";
        return $"waiting for turn of crank ({_coins} quarter{( _coins != 1 ? "s" : "" )} inserted)";
    }
}
