using DeveCoolLib.Conversion;
using DeveCoolLib.TextFormatting;
using DeveRecupDirSorter.Helpers;
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
        public static readonly Regex DigitOnlyRegex = new Regex(@"^(\d*)$", RegexOptions.Compiled);

        public string RootRecupDir { get; }
        public FileActionType FileActionType { get; }
        public int MaxFilesPerSortedDir { get; }

        public RecupDirSorter(string rootRecupDirPath, FileActionType fileActionType, int maxFilesPerSortedDir = 500)
        {
            RootRecupDir = rootRecupDirPath;
            FileActionType = fileActionType;
            MaxFilesPerSortedDir = maxFilesPerSortedDir;
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
            var outputDirPath = Path.Combine(RootRecupDir, "Output");
            Directory.CreateDirectory(outputDirPath);

            var groups = recupFiles.GroupBy(t => t.Extension).OrderByDescending(t => t.Sum(z => z.Size));

            foreach (var group in groups)
            {
                var extensionWithoutDot = group.Key.Replace(".", "");
                var groupOutputDirPath = Path.Combine(outputDirPath, extensionWithoutDot);

                Directory.CreateDirectory(groupOutputDirPath);

                Console.WriteLine($"Handling extension: {extensionWithoutDot} with {group.Count()} files");
                Console.WriteLine($"\tObtaining existing files...");
                var existingFiles = new HashSet<string>(DirectoryHelper.FindAllFileNamesInDirectory(groupOutputDirPath).ToList());

                Console.WriteLine($"\tFound {existingFiles.Count} existing files");

                foreach (var file in group)
                {
                    if (!existingFiles.Contains(file.DesiredFileName.Value))
                    {
                        CopyFileToDest(groupOutputDirPath, file);


                        existingFiles.Add(file.DesiredFileName.Value);
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write('$');
                    }
                }
                Console.WriteLine();
                Console.WriteLine("\tDone");
            }
        }

        private void CopyFileToDest(string outputDirPath, RecupFile file)
        {
            var numberedDirectories = Directory.GetDirectories(outputDirPath)
                                            .Where(t => Directory.Exists(t))
                                            .Select(t => Path.GetFileName(t))
                                            .Where(t => DigitOnlyRegex.IsMatch(t))
                                            .Select(t => int.Parse(t))
                                            .OrderBy(t => t)
                                            .ToList();


            string desiredDirectoryName = "0";

            var desiredDirNameForThisFile = file.DesiredDirName.Value;
            if (desiredDirNameForThisFile != null)
            {
                desiredDirectoryName = desiredDirNameForThisFile;
            }
            else if (numberedDirectories.Any())
            {
                var lastDir = Path.Combine(outputDirPath, numberedDirectories.Last().ToString());
                var filesHere = Directory.GetFiles(lastDir);
                if (filesHere.Length >= 500)
                {
                    desiredDirectoryName = (numberedDirectories.Last() + 1).ToString();
                }
                else
                {
                    desiredDirectoryName = numberedDirectories.Last().ToString();
                }
            }

            var destDirPath = Path.Combine(outputDirPath, desiredDirectoryName);
            Directory.CreateDirectory(destDirPath);

            var sourceFilePath = Path.Combine(RootRecupDir, file.RelativePath);
            var destFilePath = Path.Combine(destDirPath, file.DesiredFileName.Value);

            if (FileActionType == FileActionType.Copy)
            {
                File.Copy(sourceFilePath, destFilePath);
            }
            else if (FileActionType == FileActionType.Move)
            {
                File.Move(sourceFilePath, destFilePath);
            }
        }
    }
}
