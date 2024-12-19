namespace SourceCommentsTranslator.Exceptions
{
    public class RegexNotFoundException : Exception
    {
        public RegexNotFoundException(string fileExtension) : base($"The comments settings were not found, check the comment settings for {fileExtension} files")
        {
        }

        public RegexNotFoundException(string path, bool _) : base($"The comments settings were not found, check for setting file: {path}")
        {
        }
    }
}
