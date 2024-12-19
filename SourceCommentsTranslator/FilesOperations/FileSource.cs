namespace SourceCommentsTranslator.FilesOperations
{
    public class FileSource : ISourceController
    {
        public string ReadSource(string path)
        {
            var sourceCode = File.ReadAllText(path);

            return sourceCode;
        }

        public void WriteSource(string path, string? contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}
