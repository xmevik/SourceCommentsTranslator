using SourceCommentsTranslator.CommentsSeparator;
using SourceCommentsTranslator.Exceptions;
using SourceCommentsTranslator.Models;
using System.Diagnostics;
using System.Text;

namespace SourceCommentsTranslator.UnitTests
{
    public class MorhpySeparatorServiceTests
    {
        private MorhpySeparatorService _separatorService;
        private List<SourceRegexOptions> _regexOptions;
        private string? _regexPattern;

        public MorhpySeparatorServiceTests()
        {
            _regexOptions = new List<SourceRegexOptions> { };
            _regexPattern = null;
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);
        }

        [Fact]
        public void ExtractComments_UnsupportedFileExtension_ReturnsEmptyList()
        {
            // Arrange
            string code = "Some code with // comments";
            string unsupportedExtension = ".unsupported";
            _regexOptions = new List<SourceRegexOptions>()
            {
                new(){ FileExtensions = new List<string> { ".cs", ".java" }, SingleLineComment = "", MultiLineComment = new() {InitialBracket = "", EndBracket = ""} }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            Assert.ThrowsAny<RegexNotFoundException>(() => _separatorService.ExtractComments(code, unsupportedExtension));
        }

        [Fact]
        public void ExtractComments_SupportedFileExtensionWithSingleLineComments_ReturnsCorrectComments()
        {
            // Arrange
            string code = "public class Test { // This is a single-line comment\r\n    int x = 5; // Another comment\r\n}";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>()
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("This is a single", result[0]);
            Assert.Equal("line comment", result[1]);
            Assert.Equal("Another comment", result[2]);
        }

        [Fact]
        public void ExtractComments_SupportedFileExtensionWithMultiLineComments_ReturnsCorrectComments()
        {
            // Arrange
            string code = @"
            public class Test
            {
                /* This is a
                   multi-line comment */
                int x = 5;
                /* Another
                   multi-line
                   comment */
            }";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
            Assert.Equal("This is a", result[0]);
            Assert.Equal("multi", result[1]);
            Assert.Equal("line comment", result[2]);
            Assert.Equal("Another", result[3]);
            Assert.Equal("line", result[4]);
            Assert.Equal("comment", result[5]);
        }

        [Fact]
        public void ExtractComments_CodeWithNoComments_ReturnsEmptyList()
        {
            // Arrange
            string code = "public class Test { int x = 5; }";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ExtractComments_WithRegexPattern_FiltersComments()
        {
            // Arrange
            string code = @"
            // This is a single-line comment\r\n
            /* This is a
               multi-line comment */
            // 这是中文注释
            /* 这是多行
               中文注释 */
            ";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _regexPattern = @"[^\u4E00-\u9FFF]";  // Matches Chinese characters
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("这是中文注释", result[0]);
            Assert.Equal("这是多行", result[1]);
            Assert.Equal("中文注释", result[2]);
        }

        [Fact]
        public void ExtractComments_MixedSingleLineAndMultiLineComments_ReturnsAllComments()
        {
            // Arrange
            string code = @"
            // This is a single-line comment
            public class Test
            {
                /* This is a
                   multi-line comment */
                int x = 5; // Another single-line comment
                /* Another
                   multi-line
                   comment */
            }";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Count);
            Assert.Equal("This is a single", result[0]);
            Assert.Equal("line comment", result[1]);
            Assert.Equal("This is a", result[2]);
            Assert.Equal("multi", result[3]);
            Assert.Equal("Another single", result[4]);
            Assert.Equal("Another", result[5]);
            Assert.Equal("line", result[6]);
            Assert.Equal("comment", result[7]);
        }

        [Fact]
        public void ExtractComments_CommentsWithSpecialCharactersAndPunctuation_ReturnsProcessedComments()
        {
            // Arrange
            string code = @"
            // This is a comment with special characters: @#$%^&*()
            /* Multi-line comment
               with punctuation: .,!?<>|\/
               and more special chars: ~`+=-_ */
            int x = 5; // Another comment, with commas!
            ";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
            Assert.Equal("This is a comment with special characters", result[0]);
            Assert.Equal("Multi", result[1]);
            Assert.Equal("line comment", result[2]);
            Assert.Equal("with punctuation", result[3]);
            Assert.Equal("and more special chars", result[4]);
            Assert.Equal("Another comment", result[5]);
            Assert.Equal("with commas", result[6]);
        }

        [Fact]
        public void ExtractComments_LargeCodeInput_HandlesWithoutPerformanceIssues()
        {
            // Arrange
            const int codeSize = 10_000_000; // 10 million characters
            StringBuilder largeCode = new(codeSize);
            for (int i = 0; i < codeSize / 100; i++)
            {
                largeCode.Append("public void Method() { /* Comment */ } // Line comment\r\n");
            }
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var result = _separatorService.ExtractComments(largeCode.ToString(), supportedExtension);
            stopwatch.Stop();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"Extraction took {stopwatch.ElapsedMilliseconds}ms, which is longer than the 5000ms threshold");
        }

        [Fact]
        public void ExtractComments_NestedCommentStructures_ReturnsCorrectComments()
        {
            // Arrange
            string code = @"
            public class Test
            {
                // Outer single-line comment /* with nested multi-line comment */
                /* Outer multi-line comment
                   // with nested single-line comment
                   and more content */
                int x = 5;
                /* Another /* nested */ (Not a comment)multi-line comment */
            }";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Count);
            Assert.Equal("Outer single", result[0]);
            Assert.Equal("line comment", result[1]);
            Assert.Equal("with nested multi", result[2]);
            Assert.Equal("Outer multi", result[3]);
            Assert.Equal("with nested single", result[4]);
            Assert.Equal("and more content", result[5]);
            Assert.Equal("Another", result[6]);
            Assert.Equal("nested", result[7]);
        }

        [Fact]
        public void ExtractComments_UnicodeAndCJKCharacters_ReturnsCorrectComments()
        {
            // Arrange
            string code = @"
            // This is a comment with Unicode: 你好世界
            /* Multi-line comment
               with CJK characters: 日本語 */
            int x = 5; // Another comment with Korean: 안녕하세요
            ";
            string supportedExtension = ".cs";
            _regexOptions = new List<SourceRegexOptions>
            {
                new()
                {
                    FileExtensions = new List<string> { ".cs" },
                    SingleLineComment = "//",
                    MultiLineComment = new() {InitialBracket = "/*", EndBracket = "*/"}
                }
            };
            _regexPattern = @"[^\u4E00-\u9FFF\u3040-\u30FF\u3130-\u318F\uAC00-\uD7AF]+";  // Matches CJK characters
            _separatorService = new MorhpySeparatorService(_regexOptions, _regexPattern);

            // Act
            var result = _separatorService.ExtractComments(code, supportedExtension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("你好世界", result[0]);
            Assert.Equal("日本語", result[1]);
            Assert.Equal("안녕하세요", result[2]);
        }
    }
}