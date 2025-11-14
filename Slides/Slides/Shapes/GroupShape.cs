// File: Shapes/GroupShape.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public class GroupShape : IShape
{
    private readonly List<IShape> _children = new List<IShape>();
    private Rect _frameCache = Rect.Empty;

    public Rect Frame
    {
        get
        {
            if ( _children.Count == 0 )
            {
                return Rect.Empty;
            }
            var left = _children.Min( c => c.Frame.Left );
            var top = _children.Min( c => c.Frame.Top );
            var right = _children.Max( c => c.Frame.Right );
            var bottom = _children.Max( c => c.Frame.Bottom );
            _frameCache = new Rect( left, top, right - left, bottom - top );
            return _frameCache;
        }
        set
        {
            var old = this.Frame;
            var newRect = value;
            if ( _children.Count == 0 )
            { 
                _frameCache = newRect; 
                FrameChanged?.Invoke( this, EventArgs.Empty ); 
                return; 
            }
            double oldW = old.Width;
            double oldH = old.Height;
            double newW = newRect.Width;
            double newH = newRect.Height;
            foreach ( var child in _children )
            {
                var cf = child.Frame;
                double relX = ( cf.Left - old.Left );
                double relY = ( cf.Top - old.Top );
                double nx, ny, nw, nh;
                if ( oldW != 0 )
                { 
                    double sx = newW / oldW; nx = newRect.Left + relX * sx; nw = cf.Width * sx; 
                }
                else
                { 
                    nw = cf.Width; nx = newRect.Left + ( newW - nw ) / 2.0; 
                }
                if ( oldH != 0 )
                { 
                    double sy = newH / oldH; ny = newRect.Top + relY * sy; nh = cf.Height * sy; 
                }
                else
                { 
                    nh = cf.Height; ny = newRect.Top + ( newH - nh ) / 2.0; 
                }
                child.Frame = new Rect( nx, ny, nw, nh );
            }
            _frameCache = newRect;
            FrameChanged?.Invoke( this, EventArgs.Empty );
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            if ( _children.Count == 0 )
            {
                return null;
            }
            LineStyle first = _children[ 0 ].LineStyle;
            if ( first == null )
            { 
                return null;
            }
            foreach ( var c in _children )
            {
                if ( c.LineStyle == null || !c.LineStyle.Equals( first ) )
                { 
                    return null;
                }
            }
            return first.Clone();
        }
        set
        {
            if ( value == null )
            {
                return;
            }
            foreach ( var c in _children )
            {
                c.LineStyle = value.Clone();
            }
            StyleChanged?.Invoke( this, EventArgs.Empty );
        }
    }

    public FillStyle FillStyle
    {
        get
        {
            if ( _children.Count == 0 )
            {
                return null;
            }
            FillStyle first = _children[ 0 ].FillStyle;
            if ( first == null )
            {
                return null;
            }
            foreach ( var c in _children )
            {
                if ( c.FillStyle == null || !c.FillStyle.Equals( first ) ) 
                {
                    return null;
                }
            }
            return first.Clone();
        }
        set
        {
            if ( value == null )
            {
                return;
            }
            foreach ( var c in _children )
            {
                c.FillStyle = value.Clone();
            }
            StyleChanged?.Invoke( this, EventArgs.Empty );
        }
    }

    public void Draw( ICanvas canvas ) 
    { 
        foreach ( var c in _children ) c.Draw( canvas ); 
    }

    public event EventHandler FrameChanged; 
    public event EventHandler StyleChanged;

    public void Add( IShape shape ) 
    {
        if ( shape == null )
        {
            throw new ArgumentNullException( nameof( shape ) );
        }
        _children.Add( shape ); 
        shape.FrameChanged += Child_FrameOrStyleChanged;
        shape.StyleChanged += Child_FrameOrStyleChanged;
        FrameChanged?.Invoke( this, EventArgs.Empty ); 
        StyleChanged?.Invoke( this, EventArgs.Empty ); 
    }

    public void Remove( IShape shape )
    {
        if ( shape == null )
        {
            return; 
        }
        if ( _children.Remove( shape ) ) 
        { 
            shape.FrameChanged -= Child_FrameOrStyleChanged; 
            shape.StyleChanged -= Child_FrameOrStyleChanged; 
            FrameChanged?.Invoke( this, EventArgs.Empty ); 
            StyleChanged?.Invoke( this, EventArgs.Empty ); 
        } 
    }

    private void Child_FrameOrStyleChanged( object sender, EventArgs e ) 
    { 
        _frameCache = Rect.Empty; 
        FrameChanged?.Invoke( this, EventArgs.Empty ); 
        StyleChanged?.Invoke( this, EventArgs.Empty ); 
    }
}