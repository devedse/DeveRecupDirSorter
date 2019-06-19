using DeveRecupDirSorter.Helpers;
using System;
using System.IO;

namespace DeveRecupDirSorter
{
    public class RecupFile
    {
        public string RelativePath { get; }
        public long Size { get; }
        public DateTime CreationTime { get; }
        public DateTime LastWriteTime { get; }
        public string Extension => Path.GetExtension(RelativePath).ToLowerInvariant();
        public string FileName => Path.GetFileName(RelativePath);

        public RecupFile(string sourcePath, string filePath)
        {
            RelativePath = PathHelper.MakeRelativePath(sourcePath, filePath);

            var fi = new FileInfo(filePath);
            Size = fi.Length;
            CreationTime = fi.CreationTime;
            LastWriteTime = fi.LastWriteTime;
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
