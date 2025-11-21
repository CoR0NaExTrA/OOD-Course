namespace MultiGumballMachine.State;

public interface IGumballMachine
{
    void ReleaseBall();
    int GetBallCount();
    int GetCoinCount();

    void AddCoin();
    void ReturnAllCoins();

    void SetSoldOutState();
    void SetNoQuarterState();
    void SetHasQuarterState();
    void SetSoldState();
    void SetSoldOutWithCoinsState();
}
