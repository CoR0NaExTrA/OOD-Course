namespace DocumentEditor.Utils;
// Простая утилита для HTML-escape
public static class HtmlUtil
{
    public static string Escape( string s )
    {
        if ( s == null )
            return "";
        return s.Replace( "&", "&amp;" )
                .Replace( "<", "&lt;" )
                .Replace( ">", "&gt;" )
                .Replace( "\"", "&quot;" )
                .Replace( "'", "&#39;" );
    }
}
