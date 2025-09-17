using Shapes.Class.shapes;
using Shapes.Interface.shapes;
using System.Globalization;

namespace Shapes.Class;

public class CommandProcessor
{
    private readonly Picture _picture;

    public CommandProcessor( Picture picture )
    {
        _picture = picture;
    }

    public void Execute( string command )
    {
        var parts = command.Split( ' ', 2, StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length == 0 )
            return;

        var cmd = parts[ 0 ];
        var args = parts.Length > 1 ? parts[ 1 ] : "";

        try
        {
            switch ( cmd )
            {
                case "AddShape":
                    AddShape( args );
                    break;

                case "List":
                    List();
                    break;

                case "DrawPicture":
                    using ( var canvas = new gfx.SvgCanvas( "output.svg" ) )
                        _picture.DrawPicture( canvas );
                    Console.WriteLine( "Picture drawn to output.svg" );
                    break;

                case "DrawShape":
                    DrawShape( args );
                    break;

                case "MoveShape":
                    MoveShape( args );
                    break;

                case "MovePicture":
                    MovePicture( args );
                    break;

                case "DeleteShape":
                    DeleteShape( args );
                    break;

                case "ChangeColor":
                    ChangeColor( args );
                    break;

                case "ChangeShape":
                    ChangeShape( args );
                    break;

                default:
                    Console.WriteLine( $"Unknown command: {cmd}" );
                    break;
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Error: {ex.Message}" );
        }
    }

    private void AddShape( string args )
    {
        var parts = args.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length < 5 )
        {
            Console.WriteLine( "Invalid AddShape command" );
            return;
        }

        string id = parts[ 0 ];
        string color = parts[ 1 ];
        string type = parts[ 2 ];
        string[] parameters = parts.Skip( 3 ).ToArray();

        IShapeStrategy strategy = type switch
        {
            "circle" => new CircleStrategy(
                double.Parse( parameters[ 0 ], CultureInfo.InvariantCulture ),
                double.Parse( parameters[ 1 ], CultureInfo.InvariantCulture ),
                double.Parse( parameters[ 2 ], CultureInfo.InvariantCulture ) ),
            _ => throw new NotSupportedException( $"Shape type {type} not supported" )
        };

        var shape = new Shape( id, color, strategy );
        _picture.AddShape( shape );
        Console.WriteLine( $"Shape {id} added" );
    }

    private void List()
    {
        int i = 1;
        foreach ( var shape in _picture.List() )
        {
            Console.WriteLine( $"{i++} {shape.Type} {shape.Id} {shape.Color} {shape.Parameters}" );
        }
    }

    private void DrawShape( string args )
    {
        var id = args.Trim();
        if ( string.IsNullOrEmpty( id ) )
            throw new ArgumentException( "DrawShape <id>" );

        var shape = _picture.List().FirstOrDefault( s => s.Id == id ) ?? throw new KeyNotFoundException( $"Shape {id} not found" );

        using ( var canvas = new gfx.SvgCanvas( "output.svg" ) )
        {
            shape.Draw( canvas );
        }

        Console.WriteLine( $"Shape {id} drawn to output.svg" );
    }

    private void MoveShape( string args )
    {
        var parts = args.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length != 3 )
            throw new ArgumentException( "MoveShape <id> <dx> <dy>" );

        string id = parts[ 0 ];
        double dx = double.Parse( parts[ 1 ] );
        double dy = double.Parse( parts[ 2 ] );

        var shape = _picture.List().FirstOrDefault( s => s.Id == id )
            ?? throw new KeyNotFoundException( $"Shape {id} not found" );

        shape.Move( dx, dy );
        Console.WriteLine( $"Shape {id} moved" );
    }

    private void MovePicture( string args )
    {
        var parts = args.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length != 2 )
            throw new ArgumentException( "MovePicture <dx> <dy>" );

        double dx = double.Parse( parts[ 0 ] );
        double dy = double.Parse( parts[ 1 ] );

        _picture.MovePicture( dx, dy );
        Console.WriteLine( "Picture moved" );
    }

    private void DeleteShape( string args )
    {
        var id = args.Trim();
        _picture.DeleteShape( id );
        Console.WriteLine( $"Shape {id} deleted" );
    }

    private void ChangeColor( string args )
    {
        var parts = args.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length != 2 )
            throw new ArgumentException( "ChangeColor <id> <color>" );

        string id = parts[ 0 ];
        string color = parts[ 1 ];

        var shape = _picture.List().FirstOrDefault( s => s.Id == id )
            ?? throw new KeyNotFoundException( $"Shape {id} not found" );

        shape.ChangeColor( color );
        Console.WriteLine( $"Shape {id} color changed to {color}" );
    }

    private void ChangeShape( string args )
    {
        var parts = args.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        if ( parts.Length < 2 )
            throw new ArgumentException( "ChangeShape <id> <type> <params>" );

        string id = parts[ 0 ];
        string type = parts[ 1 ];
        string[] parameters = parts.Skip( 2 ).ToArray();

        var shape = _picture.List().FirstOrDefault( s => s.Id == id )
            ?? throw new KeyNotFoundException( $"Shape {id} not found" );

        IShapeStrategy newStrategy = type switch
        {
            "circle" => new CircleStrategy(
                double.Parse( parameters[ 0 ] ),
                double.Parse( parameters[ 1 ] ),
                double.Parse( parameters[ 2 ] ) ),
            _ => throw new NotSupportedException( $"Shape type {type} not supported" )
        };

        shape.ChangeShape( newStrategy );
        Console.WriteLine( $"Shape {id} changed to {type}" );
    }
}
