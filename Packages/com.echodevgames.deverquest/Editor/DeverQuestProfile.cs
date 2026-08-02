//----- DeverQuestProfile.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestTheme
    {
        System = 0,
        Dark = 1,
        Light = 2,
        EchoNeon = 3,
        Custom = 4
    }

    internal enum DeverQuestActivityScope
    {
        UnityProjectFocused = 0,
        SystemWideInput = 1
    }

    internal enum DeverQuestCampaignDifficulty
    {
        Story = 0,
        Standard = 1,
        Heroic = 2,
        Mythic = 3
    }

    [Serializable]
    internal sealed class DeverQuestProfile
    {
        public const int CurrentDataVersion = 16;

        public int dataVersion = CurrentDataVersion;
        public bool setupComplete;
        public string developerName = string.Empty;
        public string timecardRootPath = string.Empty;
        public int defaultFocusMinutes = 50;
        public bool idleDetectionEnabled = true;
        public int idleTimeoutMinutes = 5;
        public int idleWarningSeconds = 30;
        public bool countPlayModeAsActivity = true;
        public bool countCompilationAsActivity = true;
        public bool countAssetImportAsActivity = true;
        public bool countBuildsAsActivity = true;

        public bool wellnessEnabled = true;
        public int checkInMinutes = 30;
        public int movementBreakMinutes = 60;
        public int hydrationMinutes = 45;
        public int exerciseMinutes = 120;
        public int snoozeMinutes = 10;
        public int wellnessShortBreakMinutes = 5;
        public int wellnessMealBreakMinutes = 30;
        public int wellnessQuietBreakMinutes = 15;
        public int wellnessBreakExperience = 5;

        public bool mealRemindersEnabled = true;
        public int lunchHour = 12;
        public int lunchMinute = 0;
        public int dinnerHour = 18;
        public int dinnerMinute = 0;

        public bool quietHoursEnabled = true;
        public int quietHoursStartHour = 22;
        public int quietHoursEndHour = 7;
        public bool suppressWellnessDuringQuietHours = true;
        public bool showWellnessInQuestHud = true;
        public int wellnessHistoryLimit = 200;

        public bool rewardsEnabled = true;
        public int rewardWorkBlockMinutes = 30;
        public int dailyWorkGoalMinutes = 240;
        public int copperPerWorkBlock = 25;
        public int experiencePerWorkBlock = 50;
        public int dailyCopperBonus = 100;
        public int dailyExperienceBonus = 100;
        public int baseQuestCopper = 10;
        public int baseQuestExperience = 10;

        public bool autoPlayMusicOnSessionStart;
        public bool pauseMusicWithSession = true;
        public bool resumeMusicWithSession = true;
        public bool stopMusicOnSessionEnd = true;
        public bool compactMode;
        public DeverQuestTheme theme = DeverQuestTheme.EchoNeon;
        public float interfaceScale = 1f;
        public int workspaceTabColumns = 4;
        public bool useCompactWorkspaceLabels;
        public bool showWorkspaceHints = true;
        public bool showHeaderTagline = true;
        public bool autoOpenQuestHudOnSessionStart;
        public bool questHudShowStory = true;
        public Color customTitleColor =
            new Color(0.20f, 0.94f, 0.86f, 1f);
        public Color customTimerColor =
            new Color(1f, 0.30f, 0.70f, 1f);
        public Color customAccentColor =
            new Color(0.55f, 0.82f, 1f, 1f);
        public bool showEditorNotifications = true;
        public bool notificationSoundsEnabled = true;
        public bool autoOpenWindowForReminders = true;
        public string lastProjectName = string.Empty;
        public string lastDepartmentName = "Programming";
        public bool lockProjectName;
        public string lockedProjectName = string.Empty;
        public DeverQuestActivityScope activityScope =
            DeverQuestActivityScope.UnityProjectFocused;
        public List<int> focusCheckInScheduleMinutes =
            new List<int> { 15, 30, 45, 60 };
        public string gitRepositoryOverridePath = string.Empty;
        public bool chronicleIntegrityEnabled = true;
        public int chronicleMaxSessions = 12;
        public int chronicleMaxKilobytes = 512;
        public int suspiciousQuestMinutes = 240;
        public int suspiciousDailyQuestCount = 8;
        public int dailyDecreeRecommendedLevel = 1;
        public DeverQuestCampaignDifficulty campaignDifficulty =
            DeverQuestCampaignDifficulty.Standard;
        public int dailyDecreeCheckModifier;
        public bool sharedGuildEnabled;
        public string sharedGuildRepositoryPath = string.Empty;
        public bool publishCompletedQuests = true;
        public int healthyDailyFocusMinutes = 600;

        public void Sanitize()
        {
            if (dataVersion < 2)
            {
                idleWarningSeconds = 30;
                countPlayModeAsActivity = true;
                countCompilationAsActivity = true;
                countAssetImportAsActivity = true;
                countBuildsAsActivity = true;
            }

            if (dataVersion < 3)
            {
                wellnessEnabled = true;
                checkInMinutes = 30;
                movementBreakMinutes = 60;
                hydrationMinutes = 45;
                exerciseMinutes = 120;
                snoozeMinutes = 10;
                mealRemindersEnabled = true;
                lunchHour = 12;
                lunchMinute = 0;
                dinnerHour = 18;
                dinnerMinute = 0;
                quietHoursEnabled = true;
                quietHoursStartHour = 22;
            }

            if (dataVersion < 4)
            {
                rewardsEnabled = true;
                rewardWorkBlockMinutes = 30;
                dailyWorkGoalMinutes = 240;
            }

            if (dataVersion < 5)
            {
                autoPlayMusicOnSessionStart = false;
                pauseMusicWithSession = true;
                resumeMusicWithSession = true;
                stopMusicOnSessionEnd = true;
            }

            if (dataVersion < 6)
            {
                compactMode = false;
                theme = DeverQuestTheme.EchoNeon;
                showEditorNotifications = true;
                notificationSoundsEnabled = true;
                autoOpenWindowForReminders = true;
            }

            if (dataVersion < 7)
            {
                lastProjectName = string.Empty;
                lastDepartmentName = "Programming";
                lockProjectName = false;
                lockedProjectName = string.Empty;
                activityScope =
                    DeverQuestActivityScope.UnityProjectFocused;
                focusCheckInScheduleMinutes =
                    new List<int> { 15, 30, 45, 60 };
            }

            if (dataVersion < 8)
            {
                gitRepositoryOverridePath = string.Empty;
            }

            if (dataVersion < 9)
            {
                copperPerWorkBlock = 25;
                experiencePerWorkBlock = 50;
                dailyCopperBonus = 100;
                dailyExperienceBonus = 100;
            }

            if (dataVersion < 10)
            {
                baseQuestCopper = 10;
                baseQuestExperience = 10;
            }

            if (dataVersion < 11)
            {
                chronicleIntegrityEnabled = true;
                chronicleMaxSessions = 12;
                chronicleMaxKilobytes = 512;
                suspiciousQuestMinutes = 240;
                suspiciousDailyQuestCount = 8;
            }

            if (dataVersion < 12)
            {
                dailyDecreeRecommendedLevel = 1;
                campaignDifficulty =
                    DeverQuestCampaignDifficulty.Standard;
                dailyDecreeCheckModifier = 0;
            }

            if (dataVersion < 13)
            {
                wellnessShortBreakMinutes = 5;
                wellnessMealBreakMinutes = 30;
                wellnessQuietBreakMinutes = 15;
                wellnessBreakExperience = 5;
            }

            if (dataVersion < 14)
            {
                sharedGuildEnabled = false;
                sharedGuildRepositoryPath = string.Empty;
                publishCompletedQuests = true;
                healthyDailyFocusMinutes = 600;
            }

            if (dataVersion < 15)
            {
                interfaceScale = 1f;
                workspaceTabColumns = 4;
                useCompactWorkspaceLabels = false;
                showWorkspaceHints = true;
                showHeaderTagline = true;
                autoOpenQuestHudOnSessionStart = false;
                questHudShowStory = true;
                customTitleColor =
                    new Color(0.20f, 0.94f, 0.86f, 1f);
                customTimerColor =
                    new Color(1f, 0.30f, 0.70f, 1f);
                customAccentColor =
                    new Color(0.55f, 0.82f, 1f, 1f);
            }


            if (dataVersion < 16)
            {
                quietHoursEndHour = 7;
                suppressWellnessDuringQuietHours = true;
                showWellnessInQuestHud = true;
                wellnessHistoryLimit = 200;
            }

            developerName = developerName?.Trim() ?? string.Empty;
            timecardRootPath = timecardRootPath?.Trim() ?? string.Empty;
            lastProjectName = lastProjectName?.Trim() ?? string.Empty;
            lastDepartmentName =
                lastDepartmentName?.Trim() ?? string.Empty;
            lockedProjectName =
                lockedProjectName?.Trim() ?? string.Empty;
            gitRepositoryOverridePath =
                gitRepositoryOverridePath?.Trim() ?? string.Empty;
            sharedGuildRepositoryPath =
                sharedGuildRepositoryPath?.Trim() ?? string.Empty;
            if (focusCheckInScheduleMinutes == null)
            {
                focusCheckInScheduleMinutes = new List<int>();
            }
            focusCheckInScheduleMinutes.RemoveAll(value => value <= 0);
            focusCheckInScheduleMinutes.Sort();
            for (int index = focusCheckInScheduleMinutes.Count - 1;
                 index > 0;
                 index--)
            {
                if (focusCheckInScheduleMinutes[index] ==
                    focusCheckInScheduleMinutes[index - 1])
                {
                    focusCheckInScheduleMinutes.RemoveAt(index);
                }
            }
            defaultFocusMinutes = Math.Max(1, defaultFocusMinutes);
            idleTimeoutMinutes = Math.Max(1, idleTimeoutMinutes);
            idleWarningSeconds = Math.Max(0, idleWarningSeconds);
            checkInMinutes = Math.Max(0, checkInMinutes);
            movementBreakMinutes = Math.Max(0, movementBreakMinutes);
            hydrationMinutes = Math.Max(0, hydrationMinutes);
            exerciseMinutes = Math.Max(0, exerciseMinutes);
            snoozeMinutes = Math.Max(1, snoozeMinutes);
            wellnessShortBreakMinutes =
                Math.Max(1, wellnessShortBreakMinutes);
            wellnessMealBreakMinutes =
                Math.Max(1, wellnessMealBreakMinutes);
            wellnessQuietBreakMinutes =
                Math.Max(1, wellnessQuietBreakMinutes);
            wellnessBreakExperience =
                Math.Max(0, wellnessBreakExperience);
            lunchHour = Math.Min(23, Math.Max(0, lunchHour));
            lunchMinute = Math.Min(59, Math.Max(0, lunchMinute));
            dinnerHour = Math.Min(23, Math.Max(0, dinnerHour));
            dinnerMinute = Math.Min(59, Math.Max(0, dinnerMinute));
            quietHoursStartHour =
                Math.Min(23, Math.Max(0, quietHoursStartHour));
            quietHoursEndHour =
                Math.Min(23, Math.Max(0, quietHoursEndHour));
            wellnessHistoryLimit =
                Math.Min(1000, Math.Max(25, wellnessHistoryLimit));
            rewardWorkBlockMinutes =
                Math.Max(1, rewardWorkBlockMinutes);
            dailyWorkGoalMinutes =
                Math.Max(0, dailyWorkGoalMinutes);
            copperPerWorkBlock = Math.Max(0, copperPerWorkBlock);
            experiencePerWorkBlock =
                Math.Max(0, experiencePerWorkBlock);
            dailyCopperBonus = Math.Max(0, dailyCopperBonus);
            dailyExperienceBonus =
                Math.Max(0, dailyExperienceBonus);
            baseQuestCopper = Math.Max(0, baseQuestCopper);
            baseQuestExperience =
                Math.Max(0, baseQuestExperience);
            chronicleMaxSessions = Math.Max(1, chronicleMaxSessions);
            chronicleMaxKilobytes = Math.Max(32, chronicleMaxKilobytes);
            suspiciousQuestMinutes = Math.Max(0, suspiciousQuestMinutes);
            suspiciousDailyQuestCount =
                Math.Max(0, suspiciousDailyQuestCount);
            dailyDecreeRecommendedLevel =
                Math.Max(1, dailyDecreeRecommendedLevel);
            dailyDecreeCheckModifier =
                Math.Min(10, Math.Max(-10,
                    dailyDecreeCheckModifier));
            healthyDailyFocusMinutes =
                Math.Max(60, healthyDailyFocusMinutes);
            interfaceScale = Mathf.Clamp(interfaceScale, 0.85f, 1.35f);
            workspaceTabColumns =
                Math.Min(6, Math.Max(2, workspaceTabColumns));
            customTitleColor = SanitizeColor(
                customTitleColor,
                new Color(0.20f, 0.94f, 0.86f, 1f));
            customTimerColor = SanitizeColor(
                customTimerColor,
                new Color(1f, 0.30f, 0.70f, 1f));
            customAccentColor = SanitizeColor(
                customAccentColor,
                new Color(0.55f, 0.82f, 1f, 1f));
            if (!Enum.IsDefined(typeof(DeverQuestTheme), theme))
            {
                theme = DeverQuestTheme.EchoNeon;
            }
            dataVersion = CurrentDataVersion;
        }

        private static Color SanitizeColor(
            Color value,
            Color fallback)
        {
            if (float.IsNaN(value.r) ||
                float.IsNaN(value.g) ||
                float.IsNaN(value.b) ||
                float.IsNaN(value.a))
            {
                return fallback;
            }

            value.r = Mathf.Clamp01(value.r);
            value.g = Mathf.Clamp01(value.g);
            value.b = Mathf.Clamp01(value.b);
            value.a = Mathf.Clamp01(value.a);
            if (value.a <= 0.01f)
            {
                value.a = 1f;
            }
            return value;
        }
    }
}

//----- DeverQuestProfile.cs END -----
