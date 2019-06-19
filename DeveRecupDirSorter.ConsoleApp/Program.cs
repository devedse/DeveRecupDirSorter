﻿using System;

namespace DeveRecupDirSorter.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rootPath = @"\\devenology\BackupsOthers\BackupRobinAsusLaptop";

            var recupDirSorter = new RecupDirSorter(rootPath);
            recupDirSorter.FindFiles();

            Console.WriteLine($"Done, press enter to exit");
            Console.ReadLine();
        }
    }
}