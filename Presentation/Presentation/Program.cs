using System;
using System.Windows.Forms;

namespace ShapesEditor;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode( HighDpiMode.SystemAware );
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault( false );

        var form = new MainForm();
        var form1 = new MainForm();
        form1.Show();
        Application.Run( form );
    }
}
