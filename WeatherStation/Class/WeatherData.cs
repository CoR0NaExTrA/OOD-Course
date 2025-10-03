using WeatherStation.Core;
using WeatherStation.Models;

namespace WeatherStation;

public class WeatherData : Observable<WeatherInfo>
{
    private double _temperature = 0.0;
    private double _humidity = 0.0;
    private double _pressure = 760.0;

    public double GetTemperature() => _temperature;
    public double GetHumidity() => _humidity;
    public double GetPressure() => _pressure;

    public void SetMeasurements( double temp, double humidity, double pressure )
    {
        _temperature = temp;
        _humidity = humidity;
        _pressure = pressure;
        MeasurementsChanged();
    }

    public void MeasurementsChanged()
    {
        NotifyObservers();
    }

    protected override WeatherInfo GetChangedData()
    {
        return new WeatherInfo
        {
            Temperature = GetTemperature(),
            Humidity = GetHumidity(),
            Pressure = GetPressure()
        };
    }
}

public class WeatherDataPro : Observable<WeatherInfoPro>
{
    private double _temperature = 0.0;
    private double _humidity = 0.0;
    private double _pressure = 760.0;
    private double _windSpeed = 0.0;
    private double _windDirection = 0.0;

    public void SetMeasurements( double temp, double hum, double press,
                                double windSpeed, double windDirection )
    {
        _temperature = temp;
        _humidity = hum;
        _pressure = press;
        _windSpeed = windSpeed;
        _windDirection = windDirection;
        MeasurementsChanged();
    }

    public void MeasurementsChanged()
    {
        NotifyObservers();
    }

    protected override WeatherInfoPro GetChangedData()
    {
        return new WeatherInfoPro
        {
            Weather = new WeatherInfo
            {
                Temperature = _temperature,
                Humidity = _humidity,
                Pressure = _pressure
            },
            Wind = new WindInfo
            {
                Speed = _windSpeed,
                Direction = _windDirection
            }
        };
    }
}
