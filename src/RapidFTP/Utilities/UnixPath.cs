namespace RapidFTP.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using RapidFTP.Models;

    public class UnixPath
    {
        public const char DirectorySeperator = '/';

        public const string DirectorySeperatorString = "/";

        public static string Combine(params string[] args)
        {
            return string.Join(DirectorySeperatorString, args);
        }

        public static string CorrectDirectorySeperator(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            path = path.Replace(Path.DirectorySeparatorChar, DirectorySeperator);
            if (!path.StartsWith(DirectorySeperatorString))
            {
                path = DirectorySeperatorString + path;
            }

            return path;
        }

        public static bool IsEqual(string directoryA, string directoryB)
        {
            directoryA = CorrectDirectorySeperator(directoryA);
            directoryB = CorrectDirectorySeperator(directoryB);

            return directoryA.Equals(directoryB, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string[] GetRelatedDirectories(string directory)
        {
            if (!IsWellFormed(directory, ItemType.Directory))
            {
                throw new InvalidOperationException(
                    string.Format("{0} is invalid directory path", directory));
            }

            var correctedPath = CorrectDirectorySeperator(directory);
            var splitedPart = correctedPath.Split(
                new[] { DirectorySeperatorString }, StringSplitOptions.RemoveEmptyEntries);
            var relatedDirectories = new List<string>();
            var walking = new List<string>();
            for (var i = 0; i < splitedPart.Length; i++)
            {
                walking.Add(splitedPart[i]);
                var relatedDirectory = DirectorySeperatorString + Combine(walking.ToArray());
                relatedDirectories.Add(relatedDirectory);
            }

            return relatedDirectories.ToArray();
        }

        public static bool IsWellFormed(string path, ItemType itemType)
        {
            var wellFormed = true;

            switch (itemType)
            {
                case ItemType.Directory:
                    wellFormed = 
                        path == DirectorySeperatorString || Regex.IsMatch(path, @"^(/([a-zA-Z0-9_-])+)+(/)*$");
                    break;
            }

            return wellFormed;
        }

        public static string GetDirectoryName(string remoteFile)
        {
            var fixedRemoteFile = CorrectDirectorySeperator(remoteFile);
            var lastSeperator = remoteFile.LastIndexOf(DirectorySeperator);
            if (lastSeperator != -1)
            {
                return fixedRemoteFile.Substring(0, lastSeperator + 1);
            }

            // Return root directory
            return DirectorySeperatorString;
        }

        public static string GetFileName(string remoteFile)
        {
            return Path.GetFileName(remoteFile);
        }
    }
}