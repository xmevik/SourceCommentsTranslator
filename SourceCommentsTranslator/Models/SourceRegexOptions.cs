using SourceCommentsTranslator.Exceptions;
using System.Text.Json;

namespace SourceCommentsTranslator.Models
{
    public record SourceRegexOptions
    {
        /// <summary>
        /// Represents the options for regular expressions used in source code translation.
        /// </summary>
        public required IEnumerable<string> FileExtensions { get; set; }
        /// <summary>
        /// Gets or sets the regular expression pattern for single-line comments.
        /// </summary>
        public required string SingleLineComment { get; set; }
        /// <summary>
        /// Gets or sets the regular expression pattern for multi-line comments.
        /// </summary>
        public required MultiLineBrackets MultiLineComment { get; set; }

        /// <summary>
        /// Loads the regular expression options from a JSON file.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="sourceController">The source controller to read the JSON file.</param>
        /// <returns>An enumeration of <see cref="SourceRegexOptions"/>.</returns>
        /// <exception cref="RegexNotFoundException">Thrown when the JSON file is not found or cannot be deserialized.</exception>
        public static IEnumerable<SourceRegexOptions> LoadRegexOptions(string path)
        {
            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<IEnumerable<SourceRegexOptions>>(json) ?? throw new RegexNotFoundException(path, true);
        }
    }

    public record MultiLineBrackets
    {
        public required string InitialBracket { get; set; }
        public required string EndBracket { get; set; }
    }
}
