namespace Slides_tests;

using Slides.Canvas;
using Slides.Models;
using Slides.Shapes;
using Slides.Styles;
using System.Collections.Generic;

public class FakeCanvas : ICanvas
{
    public List<string> Calls { get; } = new();

    public void SetFillColor( ColorRGBA color ) =>
        Calls.Add( $"SetFillColor {color}" );

    public void SetLineColor( ColorRGBA color ) =>
        Calls.Add( $"SetLineColor {color}" );

    public void SetLineThickness( double thickness ) =>
        Calls.Add( $"SetLineThickness {thickness}" );

    public void DrawLine( double x1, double y1, double x2, double y2 ) =>
        Calls.Add( $"DrawLine {x1},{y1}->{x2},{y2}" );

    public void DrawEllipse( Rect frame ) =>
        Calls.Add( $"DrawEllipse {frame}" );

    public void FillEllipse( Rect frame ) =>
        Calls.Add( $"FillEllipse {frame}" );

    public void FillPolygon( (double x, double y)[] points ) =>
        Calls.Add( "FillPolygon " + string.Join( " ", points ) );
}

public class RectangleShapeTests
{
    [Fact]
    public void Rectangle_Draws_Fill_And_Lines()
    {
        var canvas = new FakeCanvas();
        var rect = new RectangleShape(
            new Rect( 10, 20, 30, 40 ),
            new LineStyle( true, new ColorRGBA( 1, 2, 3 ), 2 ),
            new FillStyle( true, new ColorRGBA( 10, 20, 30 ) )
        );

        rect.Draw( canvas );

        Assert.Contains( "SetFillColor rgba(10,20,30,255)", canvas.Calls );
        Assert.Contains( "FillPolygon (10, 20) (40, 20) (40, 60) (10, 60)", canvas.Calls );
        Assert.Contains( "SetLineColor rgba(1,2,3,255)", canvas.Calls );
        Assert.Contains( "SetLineThickness 2", canvas.Calls );
        Assert.True( canvas.Calls.Exists( c => c.StartsWith( "DrawLine" ) ) );
    }
}

public class EllipseShapeTests
{
    [Fact]
    public void Ellipse_Draws_Fill_And_Stroke()
    {
        var canvas = new FakeCanvas();
        var ellipse = new EllipseShape(
            new Rect( 0, 0, 100, 50 ),
            new LineStyle( true, new ColorRGBA( 0, 0, 0 ), 1 ),
            new FillStyle( true, new ColorRGBA( 255, 0, 0 ) )
        );

        ellipse.Draw( canvas );

        Assert.Contains( "SetFillColor rgba(255,0,0,255)", canvas.Calls );
        Assert.Contains( "FillEllipse Rect(X=0,Y=0,W=100,H=50)", canvas.Calls );
        Assert.Contains( "DrawEllipse Rect(X=0,Y=0,W=100,H=50)", canvas.Calls );
    }
}

public class TriangleShapeTests
{
    [Fact]
    public void Triangle_Draws_Filled_And_Stroked()
    {
        var canvas = new FakeCanvas();
        var triangle = new TriangleShape(
            new Rect( 10, 10, 20, 20 ),
            new LineStyle( true, new ColorRGBA( 0, 0, 0 ), 1 ),
            new FillStyle( true, new ColorRGBA( 0, 255, 0 ) )
        );

        triangle.Draw( canvas );

        Assert.Contains( "SetFillColor rgba(0,255,0,255)", canvas.Calls );
        Assert.Contains( canvas.Calls, c => c.Contains( "FillPolygon" ) );
        Assert.True( canvas.Calls.Exists( c => c.StartsWith( "DrawLine" ) ) );
    }
}

//GroupShape

public class GroupShapeFrameTests
{
    [Fact]
    public void Group_Frame_Is_BoundingBox_Of_Children()
    {
        var g = new GroupShape();
        g.Add( new RectangleShape( new Rect( 10, 10, 50, 20 ), new LineStyle(), new FillStyle() ) );
        g.Add( new RectangleShape( new Rect( 40, 5, 20, 100 ), new LineStyle(), new FillStyle() ) );

        var f = g.Frame;

        Assert.Equal( 10, f.Left );
        Assert.Equal( 5, f.Top );
        Assert.Equal( 50, f.Width );  // from x=10 to x=60
        Assert.Equal( 100, f.Height );
    }
}

public class GroupShapeScalingTests
{
    [Fact]
    public void Setting_Group_Frame_Scales_Children()
    {
        var g = new GroupShape();

        var child = new RectangleShape( new Rect( 10, 10, 20, 20 ),
            new LineStyle(), new FillStyle() );

        g.Add( child );

        g.Frame = new Rect( 0, 0, 200, 200 );

        Assert.Equal( 0, child.Frame.Left, 0.001 );
        Assert.Equal( 0, child.Frame.Top, 0.001 );
        Assert.Equal( 200, child.Frame.Width, 0.001 );
    }
}

public class GroupStyleTests
{
    [Fact]
    public void Setting_LineStyle_Propagates_To_Children()
    {
        var g = new GroupShape();

        var s1 = new RectangleShape( new Rect(), new LineStyle(), new FillStyle() );
        var s2 = new RectangleShape( new Rect(), new LineStyle(), new FillStyle() );

        g.Add( s1 );
        g.Add( s2 );

        var style = new LineStyle( true, new ColorRGBA( 10, 10, 10 ), 5 );
        g.LineStyle = style;

        Assert.Equal( style, s1.LineStyle );
        Assert.Equal( style, s2.LineStyle );
    }

    [Fact]
    public void Group_LineStyle_Returns_Null_If_Children_Differ()
    {
        var g = new GroupShape();

        g.Add( new RectangleShape( new Rect(), new LineStyle( true, new ColorRGBA( 1, 1, 1 ) ), new FillStyle() ) );
        g.Add( new RectangleShape( new Rect(), new LineStyle( true, new ColorRGBA( 2, 2, 2 ) ), new FillStyle() ) );

        Assert.Null( g.LineStyle );
    }
}

public class GroupShapeEventsTests
{
    [Fact]
    public void Group_Raises_Events_When_Child_Frame_Changes()
    {
        var g = new GroupShape();
        var child = new RectangleShape( new Rect( 0, 0, 10, 10 ), new LineStyle(), new FillStyle() );

        g.Add( child );

        bool frameRaised = false;
        g.FrameChanged += ( _, _ ) => frameRaised = true;

        child.Frame = new Rect( 5, 5, 10, 10 );

        Assert.True( frameRaised );
    }
}

public class SlideTests
{
    [Fact]
    public void Slide_Draws_All_Shapes()
    {
        var canvas = new FakeCanvas();
        var slide = new Slide();

        slide.AddShape( new RectangleShape( new Rect( 0, 0, 10, 10 ),
            new LineStyle(), new FillStyle() ) );

        slide.AddShape( new EllipseShape( new Rect( 0, 0, 10, 10 ),
            new LineStyle(), new FillStyle() ) );

        slide.Draw( canvas );

        Assert.True( canvas.Calls.Exists( c => c.Contains( "DrawLine" ) ) );
        Assert.True( canvas.Calls.Exists( c => c.Contains( "DrawEllipse" ) ) );
    }
}
