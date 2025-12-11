using Presentation.Command;
using Presentation.Shapes;
using ShapesEditor;
using SkiaSharp;

namespace Presentation.Presenter;

public class CanvasPresenter
{
    private readonly ICanvasView view;
    private readonly Document doc;

    private Shape selected = null;
    private bool dragging = false;
    private bool resizing = false;
    private int activeHandle = -1;
    private SKPoint lastMouse;
    private SKPoint dragOffset;

    private SKRect dragBefore;
    private SKRect resizeBefore;

    private readonly UndoRedoManager history = new();


    public CanvasPresenter( ICanvasView view, Document doc )
    {
        this.view = view ?? throw new ArgumentNullException( nameof( view ) );
        this.doc = doc ?? throw new ArgumentNullException( nameof( doc ) );
    }

    public void AddShape( ShapeType type )
    {
        var (w, h) = view.GetCanvasSize();
        var cw = Math.Max( 100, w / 6 );
        var ch = Math.Max( 60, h / 8 );
        var rect = SKRect.Create( ( w - cw ) / 2f, ( h - ch ) / 2f, cw, ch );
        Shape s = type switch
        {
            ShapeType.Rectangle => new RectShape( rect ),
            ShapeType.Ellipse => new EllipseShape( rect ),
            ShapeType.Triangle => new TriangleShape( rect ),
            _ => new RectShape( rect )
        };
        var cmd = new AddShapeCommand( doc, s );
        history.Execute( cmd );
        DoCleanup();
        view.InvalidateCanvas();
        SelectShape( s );
    }

    public void DeleteSelected()
    {
        if ( selected == null )
            return;

        var cmd = new DeleteShapeCommand( doc, selected );
        history.Execute( cmd );
        selected = null;
        DoCleanup();
        view.InvalidateCanvas();
    }


    public void OnPaint( SKCanvas canvas, int width, int height )
    {
        canvas.Clear( SKColors.White );

        foreach ( var s in doc.Shapes )
        {
            s.Draw( canvas );
        }

        if ( selected != null )
        {
            selected.DrawSelection( canvas );
        }
    }

    public void OnMouseDown( MouseEventArgs e )
    {
        lastMouse = new SKPoint( e.X, e.Y );

        if ( selected != null )
        {
            var handles = selected.GetHandleCenters();
            for ( int i = 0; i < handles.Length; i++ )
            {
                if ( Distance( handles[ i ], lastMouse ) <= 8 )
                {
                    resizing = true;
                    activeHandle = i;
                    return;
                }
            }
        }

        var p = new SKPoint( e.X, e.Y );
        Shape hit = null;
        for ( int i = doc.Shapes.Count - 1; i >= 0; i-- )
        {
            if ( doc.Shapes[ i ].HitTest( p ) )
            {
                hit = doc.Shapes[ i ];
                break;
            }
        }

        if ( hit != null )
        {
            SelectShape( hit );
            dragging = true;
            dragOffset = new SKPoint( lastMouse.X - hit.Bounds.Left, lastMouse.Y - hit.Bounds.Top );

            dragBefore = hit.Bounds;
            resizeBefore = hit.Bounds;
        }
        else
        {
            DeselectAll();
        }
        view.InvalidateCanvas();
    }

    public void OnMouseMove( MouseEventArgs e )
    {
        var cur = new SKPoint( e.X, e.Y );
        var delta = new SKPoint( cur.X - lastMouse.X, cur.Y - lastMouse.Y );
        lastMouse = cur;

        if ( resizing && selected != null )
        {
            var canvasRect = new SKRect( 0, 0, view.GetCanvasSize().W, view.GetCanvasSize().H );
            selected.ResizeFromHandle( activeHandle, cur, canvasRect );
            view.InvalidateCanvas();
            return;
        }

        if ( dragging && selected != null )
        {
            var canvasRect = new SKRect( 0, 0, view.GetCanvasSize().W, view.GetCanvasSize().H );
            var newLeft = cur.X - dragOffset.X;
            var newTop = cur.Y - dragOffset.Y;
            var nb = SKRect.Create( newLeft, newTop, selected.Bounds.Width, selected.Bounds.Height );

            if ( nb.Left < 0 )
                nb.Offset( -nb.Left, 0 );
            if ( nb.Top < 0 )
                nb.Offset( 0, -nb.Top );
            if ( nb.Right > canvasRect.Width )
                nb.Offset( canvasRect.Width - nb.Right, 0 );
            if ( nb.Bottom > canvasRect.Height )
                nb.Offset( 0, canvasRect.Height - nb.Bottom );

            selected.Bounds = nb;
            view.InvalidateCanvas();
            return;
        }
    }

    public void OnMouseUp( MouseEventArgs e )
    {
        if ( dragging && selected != null )
        {
            dragging = false;
            var cmd = new MoveShapeCommand( selected, dragBefore, selected.Bounds );
            history.Execute( cmd );
        }

        if ( resizing && selected != null )
        {
            resizing = false;
            var cmd = new ResizeShapeCommand( selected, resizeBefore, selected.Bounds );
            history.Execute( cmd );
        }

        view.InvalidateCanvas();
    }


    private void SelectShape( Shape s )
    {
        foreach ( var sh in doc.Shapes )
            sh.IsSelected = false;
        selected = s;
        if ( selected != null )
            selected.IsSelected = true;
    }

    private void DeselectAll()
    {
        foreach ( var sh in doc.Shapes )
            sh.IsSelected = false;
        selected = null;
    }

    private float Distance( SKPoint a, SKPoint b )
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return ( float )Math.Sqrt( dx * dx + dy * dy );
    }

    public void AddImageFromFile( string filePath )
    {
        if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
            return;

        var id = Document.ImageRepo.AddImageFromFile( filePath );

        var (w, h) = view.GetCanvasSize();
        var width = Math.Min( 300, Math.Max( 100, w / 4 ) );
        var height = Math.Min( 300, Math.Max( 80, h / 6 ) );
        var rect = SKRect.Create( ( w - width ) / 2f, ( h - height ) / 2f, width, height );

        var shape = new ImageShape( rect, id, Path.GetFileName( filePath ) );
        var cmd = new AddImageCommand( doc, shape, filePath );
        history.Execute( cmd );
        SelectShape( shape );

        DoCleanup();
    }

    public void Undo()
    {
        history.Undo();
        selected = null;
        DoCleanup();
        view.InvalidateCanvas();
    }

    public void Redo()
    {
        history.Redo();
        selected = null;
        DoCleanup();
        view.InvalidateCanvas();
    }

    private void DoCleanup()
    {
        var used = doc.Shapes.OfType<ImageShape>().Select( s => s.ImageId ).Where( id => id != null ).ToList();
        Document.ImageRepo.CleanupUnused( used );
    }


    public void SaveDocument( string path )
    {
        doc.SaveToFile( path );
    }

    public void LoadDocument( string path )
    {
        doc.LoadFromFile( path );
        selected = null;
        view.InvalidateCanvas();
    }
}
