using Factory.ShapeFactory;

namespace Factory.Designer;
public class Designer : IDesigner
{
    private readonly IShapeFactory _factory;


    public Designer( IShapeFactory factory )
    {
        _factory = factory;
    }


    public PictureDraft CreateDraft( TextReader strm )
    {
        var draft = new PictureDraft();
        string line;
        int lineNo = 0;
        while ( ( line = strm.ReadLine() ) != null )
        {
            lineNo++;
            line = line.Trim();
            if ( line.Length == 0 || line.StartsWith( "#" ) )
                continue;
            try
            {
                var shape = _factory.CreateShape( line );
                if ( shape != null )
                    draft.AddShape( shape );
            }
            catch ( Exception ex )
            {
                // simple error reporting to stdout
                Console.Error.WriteLine( $"Failed to parse line {lineNo}: {ex.Message}" );
            }
        }


        return draft;
    }
}
