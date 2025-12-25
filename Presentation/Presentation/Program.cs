using Presentation.Core.DocumentSerializer;

namespace ShapesEditor;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode( HighDpiMode.SystemAware );
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault( false );

        var doc = new Document();
        var serializer = new JsonDocumentSerializer();

        Application.Run( new MainForm( doc, serializer ) );
    }
}
