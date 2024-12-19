namespace SourceCommentsTranslator.FilesOperations
{
    public abstract class DirectoryController
    {
        public abstract string? GetDirectoryName(string path);
        public abstract string? GetFileName(string path);
        public abstract string GetFullPath(string path);
        public abstract string GetFileExtension(string filePath);

        public virtual HashSet<string> GetSourceFilesPaths(string path)
        {
            List<string> directories = GetDirectories(path).ToList();
            Queue<string> subDirs = new(directories);

            string[] tempDirs;

            while (subDirs.Count > 0)
            {
                string subDir = subDirs.Dequeue();

                try
                {
                    tempDirs = GetDirectories(subDir);
                }
                catch { continue; }
                directories.AddRange(tempDirs);

                foreach (var tempDir in tempDirs)
                    subDirs.Enqueue(tempDir);
            }

            var allSubDirectories = directories.ToHashSet();

            HashSet<string> resultFiles = new(capacity: (int)(allSubDirectories.Count * 1.5));
            string[] tempFilesPaths;

            foreach (var subDir in allSubDirectories)
            {
                try
                {
                    tempFilesPaths = GetFiles(subDir, null, null);
                }
                catch { continue; }

                foreach (var filePath in tempFilesPaths)
                    resultFiles.Add(filePath);
            }

            return resultFiles;
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
