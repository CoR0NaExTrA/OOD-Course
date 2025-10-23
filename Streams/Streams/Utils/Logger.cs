namespace Transform.Utils;

public static class Logger
{
    private static readonly string LogFile = "transform_debug.log";
    private static readonly object LockObject = new object();

    public static void Log( string message )
    {
        lock ( LockObject )
        {
            File.AppendAllText( LogFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}{Environment.NewLine}" );
        }
    }
}