using Gumball.State;

class Program
{
    static void Main()
    {
        var m = new GumballMachine( 5 );

        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.TurnCrank() );

        Console.WriteLine( m.ToString() );

        Console.WriteLine( m.InsertQuarter() );
        Console.WriteLine( m.EjectQuarter() );
        Console.WriteLine( m.TurnCrank() );

        Console.WriteLine( m.ToString() );

        m.InsertQuarter();
        m.EjectQuarter();
        m.TurnCrank();

        Console.WriteLine( m.ToString() );
    }
}
