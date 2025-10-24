```mermaid
classDiagram
    class IShapeFactory {
        <<interface>>
        + CreateShape(descr:string):Shape
    }

    class IDesigner {
        <<interface>>
        + CreateDraft(strm):PictureDraft
    }

    class Designer {
        + CreateDraft(strm)
    }

    class PictureDraft {
        + GetShapeCount()
        + GetShape(index):Shape
    }

    class Shape {
        + Draw(canvas:ICanvas)
        + GetColor()
    }

    class Rectangle {
        + Draw(canvas)
        + GetLeftTop()
        + GetRightBottom()
    }

    class Triangle {
        + Draw(canvas)
        + GetVertex(1)
        + GetVertex(2)
        + GetVertex(3)
    }

    class Ellipse {
        + Draw(canvas)
        + GetCenter()
        + GetHorizontalRadius()
        + GetVerticalRadius()
    }

    class RegularPolygon {
        + Draw(canvas)
        + GetVertexCount()
        + GetCenter()
        + GetRadius()
    }

    class ICanvas {
        <<interface>>
        + SetColor(color)
        + DrawLine(from, to)
        + DrawEllipse(left, top, width, heigth)
    }

    class Client

    class Painter {
        + DrawPicture(draft, canvas)
    }

    class Color {
        <<enumeration>>
        + Green
        + Red
        + Blue
        + Yellow
        + Pink
        + Black
    }

    class SvgCanvas {
        - _width: int
        - _height: int
        - _currentColor: Color
        - _elements: List<string>
        + SetColor( c: Color ): void
        + ColorToSvg( c: Color ): string
        + DrawLine( from: Point, to: Point ): void
        + DrawEllipse( left: double, top: double, width: double, height: double ): void
    }

    class ShapeFactory {
        + createShape(descr):Shape
    }

    IShapeFactory <|.. ShapeFactory
    ICanvas <|.. SvgCanvas 
    ShapeFactory ..> Rectangle
    ShapeFactory ..> Triangle
    ShapeFactory ..> Ellipse
    ShapeFactory ..> RegularPolygon
    IShapeFactory ..> Shape
    Shape <|-- Rectangle 
    Shape <|-- Triangle
    Shape <|-- Ellipse
    Shape <|-- RegularPolygon
    Color --* Shape 
    Shape "0..*" --* "1" PictureDraft
    PictureDraft <.. Designer 
    PictureDraft <.. Painter
    IShapeFactory --* Designer
    IDesigner <|.. Designer
    ICanvas --* Client
    Color <.. ICanvas
    IDesigner <.. Client 
    Painter <.. Client
    ICanvas <.. Painter
    ICanvas <.. Shape 
```