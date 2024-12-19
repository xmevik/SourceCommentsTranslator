using NLog;
using SourceCommentsTranslator.Models;
using SourceCommentsTranslator.Translators;
using SourceCommentsTranslator.FilesOperations;
using SourceCommentsTranslator.CommentsSeparator;

namespace SourceCommentsTranslator
{
    /// <summary>
    /// Represents a translator for source code comments.
    /// </summary>
    public class SourceCommentsTranslator
    {
        private readonly Logger Logger;
        private readonly Options Options;
        private readonly ITranslator Translator;
        private readonly DirectoryController Directory;
        private readonly ISourceController SourceController;
        private readonly List<SourceRegexOptions> RegexOptions;
        private readonly MorhpySeparatorService SeparatorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCommentsTranslator"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the translator.</param>
        /// <param name="directory">The directory controller for file and path operations.</param>
        /// <param name="translator">The translator to use for comment translation.</param>
        /// <param name="sourceController">The source controller for reading and writing source files.</param>
        public SourceCommentsTranslator(ITranslator translator, ISourceController sourceController, DirectoryController directory, Options options)
        {
            Options = options;
            Directory = directory;
            Translator = translator;
            SourceController = sourceController;
            Logger = LogManager.GetCurrentClassLogger();

            RegexOptions = SourceRegexOptions.LoadRegexOptions(Directory.GetFullPath(Options.RegexFilePath)).ToList();
            SeparatorService = new(RegexOptions, Options.SortCharsBy);
        }

        /// <summary>
        /// Translates comments in source files based on the configured options.
        /// </summary>
        public void Translate()
        {
            Logger.Info("Trying to get all supported source files in sub directories");
            HashSet<string> allSourceFiles = Directory.GetSourceFilesPaths(Options.Path);
            Logger.Info("Done");

            var allExtensions = RegexOptions.SelectMany(x => x.FileExtensions).Distinct();
            allSourceFiles = allSourceFiles.Where(filePath => allExtensions.Contains(Directory.GetFileExtension(filePath))).Order().ToHashSet();

            Logger.Debug("Source files:\n" + string.Join('\n', allSourceFiles));

            Logger.Info("Trying to get all comments in source files");
            IEnumerable<FileComments> filesComments = GetFileComments(allSourceFiles);
            Logger.Info("Done");

            if (Options.Debug)
                foreach (var fileComments in filesComments)
                    Logger.Debug("{Key}: \n|{Value)}", fileComments.FilePath, string.Join("\n|", fileComments.Comments));

            if (!Options.IsDontTranslate)
                TranslateAndReplace(filesComments);
        }

        /// <summary>
        /// Translates comments and replaces them in the source files.
        /// </summary>
        /// <param name="filesComments">An enumerable of FileComments containing file paths and their comments.</param>
        private void TranslateAndReplace(IEnumerable<FileComments> filesComments)
        {
            foreach (var fileComments in filesComments)
            {
                List<string> translatedComments = new();
                foreach (var comment in fileComments.Comments)
                {
                    try
                    {
                        Logger.Debug("Trying to translate {comment} in {filepath}", comment, fileComments.FilePath);
                        string translatedComment = Translator.Translate(comment);
                        translatedComments.Add(translatedComment);
                        Logger.Debug("Done: {translateComment}", translatedComment);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error translating comment: {comment}. This comment would be replaced with original.", e, comment);
                        translatedComments.Add(comment);
                    }
                }

                Logger.Debug("Trying to take source code");
                string code = SourceController.ReadSource(fileComments.FilePath);
                Logger.Debug("Done");

                Logger.Debug("Replacing comments in {fileComments}", fileComments.FilePath);
                for (int i = 0; i < translatedComments.Count; i++)
                {
                    code = code.Replace(fileComments.Comments[i], translatedComments[i]);
                }
                Logger.Debug("Done");

                Logger.Debug("Replacing original code with translated");
#pragma warning disable CS8604 // Perhaps the argument is a reference that allows a NULL value.
                SourceController.WriteSource(Path.Combine(Directory.GetDirectoryName(fileComments.FilePath), Options.FilePostscript + Directory.GetFileName(fileComments.FilePath)), code);
#pragma warning restore CS8604 // Perhaps the argument is a reference that allows a NULL value.
                Logger.Debug("Done");
            }
        }

        /// <summary>
        /// Extracts comments from the given file paths.
        /// </summary>
        /// <param name="filePaths">An enumerable of file paths to process.</param>
        /// <returns>A list of FileComments containing file paths and their extracted comments.</returns>
        private List<FileComments> GetFileComments(IEnumerable<string> filePaths)
        {
            List<FileComments> fileComments = new(capacity: filePaths.Count());

            foreach (var filePath in filePaths)
            {
                string code = SourceController.ReadSource(filePath);
                List<string> comments = SeparatorService.ExtractComments(code, Directory.GetFileExtension(filePath)).ToList();

                fileComments.Add(new() { FilePath = filePath, Comments = comments });
            }

            return fileComments;
        }
    }

}
