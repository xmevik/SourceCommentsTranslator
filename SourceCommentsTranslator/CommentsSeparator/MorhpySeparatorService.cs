using SourceCommentsTranslator.Exceptions;
using SourceCommentsTranslator.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace SourceCommentsTranslator.CommentsSeparator
{
    /// <summary>
    /// This class is responsible for separating comments from source code based on specific regex patterns.
    /// </summary>
    public class MorhpySeparatorService
    {
        private readonly List<SourceRegexOptions> RegexOptions;
        private readonly string? RegexPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="MorhpySeparatorService"/> class.
        /// </summary>
        /// <param name="regexOptions">A list of <see cref="SourceRegexOptions"/> containing regex patterns for different file extensions.</param>
        /// <param name="regexPattern">An optional regex pattern to further filter the extracted comments.</param>
        public MorhpySeparatorService(List<SourceRegexOptions> regexOptions, string? regexPattern)
        {
            RegexOptions = regexOptions;
            RegexPattern = regexPattern;
        }

        /// <summary>
        /// Extracts comments from the given source code based on the specified file extension.
        /// </summary>
        /// <param name="code">The source code as a string.</param>
        /// <param name="fileExtension">The file extension of the source code.</param>
        /// <returns>A list of extracted comments as strings.</returns>
        public List<string> ExtractComments(string code, string fileExtension)
        {
            List<string> resultComments = new();
            List<string> tempComments = new();

            SourceRegexOptions? sourceRegex = RegexOptions.Find(x => x.FileExtensions.Contains(fileExtension)) ?? throw new RegexNotFoundException(fileExtension);

            if (sourceRegex.SingleLineComment.Length == 0 || sourceRegex.MultiLineComment.InitialBracket.Length == 0 || sourceRegex.MultiLineComment.EndBracket.Length == 0)
                throw new RegexNotFoundException(fileExtension);

            string singleLineComment = sourceRegex.SingleLineComment;
            string initialBracketComment = sourceRegex.MultiLineComment.InitialBracket;
            string endBracketComment = sourceRegex.MultiLineComment.EndBracket;

            int lenght = code.Length;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == singleLineComment[0] && IsLineComment(code, singleLineComment, i))
                {
                    tempComments.Add(GetSingleLineComment(code, i, ref i));
                }
                if (i < code.Length && code[i] == initialBracketComment[0] && IsLineComment(code, initialBracketComment, i))
                {
                    tempComments.Add(GetMultiLineComment(code, i, endBracketComment, ref i));
                }
            }

            foreach (var tempComment in tempComments)
                resultComments.AddRange(GetSeparatedComment(tempComment));

            if(RegexPattern is not null)
            {
                Regex regex = new(RegexPattern);
                resultComments = resultComments.Select(x => regex.Replace(x, "")).Where(x => x != "").ToList();
            }

            return resultComments.ToHashSet().ToList();
        }

        private static HashSet<string> GetSeparatedComment(string comment)
        {
            HashSet<string> result = new();
            StringBuilder commentBuilder = new(maxCapacity: comment.Length, capacity: comment.Length);
            foreach (var charComment in comment)
            {
                if (char.IsLetter(charComment) || charComment == ' ')
                {
                    commentBuilder.Append(charComment);
                }
                else
                {
                    var tempString = commentBuilder.ToString().Trim();
                    if (tempString != "" && tempString != " ")
                    {
                        result.Add(tempString);
                        commentBuilder.Clear();
                    }
                }
            }

            return result;
        }

        private static bool IsLineComment(string code, string lineCommentPattern, int i)
        {
            foreach (var charComment in lineCommentPattern)
            {
                if (charComment != code[i])
                    return false;
                i++;
            }
            return true;
        }

        private static string GetMultiLineComment(string code, int startId, string lastBracketCommentPattern, ref int lastId)
        {
            int endId = 0;

            for (int i = startId + lastBracketCommentPattern.Length; i < code.Length; i++)
            {
                char tempChar = code[i];
                if (tempChar == lastBracketCommentPattern.First() && IsLineComment(code, lastBracketCommentPattern, i))
                {
                    endId = i + lastBracketCommentPattern.Length;
                    break;
                }

            }

            lastId = endId;
            return code[startId..endId];
        }

        private static string GetSingleLineComment(string code, int startId, ref int lastId)
        {
            return GetMultiLineComment(code, startId, "\r\n", ref lastId);
        }
    }
}
