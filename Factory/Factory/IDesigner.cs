namespace Factory;
public interface IDesigner
{
    PictureDraft CreateDraft( TextReader strm );
}
