using System;

namespace DeveRecupDirSorter.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rootPath = @"\\devenology\BackupsOthers\BackupRobinAsusLaptop";

            var recupDirSorter = new RecupDirSorter(rootPath, FileActionType.Copy);
            var recupFiles = recupDirSorter.FindFiles();
            Console.WriteLine();
            recupDirSorter.ShowRecupFilesDetails(recupFiles);
            Console.WriteLine();
            recupDirSorter.SortRecupFiles(recupFiles);

            Console.WriteLine($"Done, press enter to exit");
            Console.ReadLine();
        }
    }
}
