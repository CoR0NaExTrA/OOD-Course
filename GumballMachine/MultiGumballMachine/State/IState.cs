namespace MultiGumballMachine.State;

public interface IState
{
    string InsertQuarter();
    string EjectQuarter();
    string TurnCrank();
    string Dispense();
    string Name { get; }
}
