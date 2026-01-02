namespace Presentation.Core
{
    public class DocumentContext
    {
        public Document document { get; private set; }

        public event Action documentChanged;

        public void Open(Document doc)
        {
            document = doc;
            documentChanged?.Invoke();
        }
    }

}
