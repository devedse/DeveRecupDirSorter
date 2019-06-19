using DeveCoolLib.Conversion;
using DeveCoolLib.TextFormatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DeveRecupDirSorter
{
    public class RecupDirSorter
    {
        public string RootRecupDir { get; }

        public RecupDirSorter(string rootRecupDirPath)
        {
            RootRecupDir = rootRecupDirPath;
        }

        private IEnumerable<RecupDir> GetRecupDirs(string rootDir)
        {
            var allDirs = Directory.GetDirectories(rootDir);

            var recupDirs = allDirs
                .Where(t => RecupDir.RecupDirRegex.IsMatch(Path.GetFileName(t)))
                .Select(t => new RecupDir(t))
                .OrderBy(t => t.Number);

            return recupDirs;
        }

        public List<RecupFile> FindFiles()
        {
            Console.WriteLine($"Handling root directory: {RootRecupDir}");

            var recupDirs = GetRecupDirs(RootRecupDir).ToList();

            var foundRecupFiles = new List<RecupFile>();

            var lastNumber = recupDirs.LastOrDefault()?.Number;
            int totCount = 0;
            long totSize = 0;
            foreach (var recupDir in recupDirs)
            {
                var toPrint = $"Processing {recupDir.RecupDirName} / {lastNumber}";
                Console.Write(toPrint);

                int count = 0;
                long size = 0;
                foreach (var file in Directory.GetFiles(recupDir.RecupDirPath))
                {
                    var recupFile = new RecupFile(RootRecupDir, file);
                    foundRecupFiles.Add(recupFile);
                    count++;
                    size += recupFile.Size;
                }

                var padding = "".PadRight(Math.Max(1, 40 - toPrint.Length));
                var toPrint2 = $"{padding}(Count: {count}, Size: {ValuesToStringHelper.BytesToString(size)})";
                Console.Write(toPrint2);

                totCount += count;
                totSize += size;

                var padding2 = "".PadRight(Math.Max(1, 72 - (toPrint2.Length + toPrint.Length)));
                Console.WriteLine($"{padding2}(Total count: {totCount}, Total size: {ValuesToStringHelper.BytesToString(totSize)})");
            }

            return foundRecupFiles;
        }

        public void ShowRecupFilesDetails(List<RecupFile> recupFiles)
        {
            var tableContent = new List<List<string>>();
            tableContent.Add(new List<string>() { "Extension", "Count", "Total size" });
            tableContent.Add(null);

            var groups = recupFiles.GroupBy(t => t.Extension).OrderByDescending(t => t.Sum(z => z.Size));

            foreach (var group in groups)
            {
                tableContent.Add(new List<string>()
                {
                    group.Key,
                    group.Count().ToString(),
                    ValuesToStringHelper.BytesToString(group.Sum(t => t.Size))
                });
            }

            tableContent.Add(null);
            tableContent.Add(new List<string>() { "Total:", recupFiles.Count.ToString(), ValuesToStringHelper.BytesToString(recupFiles.Sum(t => t.Size)) });

            var str = TableToTextPrinter.TableToText(tableContent);
            Console.WriteLine(str);
        }

        public void SortRecupFiles(List<RecupFile> recupFiles)
        {

        }
    }
}
