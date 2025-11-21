using System;
using MultiGumballMachine.Naive;
using MultiGumballMachine.State;

class Program
{
    static void Main()
    {
        Console.WriteLine( "=== NAIVE MULTI-GUMBALL MACHINE ===" );
        var naive = new NaiveMultiGumballMachine( 5 );
        DemoNaive( naive );

        Console.WriteLine( "\n=== STATE MULTI-GUMBALL MACHINE ===" );
        var state = new StateMultiGumballMachine( 5 );
        DemoState( state );
    }

    static void DemoNaive( NaiveMultiGumballMachine m )
    {
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() ); // 3 coins
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.TurnCrank() ); // give 1, coins ->2
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.EjectQuarter() ); // return remaining coins
        Console.WriteLine( m.ToString() );

        // Scenario: insert more coins than balls
        m.Refill( 2 );
        Console.WriteLine( m.InsertQuarter() ); // 1
        Console.WriteLine( m.InsertQuarter() ); // 2
        Console.WriteLine( m.InsertQuarter() ); // 3
        Console.WriteLine( m.InsertQuarter() ); // 4
        Console.WriteLine( m.InsertQuarter() ); // 5
        Console.WriteLine( m.InsertQuarter() ); // 6th -> refused
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.TurnCrank() ); // -1 ball
        Console.WriteLine( m.TurnCrank() ); // -1 ball, now 0 balls but coins remain
        Console.WriteLine( m.ToString() );
        Console.WriteLine( m.EjectQuarter() ); // return leftover coins
        Console.WriteLine( m.ToString() );
    }

    static void DemoState( StateMultiGumballMachine m )
    {
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.TurnCrank() );
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.EjectQuarter() );
        Console.WriteLine( m.ToString() );

        // insert many coins, exceed limit
        m.Refill( 2 );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.InsertQuarter() ); // 6th -> refused
        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.TurnCrank() );
        Console.WriteLine( m.TurnCrank() ); // after second dispense balls -> 0, coins may remain
        Console.WriteLine( m.ToString() );
        Console.WriteLine( m.EjectQuarter() ); // return leftover coins when sold out
        Console.WriteLine( m.ToString() );
    }
}
