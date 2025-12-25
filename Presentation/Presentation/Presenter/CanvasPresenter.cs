using Presentation.Command;
using Presentation.Core.DocumentSerializer;
using Presentation.Shapes;
using ShapesEditor;
using SkiaSharp;

namespace Presentation.Presenter;

public class CanvasPresenter
{
    private readonly ICanvasView view;
    private readonly Document doc;
    private readonly IDocumentSerializer serializer;

    private readonly List<Shape> selected = new();
    private bool dragging = false;
    private bool resizing = false;
    private int activeHandle = -1;
    private SKPoint lastMouse;

    private SKRect resizeBefore;
    private Dictionary<Shape, SKRect> dragBeforeMap = new();

    private readonly UndoRedoManager history = new();

    public CanvasPresenter( ICanvasView view, Document doc, IDocumentSerializer serializer )
    {
        this.view = view ?? throw new ArgumentNullException( nameof( view ) );
        this.doc = doc ?? throw new ArgumentNullException( nameof( doc ) );
        this.serializer = serializer ?? throw new ArgumentNullException( nameof( doc ) );
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
        SelectSingle( s );
    }

    public void DeleteSelected()
    {
        if ( selected.Count == 0 )
            return;

        history.Execute(
            new DeleteGroupCommand( doc, selected )
        );

        ClearSelection();
        view.InvalidateCanvas();
    }

    public void OnPaint( SKCanvas canvas, int width, int height )
    {
        canvas.Clear( SKColors.White );

        foreach ( var s in doc.Shapes )
        {
            s.Draw( canvas );
        }

        foreach ( var s in selected )
        {
            s.DrawSelection( canvas );
        }
    }

    public void OnMouseDown( MouseEventArgs e )
    {
        lastMouse = new SKPoint( e.X, e.Y );
        var p = new SKPoint( e.X, e.Y );

        if ( selected.Count == 1 )
        {
            var shape = selected[ 0 ];
            var handles = shape.GetHandleCenters();
            for ( int i = 0; i < handles.Length; i++ )
            {
                if ( Distance( handles[ i ], lastMouse ) <= 8 )
                {
                    resizing = true;
                    activeHandle = i;
                    resizeBefore = shape.Bounds;
                    return;
                }
            }
        }

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
            if ( ( Control.ModifierKeys & Keys.Control ) != 0 )
            {
                ToggleSelection( hit );
            }
            else
            {
                if ( !selected.Contains( hit ) )
                    SelectSingle( hit );
            }

            dragging = true;

            dragBeforeMap = selected.ToDictionary( s => s, s => s.Bounds );
        }
        else
        {
            ClearSelection();
        }


        view.InvalidateCanvas();
    }

    public void OnMouseMove( MouseEventArgs e )
    {
        var cur = new SKPoint( e.X, e.Y );
        var delta = new SKPoint( cur.X - lastMouse.X, cur.Y - lastMouse.Y );
        lastMouse = cur;

        if ( resizing && selected.Count == 1 )
        {
            var s = selected[ 0 ];
            var canvasRect = new SKRect(
                0, 0,
                view.GetCanvasSize().W,
                view.GetCanvasSize().H
            );

            s.ResizeFromHandle( activeHandle, cur, canvasRect );
            view.InvalidateCanvas();
            return;
        }

        if ( dragging && selected.Count > 0 )
        {
            var canvasRect = new SKRect( 0, 0,
                view.GetCanvasSize().W,
                view.GetCanvasSize().H );

            foreach ( var s in selected )
            {
                s.MoveBy( delta.X, delta.Y, canvasRect );
            }

            view.InvalidateCanvas();
        }
    }

    public void OnMouseUp( MouseEventArgs e )
    {
        if ( dragging && selected.Count > 0 )
        {
            dragging = false;

            foreach ( var s in selected )
            {
                history.Execute(
                    new MoveShapeCommand(
                        s,
                        dragBeforeMap[ s ],
                        s.Bounds
                    )
                );
            }
        }

        if ( resizing && selected.Count == 1 )
        {
            resizing = false;
            var s = selected[ 0 ];
            history.Execute(
                new ResizeShapeCommand( s, resizeBefore, s.Bounds )
            );
        }

        view.InvalidateCanvas();
    }

    private void ClearSelection()
    {
        selected.Clear();
    }

    private void SelectSingle( Shape s )
    {
        selected.Clear();
        if ( s != null )
            selected.Add( s );
    }

    private void ToggleSelection( Shape s )
    {
        if ( selected.Contains( s ) )
            selected.Remove( s );
        else
            selected.Add( s );
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

        var id = doc.ImageRepo.AddImageFromFile( filePath );

        var (w, h) = view.GetCanvasSize();
        var width = Math.Min( 300, Math.Max( 100, w / 4 ) );
        var height = Math.Min( 300, Math.Max( 80, h / 6 ) );
        var rect = SKRect.Create( ( w - width ) / 2f, ( h - height ) / 2f, width, height );

        var shape = new ImageShape( rect, doc.ImageRepo, id, Path.GetFileName( filePath ) );
        var cmd = new AddImageCommand( doc, shape, filePath );
        history.Execute( cmd );
        SelectSingle( shape );

        DoCleanup();
    }

    public void Undo()
    {
        history.Undo();
        ClearSelection();
        view.InvalidateCanvas();
    }

    public void Redo()
    {
        history.Redo();
        ClearSelection();
        view.InvalidateCanvas();
    }

    private void DoCleanup()
    {
        var used = doc.Shapes.OfType<ImageShape>().Select( s => s.ImageId ).Where( id => id != null ).ToList();
        doc.ImageRepo.CleanupUnused( used );
    }


    public void SaveDocument( string path )
    {
        serializer.Save( doc, path );
    }

    public void LoadDocument( string path )
    {
        history.Clear();
        var loaded = serializer.Load( path );
        doc.Shapes.Clear();
        doc.Shapes.AddRange( loaded.Shapes );
        view.InvalidateCanvas();
    }
}
