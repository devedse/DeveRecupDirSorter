using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeveRecupDirSorter.Helpers
{
    public static class DirectoryHelper
    {
        public static IEnumerable<string> FindAllFileNamesInDirectory(string directory)
        {
            return FindAllFilePathsInDirectory(directory).Select(t => Path.GetFileName(t));
        }

        public static IEnumerable<string> FindAllFilePathsInDirectory(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                yield return file;
            }

            foreach (var subDir in Directory.GetDirectories(directory))
            {
                var subFiles = Directory.GetFiles(subDir);
                foreach (var subFile in subFiles)
                {
                    yield return subFile;
                }
            }
        }
    }
}
