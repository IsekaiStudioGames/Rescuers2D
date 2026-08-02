using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestExternalActivityMonitor
    {
        private const string ProfileGuidKey =
            "EchoDevGames.DeverQuest.ExternalActivity.Profile";
        private const double CheckIntervalSeconds = 0.5d;

        private static DeverQuestExternalActivityProfile profile;
        private static double nextCheckTime;
        private static string activeProviderName = string.Empty;
        private static long activeProviderStartedUtcTicks;

        static DeverQuestExternalActivityMonitor()
        {
            LoadSelection();
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static DeverQuestExternalActivityProfile Profile =>
            profile;

        public static bool HasRecentConfiguredActivity =>
            !string.IsNullOrWhiteSpace(activeProviderName);

        public static string ActiveProviderName =>
            activeProviderName;

        public static bool IsSupported
        {
            get
            {
#if UNITY_EDITOR_WIN
                return true;
#else
                return false;
#endif
            }
        }

        public static void SetProfile(
            DeverQuestExternalActivityProfile value)
        {
            if (profile == value)
            {
                return;
            }

            EndActiveProvider();
            profile = value;
            SaveSelection();
        }

        public static void EndSessionActivity()
        {
            EndActiveProvider();
        }

        private static void Update()
        {
            double editorTime =
                EditorApplication.timeSinceStartup;
            if (editorTime < nextCheckTime)
            {
                return;
            }

            nextCheckTime =
                editorTime + CheckIntervalSeconds;

            if (!DeverQuestSessionStore.HasActiveSession ||
                DeverQuestSessionStore.ActiveSession.state !=
                DeverQuestSessionState.Running ||
                profile == null ||
                profile.providers == null)
            {
                EndActiveProvider();
                return;
            }

            if (!TryGetForegroundApplication(
                    out string processName,
                    out string windowTitle))
            {
                EndActiveProvider();
                return;
            }

            DeverQuestExternalActivityProvider matched = null;
            foreach (DeverQuestExternalActivityProvider provider
                     in profile.providers)
            {
                if (provider == null ||
                    !provider.enabled ||
                    string.IsNullOrWhiteSpace(provider.processName) ||
                    !string.Equals(
                        NormalizeProcess(provider.processName),
                        NormalizeProcess(processName),
                        StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(
                         provider.windowTitleContains) &&
                     (windowTitle?.IndexOf(
                          provider.windowTitleContains,
                          StringComparison.OrdinalIgnoreCase) ?? -1) < 0))
                {
                    continue;
                }

                double idleSeconds =
                    DeverQuestIdleMonitor.CurrentIdleSeconds;
                if (idleSeconds <=
                    Math.Max(1, provider.inputFreshnessSeconds))
                {
                    matched = provider;
                }
                break;
            }

            string matchedName =
                matched?.displayName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(matchedName) &&
                matched != null)
            {
                matchedName = matched.processName;
            }

            if (string.Equals(
                    activeProviderName,
                    matchedName,
                    StringComparison.Ordinal))
            {
                return;
            }

            EndActiveProvider();
            if (matched == null)
            {
                return;
            }

            activeProviderName = matchedName;
            activeProviderStartedUtcTicks =
                DateTime.UtcNow.Ticks;
            DeverQuestSessionStore.RecordExternalActivity(
                activeProviderName,
                true,
                activeProviderStartedUtcTicks,
                0d);
        }

        private static void EndActiveProvider()
        {
            if (string.IsNullOrWhiteSpace(activeProviderName))
            {
                return;
            }

            long endedTicks = DateTime.UtcNow.Ticks;
            double durationSeconds = Math.Max(
                0d,
                TimeSpan.FromTicks(
                    endedTicks - activeProviderStartedUtcTicks)
                    .TotalSeconds);
            DeverQuestSessionStore.RecordExternalActivity(
                activeProviderName,
                false,
                endedTicks,
                durationSeconds);
            activeProviderName = string.Empty;
            activeProviderStartedUtcTicks = 0L;
        }

        private static string NormalizeProcess(string value)
        {
            string normalized =
                value?.Trim() ?? string.Empty;
            return normalized.EndsWith(
                    ".exe",
                    StringComparison.OrdinalIgnoreCase)
                ? normalized.Substring(0, normalized.Length - 4)
                : normalized;
        }

        private static void SaveSelection()
        {
            string path = profile == null
                ? string.Empty
                : AssetDatabase.GetAssetPath(profile);
            string guid = string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : AssetDatabase.AssetPathToGUID(path);
            EditorPrefs.SetString(ProfileGuidKey, guid);
        }

        private static void LoadSelection()
        {
            string guid =
                EditorPrefs.GetString(ProfileGuidKey, string.Empty);
            string path = string.IsNullOrWhiteSpace(guid)
                ? string.Empty
                : AssetDatabase.GUIDToAssetPath(guid);
            profile = string.IsNullOrWhiteSpace(path)
                ? null
                : AssetDatabase.LoadAssetAtPath<
                    DeverQuestExternalActivityProfile>(path);
        }

        private static bool TryGetForegroundApplication(
            out string processName,
            out string windowTitle)
        {
#if UNITY_EDITOR_WIN
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
            {
                processName = string.Empty;
                windowTitle = string.Empty;
                return false;
            }

            GetWindowThreadProcessId(window, out uint processId);
            try
            {
                processName =
                    Process.GetProcessById((int)processId).ProcessName;
            }
            catch
            {
                processName = string.Empty;
            }

            StringBuilder title = new StringBuilder(512);
            GetWindowText(window, title, title.Capacity);
            windowTitle = title.ToString();
            return !string.IsNullOrWhiteSpace(processName);
#else
            processName = string.Empty;
            windowTitle = string.Empty;
            return false;
#endif
        }

#if UNITY_EDITOR_WIN
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(
            IntPtr window,
            out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(
            IntPtr window,
            StringBuilder text,
            int maximumCount);
#endif
    }
}
