//----- DeverQuestProfile.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestTheme
    {
        System = 0,
        Dark = 1,
        Light = 2,
        EchoNeon = 3
    }

    internal enum DeverQuestActivityScope
    {
        UnityProjectFocused = 0,
        SystemWideInput = 1
    }

    [Serializable]
    internal sealed class DeverQuestProfile
    {
        public const int CurrentDataVersion = 9;

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

        public bool mealRemindersEnabled = true;
        public int lunchHour = 12;
        public int lunchMinute = 0;
        public int dinnerHour = 18;
        public int dinnerMinute = 0;

        public bool quietHoursEnabled = true;
        public int quietHoursStartHour = 22;

        public bool rewardsEnabled = true;
        public int rewardWorkBlockMinutes = 30;
        public int dailyWorkGoalMinutes = 240;
        public int copperPerWorkBlock = 25;
        public int experiencePerWorkBlock = 50;
        public int dailyCopperBonus = 100;
        public int dailyExperienceBonus = 100;

        public bool autoPlayMusicOnSessionStart;
        public bool pauseMusicWithSession = true;
        public bool resumeMusicWithSession = true;
        public bool stopMusicOnSessionEnd = true;
        public bool compactMode;
        public DeverQuestTheme theme = DeverQuestTheme.EchoNeon;
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

            developerName = developerName?.Trim() ?? string.Empty;
            timecardRootPath = timecardRootPath?.Trim() ?? string.Empty;
            lastProjectName = lastProjectName?.Trim() ?? string.Empty;
            lastDepartmentName =
                lastDepartmentName?.Trim() ?? string.Empty;
            lockedProjectName =
                lockedProjectName?.Trim() ?? string.Empty;
            gitRepositoryOverridePath =
                gitRepositoryOverridePath?.Trim() ?? string.Empty;
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
            lunchHour = Math.Min(23, Math.Max(0, lunchHour));
            lunchMinute = Math.Min(59, Math.Max(0, lunchMinute));
            dinnerHour = Math.Min(23, Math.Max(0, dinnerHour));
            dinnerMinute = Math.Min(59, Math.Max(0, dinnerMinute));
            quietHoursStartHour =
                Math.Min(23, Math.Max(0, quietHoursStartHour));
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
            if (!Enum.IsDefined(typeof(DeverQuestTheme), theme))
            {
                theme = DeverQuestTheme.EchoNeon;
            }
            dataVersion = CurrentDataVersion;
        }
    }
}

//----- DeverQuestProfile.cs END -----
