//----- DeverQuestIdleMonitor.cs START -----

using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestIdleMonitor
    {
        private const double CheckIntervalSeconds = 0.5d;

        private static double nextCheckTime;
        private static double exceptionGraceUntil;
        private static bool warningShown;
        private static string monitoredSessionId = string.Empty;
        private static double projectInactiveSince = -1d;

        static DeverQuestIdleMonitor()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static double CurrentIdleSeconds =>
            TryGetSystemIdleSeconds(out double seconds)
                ? seconds
                : 0d;

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

        public static void BeginIntentionalExternalAction(
            double graceSeconds = 600d)
        {
            double editorTime =
                EditorApplication.timeSinceStartup;
            exceptionGraceUntil = Math.Max(
                exceptionGraceUntil,
                editorTime + Math.Max(0d, graceSeconds));
            projectInactiveSince = -1d;
            ResetWarning();
        }

        private static void Update()
        {
            double editorTime = EditorApplication.timeSinceStartup;

            if (editorTime < nextCheckTime)
            {
                return;
            }

            nextCheckTime = editorTime + CheckIntervalSeconds;

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;

            if (!profile.setupComplete ||
                !profile.idleDetectionEnabled ||
                !DeverQuestSessionStore.HasActiveSession ||
                DeverQuestSessionStore.ActiveSession.state !=
                DeverQuestSessionState.Running)
            {
                ResetWarning();
                return;
            }

            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;

            if (monitoredSessionId != session.sessionId)
            {
                monitoredSessionId = session.sessionId;
                ResetWarning();
            }

            double thresholdSeconds =
                Math.Max(60d, profile.idleTimeoutMinutes * 60d);

            if (DeverQuestExternalActivityMonitor
                .HasRecentConfiguredActivity)
            {
                projectInactiveSince = -1d;
                exceptionGraceUntil =
                    editorTime + thresholdSeconds;
                ResetWarning();
                return;
            }

            if (profile.activityScope ==
                DeverQuestActivityScope.UnityProjectFocused)
            {
                if (editorTime < exceptionGraceUntil)
                {
                    projectInactiveSince = -1d;
                    ResetWarning();
                    return;
                }

                if (InternalEditorUtility.isApplicationActive &&
                    projectInactiveSince >= 0d)
                {
                    double inactiveSeconds =
                        editorTime - projectInactiveSince;
                    projectInactiveSince = -1d;
                    if (inactiveSeconds >= thresholdSeconds)
                    {
                        DeverQuestSessionStore.PauseSession(
                            "Unity Project Lost Focus");
                        ResetWarning();
                        DeverQuestWindow.ShowIdlePaused();
                        return;
                    }
                }

                if (!InternalEditorUtility.isApplicationActive)
                {
                    if (projectInactiveSince < 0d)
                    {
                        projectInactiveSince = editorTime;
                    }

                    double inactiveSeconds =
                        editorTime - projectInactiveSince;
                    if (inactiveSeconds >= thresholdSeconds)
                    {
                        DeverQuestSessionStore.PauseSession(
                            "Unity Project Lost Focus");
                        projectInactiveSince = -1d;
                        ResetWarning();
                        DeverQuestWindow.ShowIdlePaused();
                    }
                    return;
                }

                projectInactiveSince = -1d;
            }

            if (ShouldSuspendIdleDetection(profile))
            {
                exceptionGraceUntil = editorTime + thresholdSeconds;
                ResetWarning();
                return;
            }

            if (editorTime < exceptionGraceUntil)
            {
                ResetWarning();
                return;
            }

            if (!TryGetSystemIdleSeconds(out double idleSeconds))
            {
                return;
            }

            double warningSeconds = Math.Min(
                Math.Max(0d, profile.idleWarningSeconds),
                Math.Max(0d, thresholdSeconds - 1d));

            double warningStart =
                thresholdSeconds - warningSeconds;

            if (warningSeconds > 0d &&
                idleSeconds >= warningStart &&
                idleSeconds < thresholdSeconds)
            {
                if (!warningShown)
                {
                    warningShown = true;
                    int secondsRemaining = Mathf.CeilToInt(
                        (float)(thresholdSeconds - idleSeconds));

                    DeverQuestWindow.ShowIdleWarning(
                        secondsRemaining);
                }

                return;
            }

            if (idleSeconds < warningStart)
            {
                ResetWarning();
                return;
            }

            if (idleSeconds >= thresholdSeconds)
            {
                DeverQuestSessionStore.PauseSession(
                    "Idle Detection");

                ResetWarning();
                DeverQuestWindow.ShowIdlePaused();
            }
        }

        private static bool ShouldSuspendIdleDetection(
            DeverQuestProfile profile)
        {
            if (profile.countPlayModeAsActivity &&
                EditorApplication.isPlaying)
            {
                return true;
            }

            if (profile.countCompilationAsActivity &&
                EditorApplication.isCompiling)
            {
                return true;
            }

            if (profile.countAssetImportAsActivity &&
                EditorApplication.isUpdating)
            {
                return true;
            }

            if (profile.countBuildsAsActivity &&
                BuildPipeline.isBuildingPlayer)
            {
                return true;
            }

            return false;
        }

        private static void ResetWarning()
        {
            warningShown = false;
        }

        private static bool TryGetSystemIdleSeconds(
            out double idleSeconds)
        {
#if UNITY_EDITOR_WIN
            LastInputInfo inputInfo = new LastInputInfo
            {
                size = (uint)Marshal.SizeOf<LastInputInfo>()
            };

            if (!GetLastInputInfo(ref inputInfo))
            {
                idleSeconds = 0d;
                return false;
            }

            uint now = GetTickCount();
            uint idleMilliseconds =
                unchecked(now - inputInfo.lastInputTick);

            idleSeconds = idleMilliseconds / 1000d;
            return true;
#else
            idleSeconds = 0d;
            return false;
#endif
        }

#if UNITY_EDITOR_WIN
        [StructLayout(LayoutKind.Sequential)]
        private struct LastInputInfo
        {
            public uint size;
            public uint lastInputTick;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetLastInputInfo(
            ref LastInputInfo inputInfo);

        [DllImport("kernel32.dll")]
        private static extern uint GetTickCount();
#endif
    }
}

//----- DeverQuestIdleMonitor.cs END -----
