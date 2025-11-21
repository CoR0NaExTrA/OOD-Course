using MultiGumballMachine.Naive;
using MultiGumballMachine.State;

public class NaiveMultiGumballMachineTests
{
    [Fact]
    public void InsertQuarter_IncreasesCoinCount_UpTo5()
    {
        var m = new NaiveMultiGumballMachine( 3 );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );

        Assert.Contains( "can't insert", m.InsertQuarter() );
    }

    [Fact]
    public void TurnCrank_WithNoCoins_ReturnsNoQuarterMessage()
    {
        var m = new NaiveMultiGumballMachine( 3 );
        var res = m.TurnCrank();
        Assert.Contains( "no quarter", res );
    }

    [Fact]
    public void TurnCrank_DispensesOneBallAndConsumesOneCoin()
    {
        var m = new NaiveMultiGumballMachine( 2 );
        m.InsertQuarter();
        m.InsertQuarter();
        var beforeBalls = m.GetBallCount();
        var beforeCoins = m.GetCoinCount();
        var res = m.TurnCrank();
        Assert.Contains( "A gumball comes rolling out", res );
        Assert.Equal( beforeBalls - 1, m.GetBallCount() );
        Assert.Equal( beforeCoins - 1, m.GetCoinCount() );
    }

    [Fact]
    public void EjectQuarter_ReturnsAllCoins_AndResetsCoins()
    {
        var m = new NaiveMultiGumballMachine( 2 );
        m.InsertQuarter();
        m.InsertQuarter();
        var res = m.EjectQuarter();
        Assert.Contains( "Returned 2", res );
        Assert.Equal( 0, m.GetCoinCount() );
    }

    [Fact]
    public void SoldOutWithCoins_AllowsReturnOfCoins()
    {
        var m = new NaiveMultiGumballMachine( 1 );
        m.InsertQuarter();
        m.InsertQuarter();
        var r1 = m.TurnCrank();
        Assert.Contains( "A gumball comes", r1 );
        var r2 = m.TurnCrank();
        Assert.Contains( "no gumballs", r2 );
        var eject = m.EjectQuarter();
        Assert.Contains( "Returned", eject );
    }
}

public class StateMultiGumballMachineTests
{
    [Fact]
    public void InsertQuarter_AcceptsUpTo5()
    {
        var m = new StateMultiGumballMachine( 3 );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Equal( "You inserted a quarter", m.InsertQuarter() );
        Assert.Contains( "can't insert", m.InsertQuarter() );
    }

    [Fact]
    public void TurnCrank_WorksAndTransitionsStatesCorrectly()
    {
        var m = new StateMultiGumballMachine( 2 );
        m.InsertQuarter();
        m.InsertQuarter(); // 2 coins

        var res1 = m.TurnCrank();
        Assert.Contains( "A gumball comes", res1 );

        var res2 = m.TurnCrank();
        Assert.Contains( "A gumball comes", res2 );
        // now inventory 0
        Assert.Contains( "Inventory: 0", m.ToString() );
        // because coins consumed, either sold out or soldOutWithCoins depending on coins left
    }

    [Fact]
    public void EjectQuarter_ReturnsAllCoins()
    {
        var m = new StateMultiGumballMachine( 3 );
        m.InsertQuarter();
        m.InsertQuarter();
        var res = m.EjectQuarter();
        Assert.Contains( "Quarter(s) returned", res );
        Assert.Contains( "Coins: 0", m.ToString() );
    }

    [Fact]
    public void SoldOutWithCoins_ReturnsCoins()
    {
        var m = new StateMultiGumballMachine( 1 );
        m.InsertQuarter();
        m.InsertQuarter(); // 2 coins for 1 ball
        var r1 = m.TurnCrank(); // gives last ball; machine now sold out with coins
        Assert.Contains( "A gumball comes", r1 );
        Assert.Contains( "sold out", m.ToString() );
        // Eject should return remaining coin(s)
        var r2 = m.EjectQuarter();
        Assert.True( r2.Contains( "Returned" ) || r2.Contains( "returned" ) || r2.Contains( "Quarter(s) returned" ) );
        Assert.Contains( "Coins: 0", m.ToString() );
    }

    [Fact]
    public void CannotTurnIfNoCoin()
    {
        var m = new StateMultiGumballMachine( 2 );
        var res = m.TurnCrank();
        Assert.Contains( "no quarter", res );
    }
}
