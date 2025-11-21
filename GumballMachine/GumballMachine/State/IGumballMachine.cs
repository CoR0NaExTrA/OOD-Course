namespace Gumball.State;

public interface IGumballMachine
{
    void ReleaseBall();
    int GetBallCount();

    void SetSoldOutState();
    void SetNoQuarterState();
    void SetHasQuarterState();
    void SetSoldState();
}
