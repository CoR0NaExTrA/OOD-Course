using Shapes.Class;
using Shapes.Class.shapes;

var picture = new Picture();
var processor = new CommandProcessor( picture );

Console.WriteLine( "Enter commands (type 'exit' to quit):" );

while ( true )
{
    Console.Write( "> " );
    var line = Console.ReadLine();
    if ( line == null || line.Equals( "exit", StringComparison.OrdinalIgnoreCase ) )
        break;

    processor.Execute( line );
}
