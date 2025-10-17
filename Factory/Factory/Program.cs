using Factory;
using Factory.Canvas;
using Factory.Designer;
using Factory.ShapeFactory;

class Program
{
    static void Main( string[] args )
    {
        string svgPath = args.Length > 0 ? args[ 0 ] : "output.svg";


        using ( var reader = Console.In )
        {
            var factory = new ShapeFactory();
            IDesigner designer = new Designer( factory );
            var draft = designer.CreateDraft( reader );


            using ( var canvas = new SvgCanvas( svgPath, 1000, 1000 ) )
            {
                var painter = new Painter();
                painter.DrawPicture( draft, canvas );
            }


            Console.WriteLine( $"SVG written to: {Path.GetFullPath( svgPath )}" );
        }
    }
}