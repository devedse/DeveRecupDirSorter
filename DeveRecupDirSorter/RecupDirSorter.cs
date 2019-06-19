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

            foreach (var recupDir in recupDirs)
            {
                Console.WriteLine($"Processing {recupDir.RecupDirName}");

                foreach (var file in Directory.GetFiles(recupDir.RecupDirPath))
                {
                    var recupFile = new RecupFile(RootRecupDir, file);
                    foundRecupFiles.Add(recupFile);
                }
            }

            return foundRecupFiles;
        }

        public void ShowRecupFilesDetails(List<RecupFile> recupFiles)
        {
            var tableContent = new List<List<string>>();
            tableContent.Add(new List<string>() { "Extension", "Count", "Total size" });
            tableContent.Add(null);

            var groups = recupFiles.GroupBy(t => t.Extension).OrderBy(t => t.Count());

            foreach (var group in groups)
            {
                tableContent.Add(new List<string>()
                {
                    group.Key,
                    group.Count().ToString(),
                    ValuesToStringHelper.BytesToString( group.Sum(t => t.Size))
                });
            }

            TableToTextPrinter.TableToText(tableContent);
        }

        public void SortRecupFiles(List<RecupFile> recupFiles)
        {

        }
    }
}
