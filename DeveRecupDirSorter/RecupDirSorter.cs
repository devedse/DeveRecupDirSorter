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

        public void FindFiles()
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


        }
    }
}
