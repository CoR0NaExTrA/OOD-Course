using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledImage.Drawing;
public interface ITile : IDisposable, ICloneable
{
    const int SIZE = 8;

    char GetPixel( Point p );
    void SetPixel( Point p, char color );
}