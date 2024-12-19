namespace SourceCommentsTranslator.FilesOperations
{
    public interface ISourceController
    {
        public string ReadSource(string path);
        public void WriteSource(string path, string? contents);
    }
}
