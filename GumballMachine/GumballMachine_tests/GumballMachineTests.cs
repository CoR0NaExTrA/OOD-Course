using Gumball.Naive;
using Gumball.State;
using Gumball.State.States;
using Xunit;

public class GumballMachineStateTests
{
    [Fact]
    public void NoQuarterState_InsertQuarter_ChangesState()
    {
        var m = new GumballMachine( 5 );
        var result = m.InsertQuarter();

        Assert.Contains( "inserted a quarter", result );
        Assert.Contains( "waiting for turn of crank", m.ToString() );
    }

    [Fact]
    public void HasQuarterState_EjectQuarter_ChangesStateToNoQuarter()
    {
        var m = new GumballMachine( 5 );
        m.InsertQuarter();
        var result = m.EjectQuarter();

        Assert.Contains( "Quarter returned", result );
        Assert.Contains( "waiting for quarter", m.ToString() );
    }

    [Fact]
    public void SoldState_Dispense_ReleasesBall()
    {
        var m = new GumballMachine( 2 );
        m.InsertQuarter();
        m.TurnCrank();

        Assert.Contains( "Inventory: 1", m.ToString() );
    }

    [Fact]
    public void SoldOutState_InsertQuarter_Denied()
    {
        var m = new GumballMachine( 0 );
        var result = m.InsertQuarter();

        Assert.Contains( "sold out", m.ToString() );
        Assert.Contains( "can't insert", result );
    }
}

public class NaiveGumballMachineTests
{
    [Fact]
    public void InsertQuarter_WhenEmpty_Denied()
    {
        var m = new NaiveGumballMachine( 0 );
        var res = m.InsertQuarter();

        Assert.Contains( "sold out", res );
    }

    [Fact]
    public void FullCycle_WorksCorrectly()
    {
        var m = new NaiveGumballMachine( 1 );

        Assert.Contains( "inserted", m.InsertQuarter() );
        Assert.Contains( "gumball", m.TurnCrank() );
        Assert.Contains( "0 gumballs", m.ToString() );
    }
}

public class StateGumballMachineTests
{
    [Fact]
    public void FullCycle_WorksCorrectly()
    {
        var m = new GumballMachine( 1 );

        Assert.Contains( "inserted", m.InsertQuarter() );
        Assert.Contains( "A gumball", m.TurnCrank() );
        Assert.Contains( "0 gumballs", m.ToString() );
    }
}

