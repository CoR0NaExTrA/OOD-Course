using WeatherStation.Core;
using WeatherStation.Models;

namespace WeatherStation.Displays;

// Общий агрегатор статистики — по одному числовому потоку
public class StatsAggregator
{
    private double _min = double.PositiveInfinity;
    private double _max = double.NegativeInfinity;
    private double _sum = 0;
    private long _count = 0;

    public void Add( double value )
    {
        if ( value < _min )
            _min = value;
        if ( value > _max )
            _max = value;
        _sum += value;
        _count++;
    }

    public double Min => _count == 0 ? double.NaN : _min;
    public double Max => _count == 0 ? double.NaN : _max;
    public double Average => _count == 0 ? double.NaN : ( _sum / _count );
    public long Count => _count;
}

public class WindStatsAggregator
{
    private double _sumX = 0;
    private double _sumY = 0;
    private double _sumSpeed = 0;
    private long _count = 0;

    public void Add( double speed, double directionDegrees )
    {
        double radians = directionDegrees * Math.PI / 180.0;
        _sumX += speed * Math.Cos( radians );
        _sumY += speed * Math.Sin( radians );
        _sumSpeed += speed;
        _count++;
    }

    public double AverageSpeed => _count == 0 ? double.NaN : ( _sumSpeed / _count );

    public double AverageDirection
    {
        get
        {
            if ( _count == 0 )
                return double.NaN;
            double avgX = _sumX / _count;
            double avgY = _sumY / _count;
            double radians = Math.Atan2( avgY, avgX );
            double degrees = radians * 180.0 / Math.PI;
            return ( degrees + 360 ) % 360;
        }
    }
}

// Показатель текущего состояния (температура/влажность/давление)
public class CurrentConditionsDisplay : IObserver<WeatherStation.Models.WeatherInfo>
{
    private readonly string _name; // можно указывать имя индикатора

    public CurrentConditionsDisplay( string name = "Current" )
    {
        _name = name;
    }

    public void Update( WeatherInfo data, IObservable<WeatherStation.Models.WeatherInfo> source )
    {
        Console.WriteLine( $"{_name} display — current: Temp={data.Temperature:0.##} °C, Hum={data.Humidity:0.##}, Press={data.Pressure:0.##}" );
    }
}

// Умный статистический индикатор: использует один агрегатор на тип (темп/влаж/давление)
public class StatsDisplay : IObserver<WeatherInfo>
{
    private readonly StatsAggregator _tempAgg = new StatsAggregator();
    private readonly StatsAggregator _humAgg = new StatsAggregator();
    private readonly StatsAggregator _pressAgg = new StatsAggregator();
    private readonly string _name;

    public StatsDisplay( string name = "Stats" )
    {
        _name = name;
    }

    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        _tempAgg.Add( data.Temperature );
        _humAgg.Add( data.Humidity );
        _pressAgg.Add( data.Pressure );

        Console.WriteLine( $"[{_name}] maxT={_tempAgg.Max:0.##}, minT={_tempAgg.Min:0.##}, avgT={_tempAgg.Average:0.##}" );
        Console.WriteLine( $"[{_name}] maxH={_humAgg.Max:0.##}, minH={_humAgg.Min:0.##}, avgH={_humAgg.Average:0.##}" );
        Console.WriteLine( $"[{_name}] maxP={_pressAgg.Max:0.##}, minP={_pressAgg.Min:0.##}, avgP={_pressAgg.Average:0.##}" );
        Console.WriteLine( "--------------------------" );
    }
}

// Индикатор, который подписывается на несколько субъектов и распознаёт источник (DUO)
public class DuoDisplay : IObserver<WeatherInfo>
{
    private readonly Dictionary<IObservable<WeatherInfo>, string> _labels = new Dictionary<IObservable<WeatherInfo>, string>();
    private readonly string _name;

    // mapping: observable -> label (например, in/out)
    public DuoDisplay( string name = "Duo" )
    {
        _name = name;
    }

    public void AddSource( IObservable<WeatherInfo> observable, string label, int priority = 0 )
    {
        if ( !_labels.ContainsKey( observable ) )
        {
            _labels[ observable ] = label;
            observable.RegisterObserver( this, priority );
        }
    }

    public void RemoveSource( IObservable<WeatherInfo> observable )
    {
        if ( _labels.Remove( observable ) )
            observable.RemoveObserver( this );
    }

    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        string label = _labels.TryGetValue( source, out var l ) ? l : "unknown";
        Console.WriteLine( $"[{_name}] source={label}: Temp={data.Temperature:0.##}, Hum={data.Humidity:0.##}, Press={data.Pressure:0.##}" );
    }
}

public class StatsDisplayPro : IObserver<WeatherInfoPro>
{
    private readonly StatsAggregator _tempAgg = new StatsAggregator();
    private readonly StatsAggregator _humAgg = new StatsAggregator();
    private readonly StatsAggregator _pressAgg = new StatsAggregator();
    private readonly WindStatsAggregator _windAgg = new WindStatsAggregator();

    private readonly string _name;

    public StatsDisplayPro( string name = "ProStats" )
    {
        _name = name;
    }

    public void Update( WeatherInfoPro data, IObservable<WeatherInfoPro> source )
    {
        _tempAgg.Add( data.Weather.Temperature );
        _humAgg.Add( data.Weather.Humidity );
        _pressAgg.Add( data.Weather.Pressure );
        _windAgg.Add( data.Wind.Speed, data.Wind.Direction );

        Console.WriteLine( $"[{_name}] Temp avg={_tempAgg.Average:0.##}, min={_tempAgg.Min:0.##}, max={_tempAgg.Max:0.##}" );
        Console.WriteLine( $"[{_name}] Hum  avg={_humAgg.Average:0.##}, min={_humAgg.Min:0.##}, max={_humAgg.Max:0.##}" );
        Console.WriteLine( $"[{_name}] Press avg={_pressAgg.Average:0.##}, min={_pressAgg.Min:0.##}, max={_pressAgg.Max:0.##}" );
        Console.WriteLine( $"[{_name}] Wind avgSpeed={_windAgg.AverageSpeed:0.##} m/s, avgDir={_windAgg.AverageDirection:0.##}°" );
        Console.WriteLine( "-----------------------------" );
    }
}

public class DuoDisplayPro : IObserver<WeatherInfo>, IObserver<WeatherInfoPro>
{
    private readonly Dictionary<object, string> _labels = new();
    private readonly string _name;

    public DuoDisplayPro( string name = "DuoPro" )
    {
        _name = name;
    }

    public void AddSource<T>( IObservable<T> observable, string label, int priority = 0 )
    {
        if ( !_labels.ContainsKey( observable ) )
        {
            _labels[ observable ] = label;
            observable.RegisterObserver( this as IObserver<T>, priority );
        }
    }

    public void RemoveSource<T>( IObservable<T> observable )
    {
        if ( _labels.Remove( observable ) )
            observable.RemoveObserver( this as IObserver<T> );
    }

    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        string label = _labels.TryGetValue( source, out var l ) ? l : "unknown";
        Console.WriteLine( $"[{_name}] ({label}) Temp={data.Temperature:0.##}, Hum={data.Humidity:0.##}, Press={data.Pressure:0.##}" );
    }

    public void Update( WeatherInfoPro data, IObservable<WeatherInfoPro> source )
    {
        string label = _labels.TryGetValue( source, out var l ) ? l : "unknown";
        Console.WriteLine( $"[{_name}] ({label}) Temp={data.Weather.Temperature:0.##}, Hum={data.Weather.Humidity:0.##}, Press={data.Weather.Pressure:0.##}, Wind={data.Wind.Speed:0.##} m/s at {data.Wind.Direction:0.##}°" );
    }
}


