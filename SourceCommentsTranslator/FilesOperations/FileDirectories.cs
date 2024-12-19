using NLog;

namespace SourceCommentsTranslator.FilesOperations
{
    public class FileDirectories : DirectoryController
    {
        private readonly Logger Logger;
        private readonly IEnumerable<string> IgnorableFolders;
        public FileDirectories(IEnumerable<string> ignorableFolders)
        {
            Logger = LogManager.GetCurrentClassLogger();
            IgnorableFolders = ignorableFolders;
        }
        public override string? GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public override string? GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public override string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public override string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public override HashSet<string> GetSourceFilesPaths(string path)
        {
            Logger.Info("Trying to get all sub directories");

            var allSubDirectories = GetSubdirectories(path);

            Logger.Info("Done");
            Logger.Debug("Subdirectories:\n" + string.Join('\n', allSubDirectories));

            HashSet<string> resultFiles = new(capacity: (int)(allSubDirectories.Count * 1.5));
            string[] tempFilesPaths;

            foreach (var subDir in allSubDirectories)
            {
                try
                {
                    tempFilesPaths = GetFiles(subDir, null, null);
                }
                catch (Exception ex) { Logger.Error(ex); continue; }

                foreach (var filePath in tempFilesPaths)
                    resultFiles.Add(filePath);
            }

            return resultFiles;
        }

        private HashSet<string> GetSubdirectories(string path)
        {
            List<string> directories = GetDirectories(path).ToList();
            Queue<string> subDirs = new(directories);

            string subDir;
            string[] tempDirs;

            while (subDirs.Count > 0)
            {
                subDir = subDirs.Dequeue();

                foreach (var ignoreFolder in IgnorableFolders)
                    if (subDir.Contains(ignoreFolder))
                        continue;

                try
                {
                    tempDirs = GetDirectories(subDir);
                }
                catch (Exception ex) { Logger.Error(ex); continue; }
                directories.AddRange(tempDirs);

                foreach (var tempDir in tempDirs)
                    subDirs.Enqueue(tempDir);
            }

            return directories.ToHashSet();
        }

        private static string[] GetFiles(string path, string? searchPattern, SearchOption? searchOption)
        {
            return Directory.GetFiles(path, searchPattern ?? string.Empty, searchOption ?? SearchOption.TopDirectoryOnly);
        }

        private static string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
