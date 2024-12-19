using NLog;
using NLog.Config;
using NLog.Targets;
using SourceCommentsTranslator.Models;
using SourceCommentsTranslator.Translators;
using SourceCommentsTranslator.FilesOperations;

namespace SourceCommentsTranslator
{
    internal class Program
    {
        static Logger _logger = null!;
        static Options Options = null!;
        static void Main(string[] args)
        {
            Options = Options.ParseOptions(args);

            BuildLogger();

            _logger.Debug("Setup dependencies");

            ITranslator translator;
            if (Options.IsTranslateWithLibre)
                translator = new LibreTranslate(Options.Source, Options.Destination, Options.ReTranslateUri);
            else
                translator = new GoogleTranslator(Options.Source, Options.Destination);

            DirectoryController directory = new FileDirectories(Options.IgnoreFolderNames);
            ISourceController source = new FileSource();
            _logger.Debug("Setup dependencies complete");

            _logger.Info("Start translating...");
            var sourceTranslator = new SourceCommentsTranslator(translator, source, directory, Options);

            sourceTranslator.Translate();
        }

        private static void BuildLogger()
        {
            AddNLogRules(Options.LogFilePath);

            _logger = LogManager.GetCurrentClassLogger();
        }

        private static void AddNLogRules(string? path)
        {
            var config = new LoggingConfiguration();
            string layout = "${longdate} [${level}] ${message} | ${exception:format=tostring}";

            var logfile = new FileTarget("logfile")
            {
                FileName = path,
                Layout = layout,
            };

            var logconsole = new ConsoleTarget("logconsole")
            {
                Layout = layout,
            };

            if (Options.Debug)
            {
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            }
            else
            {
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            }

            LogManager.Configuration = config;
        }
    }
}
