using CommandLine;

namespace SourceCommentsTranslator.Models
{
    public class Options
    {
        [Option(longName: "path", HelpText = "The absolute path to your project", Required = true)]
        public required string Path { get; set; }

        [Option(longName: "src", HelpText = "The original language of the comments", Default = "auto")]
        public required string Source { get; set; } = "auto";

        [Option(longName: "dest", HelpText = "The destination language of the comments", Default = "en")]
        public required string Destination { get; set; } = "en";

        [Option(longName: "postscript", HelpText = "Creates new file with [[postscript][original file name]] instead of updating file")]
        public string FilePostscript { get; set; } = string.Empty;

        [Option(longName: "regpath", HelpText = "The path to the regex settings file", Default = "./CommentRegex.json")]
        public required string RegexFilePath { get; set; } = "./CommentRegex.json";

        [Option(longName: "logpath", HelpText = "The path to the log file", Default = "./temp.log")]
        public required string LogFilePath { get; set; } = "./temp.log";

        [Option(longName: "debug", HelpText = "Determines whether debugging information needs to be include", Default = false)]
        public required bool Debug { get; set; } = false;

        [Option(longName: "donttranslate", HelpText = "Determines whether to translate or not", Default = false)]
        public required bool IsDontTranslate { get; set; } = false;

        [Option(shortName: 'g', longName: "libretranslate", HelpText = "If you want to use the Libre Translate Api", Default = false)]
        public required bool IsTranslateWithLibre { get; set; } = false;

        [Option(shortName: 'r', longName: "libretranslateuri", HelpText = "Uri to the end path of LibreTranslate", Default = "http://localhost:5000/translate")]
        public required string ReTranslateUri { get; set; } = "http://localhost:5000/translate";

        [Option(shortName: 'i', longName: "ignorefolders", HelpText = "If you want to specify folders to ignore", Separator = ',')]
        public required IEnumerable<string> IgnoreFolderNames { get; set; } = ["obj", "bin"];

        [Option(longName: "sortcharsby", HelpText = "If you want only certain characters to be translated(only the Regex pattern is accepted)", Default = null)]
        public string? SortCharsBy { get; set; } = null;

        public static Options ParseOptions(string[] args)
        {
            var resultArgs = Parser.Default.ParseArguments<Options>(args);

            if (resultArgs.Errors.Any())
                Environment.Exit(0);

            return resultArgs.Value;
        }
    }
}
