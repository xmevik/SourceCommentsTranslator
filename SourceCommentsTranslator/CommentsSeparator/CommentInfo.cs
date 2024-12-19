namespace SourceCommentsTranslator.CommentsSeparator
{
    public class CommentInfo
    {
        public required string Comment { get; set; }
        public CommentType CommentType { get; set; }
        public int StartId { get; set; }
        public int? EndId { get; set; }
        public string TranslatedComment { get; set; } = null!;

    }
}
