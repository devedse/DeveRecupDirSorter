using System;

namespace DeveRecupDirSorter.Helpers
{
    public static class PathHelper
    {
        public static string MakeRelativePath(string rootFullPath, string relativeFullPath)
        {
            var relativeFullUri = new Uri(relativeFullPath);
            var rootFullUri = new Uri(rootFullPath);

            var relativeUri = rootFullUri.MakeRelativeUri(relativeFullUri);

            return relativeUri.ToString();
        }
    }
}
