// File: Program.cs

using Slides.Canvas;
using Slides.Models;
using Slides.Shapes;
using Slides.Styles;

class Program
{
    static void Main()
    {
        var slide = new Slide();

        // Стили
        var blackLine = new LineStyle( true, new ColorRGBA( 0, 0, 0, 255 ), 2.0 );
        var thinLine = new LineStyle( true, new ColorRGBA( 0, 0, 0, 255 ), 1.0 );
        var darkBrown = new FillStyle( true, new ColorRGBA( 101, 67, 33, 255 ) );
        var redFill = new FillStyle( true, new ColorRGBA( 200, 50, 50, 255 ) );
        var green = new FillStyle( true, new ColorRGBA( 40, 160, 60, 255 ) );
        var glass = new FillStyle( true, new ColorRGBA( 180, 220, 250, 200 ) );

        // Дом
        var houseBody = new RectangleShape( new Rect( 100, 200, 200, 150 ), blackLine.Clone(), darkBrown.Clone() );
        var roof = new TriangleShape( new Rect( 90, 120, 220, 100 ), blackLine.Clone(), redFill.Clone() );
        var door = new RectangleShape( new Rect( 180, 270, 40, 80 ), blackLine.Clone(), new FillStyle( true, new ColorRGBA( 120, 60, 20 ) ) );
        var window1 = new RectangleShape( new Rect( 120, 230, 40, 40 ), thinLine.Clone(), glass.Clone() );
        var window2 = new RectangleShape( new Rect( 240, 230, 40, 40 ), thinLine.Clone(), glass.Clone() );
        var houseGroup = new GroupShape();
        houseGroup.Add( roof );
        houseGroup.Add( houseBody );
        houseGroup.Add( door );
        houseGroup.Add( window1 );
        houseGroup.Add( window2 );

        // Дерево
        var trunk = new RectangleShape( new Rect( 330, 230, 30, 120 ), blackLine.Clone(), new FillStyle( true, new ColorRGBA( 101, 67, 33 ) ) );
        var crown1 = new EllipseShape( new Rect( 300, 180, 90, 70 ), thinLine.Clone(), green.Clone() );
        var crown2 = new EllipseShape( new Rect( 320, 140, 90, 70 ), thinLine.Clone(), green.Clone() );
        var crownGroup = new GroupShape();
        crownGroup.Add( crown1 );
        crownGroup.Add( crown2 );
        var treeGroup = new GroupShape();
        treeGroup.Add( trunk );
        treeGroup.Add( crownGroup );

        // Земля
        var ground = new RectangleShape( new Rect( 0, 340, 800, 160 ), new LineStyle( false, new ColorRGBA( 0, 0, 0 ) ), new FillStyle( true, new ColorRGBA( 100, 200, 100 ) ) );

        slide.AddShape( ground );
        slide.AddShape( houseGroup );
        slide.AddShape( treeGroup );

        var svg = new SvgCanvas( 800, 600 );
        slide.Draw( svg );
        var outPath = Path.Combine( Directory.GetCurrentDirectory(), "slide.svg" );
        svg.Save( outPath );
        Console.WriteLine( $"SVG saved to: {outPath}" );

        // Применяем стиль ко всей группе дома
        houseGroup.LineStyle = new LineStyle( true, new ColorRGBA( 0, 0, 150 ), 10.0 );

        // Масштабируем дом
        var oldFrame = houseGroup.Frame;
        var newFrame = new Rect( oldFrame.X - 20, oldFrame.Y - 20, oldFrame.Width * 1.2, oldFrame.Height * 1.2 );
        houseGroup.Frame = newFrame;

        var consoleCanvas = new ConsoleCanvas();
        Console.WriteLine( "---- Drawing to console canvas (debug) ----" );
        slide.Draw( consoleCanvas );

        var svg1 = new SvgCanvas( 800, 600 );
        slide.Draw( svg1 );
        var outPath1 = Path.Combine( Directory.GetCurrentDirectory(), "slide1.svg" );
        svg1.Save( outPath1 );
        Console.WriteLine( $"SVG saved to: {outPath1}" );
    }
}
