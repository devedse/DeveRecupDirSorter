using DeveRecupDirSorter.Helpers;
using ExifLibrary;
using System;
using System.IO;
using System.Linq;

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

        public Lazy<string> DesiredFileName { get; }
        public Lazy<string> DesiredDirName { get; }

        public string FilePath { get; }

        public RecupFile(string sourcePath, string filePath)
        {
            RelativePath = PathHelper.MakeRelativePath(sourcePath, filePath);

            var fi = new FileInfo(filePath);
            Size = fi.Length;
            CreationTime = fi.CreationTime;
            LastWriteTime = fi.LastWriteTime;
            FilePath = filePath;

            DesiredFileName = new Lazy<string>(() => GetDesiredFileName());
            DesiredDirName = new Lazy<string>(() => GetDesiredDirName());
        }

        public override string ToString()
        {
            return RelativePath;
        }

        private DateTime? FindDateTime()
        {
            try
            {
                var imageFile = ImageFile.FromFile(FilePath);

                ExifProperty exifProperty;

                exifProperty = imageFile.Properties.FirstOrDefault(t => t.Tag == ExifTag.DateTime);
                if (exifProperty != null)
                {
                    return (DateTime)exifProperty.Value;
                }

                exifProperty = imageFile.Properties.FirstOrDefault(t => t.Tag == ExifTag.DateTimeOriginal);
                if (exifProperty != null)
                {
                    return (DateTime)exifProperty.Value;
                }

                exifProperty = imageFile.Properties.FirstOrDefault(t => t.Tag == ExifTag.DateTimeDigitized);
                if (exifProperty != null)
                {
                    return (DateTime)exifProperty.Value;
                }

                exifProperty = imageFile.Properties.FirstOrDefault(t => t.Tag == ExifTag.ThumbnailDateTime);
                if (exifProperty != null)
                {
                    return (DateTime)exifProperty.Value;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        private string GetDesiredFileName()
        {
            if (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".tiff")
            {
                var dt = FindDateTime();
                if (dt != null)
                {
                    var dtValue = dt.Value;
                    return $"{dtValue.Hour.ToString().PadLeft(2, '0')}-{dtValue.Minute.ToString().PadLeft(2, '0')}-{FileName}";
                }
            }
            return FileName;
        }

        private string GetDesiredDirName()
        {
            if (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".tiff")
            {
                var dt = FindDateTime();
                if (dt != null)
                {
                    var dtValue = dt.Value;
                    return $"{dtValue.Year}-{dtValue.Month.ToString().PadLeft(2, '0')}-{dtValue.Day.ToString().PadLeft(2, '0')}";
                }
            }
            return null;
        }
    }
}
