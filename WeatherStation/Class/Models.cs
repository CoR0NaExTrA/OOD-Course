namespace WeatherStation.Models;

// Структура, содержащая погодные данные
public struct WeatherInfo
{
    public double Temperature; // °C
    public double Humidity;    // 0..100
    public double Pressure;    // mmHg
}

public struct WindInfo
{
    public double Speed;      // м/с
    public double Direction;  // градусы: 0=N, 90=E, 180=S, 270=W
}

public struct WeatherInfoPro
{
    public WeatherInfo Weather;
    public WindInfo Wind;
}