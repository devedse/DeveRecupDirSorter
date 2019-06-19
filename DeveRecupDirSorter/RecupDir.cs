using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace DeveRecupDirSorter
{
    public class RecupDir
    {
        private const string RecupDirRegexString = @"(^recup_dir\.(\d*)$)";
        public static readonly Regex RecupDirRegex = new Regex(RecupDirRegexString, RegexOptions.Compiled);

        public string RecupDirName { get; }
        public string RecupDirPath { get; }

        public int Number { get; }

        public RecupDir(string recupDirPath)
        {
            RecupDirPath = recupDirPath;
            RecupDirName = Path.GetFileName(recupDirPath);

            var match = RecupDirRegex.Match(RecupDirName);
            if (!match.Success)
            {
                throw new InvalidOperationException($"Can only create RecupDir instance for paths that comply with regex: {RecupDirRegexString}. RecupDirPath value: {RecupDirPath}");
            }

            var number = match.Groups[match.Groups.Count - 1].Value;
            Number = int.Parse(number);
        }

        public override string ToString()
        {
            return RecupDirName;
        }
    }
}
