// File: Interfaces/IShape.cs
using Slides.Canvas;
using Slides.Models;
using Slides.Styles;

namespace Slides.Shapes;

public interface IShape
{
    Rect Frame { get; set; }
    LineStyle LineStyle { get; set; }
    FillStyle FillStyle { get; set; }
    void Draw( ICanvas canvas );
    event EventHandler FrameChanged;
    event EventHandler StyleChanged;
}