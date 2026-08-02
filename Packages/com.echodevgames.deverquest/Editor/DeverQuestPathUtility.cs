//----- DeverQuestPathUtility.cs START -----

using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestPathUtility
    {
        private const string DefaultTimecardFolderName =
            "DeverQuestTimecards";

        public static string GetDefaultTimecardRoot()
        {
            DirectoryInfo assetsDirectory =
                new DirectoryInfo(Application.dataPath);

            string projectRoot =
                assetsDirectory.Parent?.FullName ??
                Application.dataPath;

            return Path.Combine(projectRoot, DefaultTimecardFolderName);
        }

        public static string GetDeveloperFolder(
            string rootPath,
            string developerName)
        {
            return Path.Combine(
                rootPath,
                MakeSafeFolderName(developerName));
        }

        public static string GetMediaFolder(
            string rootPath,
            string developerName,
            DateTime localDate)
        {
            return Path.Combine(
                GetDeveloperFolder(rootPath, developerName),
                "Media",
                localDate.ToString("yyyy-MM-dd"));
        }

        public static string MakeSafeFolderName(string value)
        {
            string trimmed = value?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return "Developer";
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();

            string safeName = new string(
                trimmed
                    .Select(character =>
                        invalidCharacters.Contains(character)
                            ? '_'
                            : character)
                    .ToArray());

            return safeName.Replace(' ', '_');
        }

        public static bool TryCreateDirectory(
            string path,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                Directory.CreateDirectory(path);
                return Directory.Exists(path);
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }
    }
}

//----- DeverQuestPathUtility.cs END -----
