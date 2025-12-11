using Presentation.Presenter;
using Presentation.Shapes;
using SkiaSharp.Views.Desktop;


namespace ShapesEditor;

public partial class MainForm : Form, ICanvasView
{
    private SKControl skControl;
    private ToolStrip toolStrip;
    private CanvasPresenter presenter;
    private string currentFile = null;

    public MainForm()
    {
        BuildUi();
        presenter = new CanvasPresenter( this, new Document() );
    }

    private void BuildUi()
    {
        Text = "Shapes Editor";
        Width = 1000;
        Height = 700;

        toolStrip = new ToolStrip();
        var addRectBtn = new ToolStripButton( "Rect" );
        var addEllipseBtn = new ToolStripButton( "Ellipse" );
        var addTriBtn = new ToolStripButton( "Triangle" );
        var deleteBtn = new ToolStripButton( "Delete" );
        var menu = new MenuStrip();
        var file = new ToolStripMenuItem( "File" );

        var insertImage = new ToolStripMenuItem( "Insert image" );
        var open = new ToolStripMenuItem( "Open" );
        var save = new ToolStripMenuItem( "Save" );
        var saveAs = new ToolStripMenuItem( "Save As" );

        file.DropDownItems.Add( insertImage );
        file.DropDownItems.Add( open );
        file.DropDownItems.Add( save );
        file.DropDownItems.Add( saveAs );
        menu.Items.Add( file );

        Controls.Add( menu );
        menu.Dock = DockStyle.Top;
        toolStrip.Items.AddRange( new ToolStripItem[] { addRectBtn, addEllipseBtn, addTriBtn, new ToolStripSeparator(), deleteBtn } );
        Controls.Add( toolStrip );
        toolStrip.Dock = DockStyle.Top;

        skControl = new SKControl();
        skControl.Dock = DockStyle.Fill;
        Controls.Add( skControl );

        // Events
        skControl.PaintSurface += ( s, e ) => presenter.OnPaint( e.Surface.Canvas, e.Info.Width, e.Info.Height );
        skControl.MouseDown += ( s, e ) => presenter.OnMouseDown( e );
        skControl.MouseMove += ( s, e ) => presenter.OnMouseMove( e );
        skControl.MouseUp += ( s, e ) => presenter.OnMouseUp( e );
        skControl.Resize += ( s, e ) => skControl.Invalidate();

        addRectBtn.Click += ( s, e ) => { presenter.AddShape( ShapeType.Rectangle ); skControl.Invalidate(); };
        addEllipseBtn.Click += ( s, e ) => { presenter.AddShape( ShapeType.Ellipse ); skControl.Invalidate(); };
        addTriBtn.Click += ( s, e ) => { presenter.AddShape( ShapeType.Triangle ); skControl.Invalidate(); };
        deleteBtn.Click += ( s, e ) => { presenter.DeleteSelected(); skControl.Invalidate(); };

        KeyPreview = true;
        this.KeyDown += MainForm_KeyDown;

        open.Click += ( s, e ) =>
        {
            var dlg = new OpenFileDialog { Filter = "JSON Files|*.json" };
            if ( dlg.ShowDialog() == DialogResult.OK )
            {
                presenter.LoadDocument( dlg.FileName );
                currentFile = dlg.FileName;
            }
        };

        save.Click += ( s, e ) =>
        {
            if ( currentFile == null )
            {
                saveAs.PerformClick();
                return;
            }
            presenter.SaveDocument( currentFile );
        };

        saveAs.Click += ( s, e ) =>
        {
            var dlg = new SaveFileDialog { Filter = "JSON Files|*.json" };
            if ( dlg.ShowDialog() == DialogResult.OK )
            {
                presenter.SaveDocument( dlg.FileName );
                currentFile = dlg.FileName;
            }
        };

        insertImage.Click += ( s, e ) =>
        {
            var dlg = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*" };
            if ( dlg.ShowDialog() == DialogResult.OK )
            {
                presenter.AddImageFromFile( dlg.FileName );
                skControl.Invalidate();
            }
        };

    }

    private void MainForm_KeyDown( object sender, KeyEventArgs e )
    {
        if ( e.KeyCode == Keys.Delete )
        {
            presenter.DeleteSelected();
            skControl.Invalidate();
        }

        if ( e.Control && e.KeyCode == Keys.Z )
        {
            presenter.Undo();
        }

        if ( e.Control && e.KeyCode == Keys.Y )
        {
            presenter.Redo();
        }
    }

    public void InvalidateCanvas() => skControl.Invalidate();
    public (int W, int H) GetCanvasSize() => (skControl.Width, skControl.Height);
}
