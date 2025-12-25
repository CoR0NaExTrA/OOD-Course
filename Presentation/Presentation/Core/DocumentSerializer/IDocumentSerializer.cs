using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Core.DocumentSerializer;
public interface IDocumentSerializer
{
    void Save( Document document, string path );
    Document Load( string path );
}
