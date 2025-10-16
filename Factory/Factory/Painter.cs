namespace Factory;
public class Painter
{
    public void DrawPicture( PictureDraft draft, ICanvas canvas )
    {
        foreach ( var s in draft.Shapes )
        {
            s.Draw( canvas );
        }
    }
}
