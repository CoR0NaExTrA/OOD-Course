classDiagram
direction LR

class ICanvas {
  <<interface>>
  +MoveTo(x: int, y: int)
  +LineTo(x: int, y: int)
}

class CCanvas {
  +MoveTo(x: int, y: int)
  +LineTo(x: int, y: int)
}

class CModernGraphicsRenderer {
  +BeginDraw()
  +DrawLine(start: CPoint, end: CPoint)
  +EndDraw()
}

class CPoint {
  +x: int
  +y: int
}

class ModernGraphicsRendererAdapter {
  -m_renderer: CModernGraphicsRenderer
  -m_currentPoint: CPoint
  +MoveTo(x: int, y: int)
  +LineTo(x: int, y: int)
  +Dispose()
}

class ICanvasDrawable {
  <<interface>>
  +Draw(canvas: ICanvas)
}

class CTriangle {
  -m_p1: Point
  -m_p2: Point
  -m_p3: Point
  +Draw(canvas: ICanvas)
}

class CRectangle {
  -m_leftTop: Point
  -m_width: int
  -m_height: int
  +Draw(canvas: ICanvas)
}

class CCanvasPainter {
  -m_canvas: ICanvas
  +Draw(drawable: ICanvasDrawable)
}

class Point {
  +x: int
  +y: int
}

ICanvas <|.. CCanvas
ICanvas <|.. ModernGraphicsRendererAdapter
ICanvasDrawable <|.. CTriangle
ICanvasDrawable <|.. CRectangle
CCanvasPainter --> ICanvas
CCanvasPainter --> ICanvasDrawable
ModernGraphicsRendererAdapter --> CModernGraphicsRenderer
ModernGraphicsRendererAdapter --> CPoint
CTriangle --> Point
CRectangle --> Point
