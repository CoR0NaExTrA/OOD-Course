namespace ShapesEditor;

public interface ICanvasView
{
    void InvalidateCanvas();
    (int W, int H) GetCanvasSize();
}

