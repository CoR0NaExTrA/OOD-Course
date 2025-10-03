using WeatherStation;
using WeatherStation.Displays;

class Program
{
    static void Main()
    {
        Console.WriteLine( "=== Weather Station demo (basic) ===" );

        // Обычная метеостанция
        var wd = new WeatherData();
        var current = new CurrentConditionsDisplay( "Main" );
        var stats = new StatsDisplay( "MainStats" );

        wd.RegisterObserver( current );
        wd.RegisterObserver( stats, priority: 1 );

        wd.SetMeasurements( 3.0, 70.0, 760 );
        wd.SetMeasurements( 4.0, 75.0, 762 );

        Console.WriteLine( "\n=== Pro Weather Station demo ===" );

        // Pro-метеостанция с ветром
        var proStation = new WeatherDataPro();
        var proStats = new StatsDisplayPro( "ProStats" );

        proStation.RegisterObserver( proStats );

        proStation.SetMeasurements( 10.0, 60.0, 755, 5.0, 90 );
        proStation.SetMeasurements( 12.0, 65.0, 752, 7.5, 135 );

        Console.WriteLine( "\n=== DuoPro demo (two stations) ===" );

        var indoor = new WeatherData();      // обычная станция
        var outdoorPro = new WeatherDataPro(); // внешняя Pro-станция

        var duo = new DuoDisplayPro( "BigIndicator" );

        duo.AddSource( indoor, "IN", priority: 1 );
        duo.AddSource( outdoorPro, "OUT", priority: 2 );

        indoor.SetMeasurements( 22.0, 40.0, 760 );
        outdoorPro.SetMeasurements( -5.0, 80.0, 750, 6.5, 200 );

        Console.WriteLine( "\n=== End demo ===" );
    }
}
