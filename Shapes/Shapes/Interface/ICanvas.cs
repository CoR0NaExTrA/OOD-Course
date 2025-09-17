using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapes.Interface.gfx;
public interface ICanvas
{
    void SetColor( string color );
    void MoveTo( double x, double y );
    void LineTo( double x, double y );
    void DrawEllipse( double cx, double cy, double rx, double ry );
    void DrawText( double left, double top, double fontSize, string text );
}
