namespace SourceCommentsTranslator.Models
{
    public record FileComments
    {
        public required string FilePath { get; set; }

        public IList<string> Comments { get; set; } = null!;
    }
}
