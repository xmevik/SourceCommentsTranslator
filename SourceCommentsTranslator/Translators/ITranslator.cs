namespace SourceCommentsTranslator.Translators
{
    public interface ITranslator : IDisposable
    {
        public string Translate(string text);
    }
}
