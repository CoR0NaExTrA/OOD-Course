using Presentation.Core;
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

        var context = new DocumentContext();
        context.Open(new Document());

        var serializer = new JsonDocumentSerializer();

        Application.Run(new MainForm(context, serializer));

    }
}
