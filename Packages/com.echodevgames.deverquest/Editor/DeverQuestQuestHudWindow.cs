//----- DeverQuestQuestHudWindow.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestQuestHudCoordinator
    {
        static DeverQuestQuestHudCoordinator()
        {
            DeverQuestSessionStore.SessionStarted -= OnSessionStarted;
            DeverQuestSessionStore.SessionStarted += OnSessionStarted;
        }

        private static void OnSessionStarted()
        {
            if (!DeverQuestSettingsStore.Profile
                    .autoOpenQuestHudOnSessionStart)
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                if (DeverQuestSessionStore.HasActiveSession)
                {
                    DeverQuestQuestHudWindow.Open();
                }
            };
        }
    }

    internal sealed class DeverQuestQuestHudWindow : EditorWindow
    {
        private const float MinimumWidth = 280f;
        private const float MinimumHeight = 260f;

        private GUIStyle titleStyle;
        private GUIStyle timerStyle;
        private GUIStyle wrappedStyle;
        private Vector2 scrollPosition;

        [MenuItem("Tools/DeverQuest/Quest HUD")]
        internal static void Open()
        {
            DeverQuestQuestHudWindow window =
                GetWindow<DeverQuestQuestHudWindow>("Quest HUD");
            window.minSize = new Vector2(
                MinimumWidth,
                MinimumHeight);
            window.Show();
            window.Repaint();
        }

        private void OnEnable()
        {
            minSize = new Vector2(
                MinimumWidth,
                MinimumHeight);
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnInspectorUpdate()
        {
            if (DeverQuestSessionStore.HasActiveSession ||
                DeverQuestWellnessMonitor.HasActiveReminder ||
                DeverQuestWellnessMonitor.PendingCount > 0)
            {
                Repaint();
            }
        }

        private void Subscribe()
        {
            DeverQuestSessionStore.SessionStarted -= Repaint;
            DeverQuestSessionStore.SessionStarted += Repaint;
            DeverQuestSessionStore.SessionPaused -= Repaint;
            DeverQuestSessionStore.SessionPaused += Repaint;
            DeverQuestSessionStore.SessionResumed -= Repaint;
            DeverQuestSessionStore.SessionResumed += Repaint;
            DeverQuestSessionStore.SessionCompleted -= Repaint;
            DeverQuestSessionStore.SessionCompleted += Repaint;
            DeverQuestSessionStore.SessionFinalized -= Repaint;
            DeverQuestSessionStore.SessionFinalized += Repaint;
            DeverQuestSessionStore.SessionDiscarded -= Repaint;
            DeverQuestSessionStore.SessionDiscarded += Repaint;
            DeverQuestWellnessMonitor.StateChanged -= Repaint;
            DeverQuestWellnessMonitor.StateChanged += Repaint;
        }

        private void Unsubscribe()
        {
            DeverQuestSessionStore.SessionStarted -= Repaint;
            DeverQuestSessionStore.SessionPaused -= Repaint;
            DeverQuestSessionStore.SessionResumed -= Repaint;
            DeverQuestSessionStore.SessionCompleted -= Repaint;
            DeverQuestSessionStore.SessionFinalized -= Repaint;
            DeverQuestSessionStore.SessionDiscarded -= Repaint;
            DeverQuestWellnessMonitor.StateChanged -= Repaint;
        }

        private void OnGUI()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            BuildStyles(profile);

            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition);

            EditorGUILayout.LabelField(
                "DEVERQUEST QUEST HUD",
                titleStyle);
            EditorGUILayout.Space(6f);

            if (!profile.setupComplete)
            {
                DrawSetupRequired();
            }
            else if (!DeverQuestGuildAccountService.IsAuthenticated)
            {
                DrawAuthenticationRequired();
            }
            else if (!DeverQuestSessionStore.HasActiveSession)
            {
                DrawNoActiveQuest();
            }
            else
            {
                DrawActiveQuest(
                    profile,
                    DeverQuestSessionStore.ActiveSession);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSetupRequired()
        {
            EditorGUILayout.HelpBox(
                "Complete DeverQuest setup before using the Quest HUD.",
                MessageType.Info);
            if (GUILayout.Button("Open DeverQuest Setup"))
            {
                DeverQuestWindow.Open();
            }
        }

        private void DrawAuthenticationRequired()
        {
            EditorGUILayout.HelpBox(
                "Enter the Guild Hall before using the Quest HUD.",
                MessageType.Info);
            if (GUILayout.Button("Open DeverQuest Login"))
            {
                DeverQuestWindow.Open();
            }
        }

        private void DrawNoActiveQuest()
        {
            EditorGUILayout.HelpBox(
                "No Quest is active. The HUD will use the same Session as " +
                "the main DeverQuest window when a Quest begins.",
                MessageType.Info);

            DeverQuestSession last =
                DeverQuestSessionStore.LastCompletedSession;
            if (last != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    "Last Completed",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    string.IsNullOrWhiteSpace(last.taskName)
                        ? "Untitled Quest"
                        : last.taskName,
                    wrappedStyle);
                EditorGUILayout.LabelField(
                    FormatDuration(last.accumulatedFocusedSeconds) +
                    " focused");
                EditorGUILayout.EndVertical();
            }

            DrawWellnessStatus(
                DeverQuestSettingsStore.Profile);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Quest Board"))
            {
                DeverQuestWindow.OpenQuestWorkspace();
            }
            if (GUILayout.Button("Open Chronicle"))
            {
                DeverQuestWindow.OpenChronicleWorkspace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActiveQuest(
            DeverQuestProfile profile,
            DeverQuestSession session)
        {
            foreach (string completedTitle in
                     DeverQuestSessionStore.UpdateQuestStages())
            {
                ShowNotification(
                    new GUIContent(
                        "Encounter Complete: " + completedTitle),
                    5d);
                if (profile.notificationSoundsEnabled)
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.StageComplete);
                }
            }

            EditorGUILayout.LabelField(
                string.IsNullOrWhiteSpace(session.taskName)
                    ? "Untitled Quest"
                    : session.taskName,
                titleStyle);

            if (!string.IsNullOrWhiteSpace(session.projectName))
            {
                EditorGUILayout.LabelField(
                    session.projectName,
                    EditorStyles.centeredGreyMiniLabel);
            }

            double focusedSeconds =
                DeverQuestSessionStore.GetFocusedSeconds();
            EditorGUILayout.LabelField(
                FormatDuration(focusedSeconds),
                timerStyle);

            DrawProgress(profile, session, focusedSeconds);
            DrawState(session);
            DrawWellnessStatus(profile);
            DrawCurrentEncounter(session, focusedSeconds);

            if (profile.questHudShowStory &&
                !string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Quest Story",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.questStory,
                    wrappedStyle);
            }

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Task Objective",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.goal,
                    wrappedStyle);
            }

            DrawLatestEvent(session);
            DrawControls(profile, session);
            DrawNavigation();
        }

        private static void DrawWellnessStatus(
            DeverQuestProfile profile)
        {
            if (profile == null || !profile.showWellnessInQuestHud)
            {
                return;
            }

            bool show = DeverQuestWellnessMonitor.HasActiveReminder ||
                        DeverQuestWellnessMonitor.PendingCount > 0 ||
                        DeverQuestWellnessMonitor.QuietHoursActive ||
                        DeverQuestSessionStore.HasActiveSession;
            if (!show)
            {
                return;
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Wellness",
                EditorStyles.boldLabel);

            if (DeverQuestWellnessMonitor.HasActiveReminder)
            {
                EditorGUILayout.LabelField(
                    DeverQuestWellnessMonitor.ActiveTitle,
                    wrappedStyleStatic);
                EditorGUILayout.LabelField(
                    DeverQuestWellnessMonitor.ActiveMessage,
                    wrappedStyleStatic);
                EditorGUILayout.LabelField(
                    "Break Goal",
                    DeverQuestWellnessMonitor.RecommendedBreakMinutes +
                    "m planned · " +
                    DeverQuestWellnessMonitor.RequiredBreakMinutes +
                    "m minimum");

                using (new EditorGUI.DisabledScope(
                           !DeverQuestWellnessMonitor
                               .CanStartApprovedBreak))
                {
                    if (GUILayout.Button("Take Approved Break"))
                    {
                        DeverQuestWellnessMonitor.Acknowledge(true);
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Acknowledge"))
                {
                    DeverQuestWellnessMonitor.Acknowledge(false);
                }
                if (GUILayout.Button("Snooze"))
                {
                    DeverQuestWellnessMonitor.Snooze();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(
                    "Next",
                    DeverQuestWellnessMonitor
                        .NextSessionReminderSummary());
            }

            if (DeverQuestWellnessMonitor.PendingCount > 0)
            {
                EditorGUILayout.LabelField(
                    "Queued",
                    DeverQuestWellnessMonitor.PendingCount.ToString());
            }
            if (DeverQuestWellnessMonitor.QuietHoursActive)
            {
                EditorGUILayout.LabelField(
                    "Quiet Hours",
                    "Until " +
                    DeverQuestWellnessMonitor.QuietHoursEndsAtLocal
                        .ToString("h:mm tt"));
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawProgress(
            DeverQuestProfile profile,
            DeverQuestSession session,
            double focusedSeconds)
        {
            int targetMinutes =
                session.questSuggestedFocusMinutes > 0
                    ? session.questSuggestedFocusMinutes
                    : profile.defaultFocusMinutes;
            double targetSeconds = Math.Max(60d, targetMinutes * 60d);
            float progress = Mathf.Clamp01(
                (float)(focusedSeconds / targetSeconds));
            Rect progressRect = GUILayoutUtility.GetRect(
                16f,
                20f,
                GUILayout.ExpandWidth(true));
            string progressLabel =
                focusedSeconds <= targetSeconds
                    ? $"{progress * 100f:0}% · " +
                      $"{FormatDuration(targetSeconds - focusedSeconds)} left"
                    : $"Beyond target by " +
                      FormatDuration(focusedSeconds - targetSeconds);
            EditorGUI.ProgressBar(
                progressRect,
                progress,
                progressLabel);
            EditorGUILayout.LabelField(
                "Predicted Task Length",
                targetMinutes + " minutes");
        }

        private static void DrawState(
            DeverQuestSession session)
        {
            string state = session.state ==
                DeverQuestSessionState.Running
                    ? "Working"
                    : "Meditating";
            EditorGUILayout.LabelField("State", state);

            if (session.state != DeverQuestSessionState.Paused)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(session.pauseReason))
            {
                EditorGUILayout.HelpBox(
                    session.pauseReason,
                    session.idlePauseAcknowledged
                        ? MessageType.Info
                        : MessageType.Warning);
            }

            if (DeverQuestSessionStore.GetMeditationRecoveryPreview(
                    out int meditationMinutes,
                    out int meditationHitPoints,
                    out int meditationMana))
            {
                EditorGUILayout.LabelField(
                    "Meditation Recovery",
                    $"+{meditationHitPoints} HP · +{meditationMana} Mana");
                EditorGUILayout.LabelField(
                    "Completed Meditation",
                    meditationMinutes + " full minute(s)");
                EditorGUILayout.LabelField(
                    "Recovery Rate",
                    $"{DeverQuestSessionStore.MeditationHitPointsPerMinute} HP · " +
                    $"{DeverQuestSessionStore.MeditationManaPerMinute} Mana / minute");
            }

            if (session.approvedBreakUntilUtcTicks >
                DateTime.UtcNow.Ticks)
            {
                double remaining = TimeSpan.FromTicks(
                    session.approvedBreakUntilUtcTicks -
                    DateTime.UtcNow.Ticks).TotalSeconds;
                int minimumMinutes = Mathf.CeilToInt(
                    session.approvedBreakPlannedMinutes * 0.8f);
                EditorGUILayout.LabelField(
                    "Approved Break",
                    FormatDuration(remaining) + " remaining");
                EditorGUILayout.LabelField(
                    "Minimum for Benefit",
                    minimumMinutes + " minutes");
            }
        }

        private static void DrawCurrentEncounter(
            DeverQuestSession session,
            double focusedSeconds)
        {
            DeverQuestSessionStage stage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (stage == null)
            {
                return;
            }

            int encounterNumber = 0;
            int encounterCount = 0;
            if (session.questStages != null)
            {
                List<DeverQuestSessionStage> assigned =
                    session.questStages
                        .Where(candidate => candidate != null)
                        .ToList();
                encounterCount = assigned.Count;
                encounterNumber = assigned.IndexOf(stage) + 1;
            }

            string title = string.IsNullOrWhiteSpace(stage.stageTitle)
                ? "Encounter " + Math.Max(1, encounterNumber)
                : stage.stageTitle;
            double elapsed = Math.Max(
                0d,
                focusedSeconds - stage.startedFocusedSeconds);

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Current Encounter",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(title, wrappedStyleStatic);
            if (encounterCount > 0)
            {
                EditorGUILayout.LabelField(
                    "Position",
                    Math.Max(1, encounterNumber) + " of " +
                    encounterCount);
            }
            EditorGUILayout.LabelField(
                "Encounter Pace",
                $"{elapsed / 60d:0.0} / " +
                $"{Math.Max(0, stage.focusedMinutesRequired)} minutes");
            EditorGUILayout.EndVertical();
        }

        private static GUIStyle wrappedStyleStatic
        {
            get
            {
                return new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true
                };
            }
        }

        private void DrawLatestEvent(
            DeverQuestSession session)
        {
            List<DeverQuestQuestEvent> timeline =
                DeverQuestQuestArchiveService.BuildTimeline(
                    session,
                    true);
            DeverQuestQuestEvent latest =
                timeline.FirstOrDefault();
            if (latest == null)
            {
                return;
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Latest Quest Event",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                latest.Title,
                wrappedStyle);
            if (!string.IsNullOrWhiteSpace(latest.Detail))
            {
                EditorGUILayout.LabelField(
                    latest.Detail,
                    wrappedStyle);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawControls(
            DeverQuestProfile profile,
            DeverQuestSession session)
        {
            EditorGUILayout.Space(8f);

            if (!session.idlePauseAcknowledged)
            {
                if (GUILayout.Button("I Have Returned — Acknowledge"))
                {
                    DeverQuestSessionStore.AcknowledgeIdlePause();
                    Repaint();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (session.state == DeverQuestSessionState.Running)
                {
                    if (GUILayout.Button("Meditate"))
                    {
                        DeverQuestSessionStore.PauseSession();
                    }
                    if (GUILayout.Button(
                            $"Start {profile.wellnessShortBreakMinutes}m Break"))
                    {
                        DeverQuestSessionStore.PauseForApprovedBreak(
                            profile.wellnessShortBreakMinutes,
                            "Quest HUD Break");
                    }
                }
                else if (GUILayout.Button("Resume Quest"))
                {
                    DeverQuestSessionStore.ResumeSession();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button(
                    "Open Quest Turn-In",
                    GUILayout.Height(28f)))
            {
                DeverQuestWindow.OpenQuestTurnIn();
            }
        }

        private static void DrawNavigation()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Current Quest"))
            {
                DeverQuestWindow.OpenQuestWorkspace();
            }
            if (GUILayout.Button("Quest Log"))
            {
                DeverQuestWindow.OpenQuestLogWorkspace();
            }
            if (GUILayout.Button("Chronicle"))
            {
                DeverQuestWindow.OpenChronicleWorkspace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void BuildStyles(
            DeverQuestProfile profile)
        {
            float scale = Mathf.Clamp(
                profile.interfaceScale,
                0.85f,
                1.35f);
            Color titleColor = ResolveTitleColor(profile);
            Color timerColor = ResolveTimerColor(profile);

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(
                    EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
            }
            if (timerStyle == null)
            {
                timerStyle = new GUIStyle(
                    EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }
            if (wrappedStyle == null)
            {
                wrappedStyle = new GUIStyle(
                    EditorStyles.label)
                {
                    wordWrap = true
                };
            }

            titleStyle.fontSize = Mathf.RoundToInt(18f * scale);
            timerStyle.fontSize = Mathf.RoundToInt(30f * scale);
            titleStyle.normal.textColor = titleColor;
            timerStyle.normal.textColor = timerColor;
        }

        private static Color ResolveTitleColor(
            DeverQuestProfile profile)
        {
            switch (profile.theme)
            {
                case DeverQuestTheme.Dark:
                    return new Color(0.78f, 0.86f, 0.94f);
                case DeverQuestTheme.Light:
                    return new Color(0.12f, 0.20f, 0.28f);
                case DeverQuestTheme.EchoNeon:
                    return new Color(0.20f, 0.94f, 0.86f);
                case DeverQuestTheme.Custom:
                    return profile.customTitleColor;
                default:
                    return EditorStyles.label.normal.textColor;
            }
        }

        private static Color ResolveTimerColor(
            DeverQuestProfile profile)
        {
            switch (profile.theme)
            {
                case DeverQuestTheme.Dark:
                    return new Color(0.55f, 0.82f, 1f);
                case DeverQuestTheme.Light:
                    return new Color(0.05f, 0.42f, 0.56f);
                case DeverQuestTheme.EchoNeon:
                    return new Color(1f, 0.30f, 0.70f);
                case DeverQuestTheme.Custom:
                    return profile.customTimerColor;
                default:
                    return EditorStyles.label.normal.textColor;
            }
        }

        private static string FormatDuration(
            double totalSeconds)
        {
            TimeSpan duration = TimeSpan.FromSeconds(
                Math.Max(0d, totalSeconds));
            if (duration.TotalHours >= 1d)
            {
                return $"{(int)duration.TotalHours:00}:" +
                       $"{duration.Minutes:00}:" +
                       $"{duration.Seconds:00}";
            }
            return $"{duration.Minutes:00}:" +
                   $"{duration.Seconds:00}";
        }
    }
}

//----- DeverQuestQuestHudWindow.cs END -----
