//----- DeverQuestRewardService.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestRewardCategory
    {
        public string categoryId = string.Empty;
        public string displayName = string.Empty;
        public double rewardMinutesPerBlock;
        public double dailyBonusMinutes;
        public double balanceMinutes;
        public double totalEarnedMinutes;
        public double totalSpentMinutes;
        public bool isBuiltIn;

        public void Sanitize()
        {
            categoryId = categoryId?.Trim() ?? string.Empty;
            displayName = displayName?.Trim() ?? string.Empty;
            rewardMinutesPerBlock =
                Math.Max(0d, rewardMinutesPerBlock);
            dailyBonusMinutes =
                Math.Max(0d, dailyBonusMinutes);
            balanceMinutes = Math.Max(0d, balanceMinutes);
            totalEarnedMinutes =
                Math.Max(0d, totalEarnedMinutes);
            totalSpentMinutes =
                Math.Max(0d, totalSpentMinutes);
        }
    }

    [Serializable]
    internal sealed class DeverQuestRewardDay
    {
        public string localDate = string.Empty;
        public double focusedSeconds;
        public bool dailyBonusAwarded;
    }

    [Serializable]
    internal sealed class DeverQuestRewardWallet
    {
        public List<DeverQuestRewardCategory> categories =
            new List<DeverQuestRewardCategory>();

        public List<string> processedSessionIds =
            new List<string>();

        public List<DeverQuestRewardDay> days =
            new List<DeverQuestRewardDay>();

        public double unrewardedWorkSeconds;
        public bool legacyBalancesMigratedToCopper;

        public void Sanitize()
        {
            categories ??= new List<DeverQuestRewardCategory>();
            processedSessionIds ??= new List<string>();
            days ??= new List<DeverQuestRewardDay>();

            foreach (DeverQuestRewardCategory category in categories)
            {
                category?.Sanitize();
            }

            unrewardedWorkSeconds =
                Math.Max(0d, unrewardedWorkSeconds);
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestRewardService
    {
        private const string WalletKey =
            "EchoDevGames.DeverQuest.RewardWallet.v1";

        private const string GameTimeId = "game-time";
        private const string OtherFunId = "other-fun";

        private static DeverQuestRewardWallet wallet;

        static DeverQuestRewardService()
        {
            Load();
            EnsureDefaults();
        }

        public static DeverQuestRewardWallet Wallet
        {
            get
            {
                if (wallet == null)
                {
                    Load();
                    EnsureDefaults();
                }

                return wallet;
            }
        }

        public static void ProcessCompletedSession(
            DeverQuestProfile profile,
            DeverQuestSession session)
        {
            if (profile == null ||
                session == null ||
                !profile.rewardsEnabled ||
                Wallet.processedSessionIds.Contains(session.sessionId))
            {
                return;
            }

            EnsureDefaults();

            Wallet.unrewardedWorkSeconds +=
                session.accumulatedFocusedSeconds;

            double blockSeconds =
                Math.Max(60d, profile.rewardWorkBlockMinutes * 60d);

            int completedBlocks =
                (int)Math.Floor(
                    Wallet.unrewardedWorkSeconds / blockSeconds);

            if (completedBlocks > 0)
            {
                Wallet.unrewardedWorkSeconds -=
                    completedBlocks * blockSeconds;

                AwardProgression(
                    session,
                    completedBlocks * (long)profile.copperPerWorkBlock,
                    completedBlocks * (long)profile.experiencePerWorkBlock,
                    "Work Block",
                    $"{completedBlocks} completed work block(s)");
            }

            ProcessDailyGoal(profile, session);

            Wallet.processedSessionIds.Add(session.sessionId);
            session.rewardsProcessed = true;
            Save();
        }

        public static bool Spend(
            string categoryId,
            double minutes,
            out DeverQuestRewardTransaction transaction,
            out string errorMessage)
        {
            transaction = null;
            errorMessage = string.Empty;

            DeverQuestRewardCategory category =
                Wallet.categories.FirstOrDefault(
                    item => item.categoryId == categoryId);

            if (category == null)
            {
                errorMessage = "Reward category was not found.";
                return false;
            }

            minutes = Math.Round(minutes, 1);

            if (minutes <= 0d)
            {
                errorMessage = "Enter reward minutes greater than zero.";
                return false;
            }

            if (minutes > category.balanceMinutes)
            {
                errorMessage =
                    $"Only {category.balanceMinutes:0.#} minutes are available.";
                return false;
            }

            category.balanceMinutes -= minutes;
            category.totalSpentMinutes += minutes;

            transaction = new DeverQuestRewardTransaction
            {
                categoryName = category.displayName,
                transactionType = "Spent",
                minutes = -minutes,
                createdUtcTicks = DateTime.UtcNow.Ticks,
                note = "Reward time spent"
            };

            Save();
            return true;
        }

        public static bool AddCategory(
            string displayName,
            double rewardMinutesPerBlock,
            double dailyBonusMinutes,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            displayName = displayName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                errorMessage = "Enter a category name.";
                return false;
            }

            if (Wallet.categories.Any(
                    category => string.Equals(
                        category.displayName,
                        displayName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "A category with that name already exists.";
                return false;
            }

            Wallet.categories.Add(
                new DeverQuestRewardCategory
                {
                    categoryId = Guid.NewGuid().ToString("N"),
                    displayName = displayName,
                    rewardMinutesPerBlock =
                        Math.Max(0d, rewardMinutesPerBlock),
                    dailyBonusMinutes =
                        Math.Max(0d, dailyBonusMinutes)
                });

            Save();
            return true;
        }

        public static bool RemoveCategory(
            string categoryId,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            DeverQuestRewardCategory category =
                Wallet.categories.FirstOrDefault(
                    item => item.categoryId == categoryId);

            if (category == null)
            {
                errorMessage = "Reward category was not found.";
                return false;
            }

            if (category.isBuiltIn)
            {
                errorMessage = "Built-in categories cannot be removed.";
                return false;
            }

            Wallet.categories.Remove(category);
            Save();
            return true;
        }

        public static void Save()
        {
            Wallet.Sanitize();
            EditorPrefs.SetString(
                WalletKey,
                JsonUtility.ToJson(Wallet));
        }

        public static double GetTodayFocusedMinutes()
        {
            string dateKey = DateTime.Now.ToString("yyyy-MM-dd");

            DeverQuestRewardDay day = Wallet.days.FirstOrDefault(
                item => item.localDate == dateKey);

            return day == null
                ? 0d
                : day.focusedSeconds / 60d;
        }

        private static void ProcessDailyGoal(
            DeverQuestProfile profile,
            DeverQuestSession session)
        {
            DateTime completion =
                DeverQuestSessionStore.GetLocalCompletionTime(session);

            string dateKey = completion.ToString("yyyy-MM-dd");

            DeverQuestRewardDay day = Wallet.days.FirstOrDefault(
                item => item.localDate == dateKey);

            if (day == null)
            {
                day = new DeverQuestRewardDay
                {
                    localDate = dateKey
                };

                Wallet.days.Add(day);
            }

            day.focusedSeconds += session.accumulatedFocusedSeconds;

            double goalSeconds =
                Math.Max(0d, profile.dailyWorkGoalMinutes * 60d);

            if (goalSeconds <= 0d ||
                day.dailyBonusAwarded ||
                day.focusedSeconds < goalSeconds)
            {
                return;
            }

            AwardProgression(
                session,
                profile.dailyCopperBonus,
                profile.dailyExperienceBonus,
                "Daily Decree",
                "Daily focused-work decree fulfilled");

            day.dailyBonusAwarded = true;
        }

        private static void Award(
            DeverQuestSession session,
            DeverQuestRewardCategory category,
            double minutes,
            string transactionType,
            string note)
        {
            minutes = Math.Round(minutes, 1);

            if (minutes <= 0d)
            {
                return;
            }

            category.balanceMinutes += minutes;
            category.totalEarnedMinutes += minutes;

            session.rewardTransactions.Add(
                new DeverQuestRewardTransaction
                {
                    categoryName = category.displayName,
                    transactionType = transactionType,
                    minutes = minutes,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    note = note
                });
        }

        private static void AwardProgression(
            DeverQuestSession session,
            long copper,
            long experience,
            string transactionType,
            string note)
        {
            copper = Math.Max(0L, copper);
            experience = Math.Max(0L, experience);
            if (copper <= 0L && experience <= 0L)
            {
                return;
            }

            DeverQuestProgressionResult result =
                DeverQuestAdventurerService.Award(copper, experience);

            session.rewardTransactions.Add(
                new DeverQuestRewardTransaction
                {
                    categoryName = "Adventurer Progression",
                    transactionType = transactionType,
                    copper = copper,
                    experience = experience,
                    startingLevel = result.StartingLevel,
                    endingLevel = result.EndingLevel,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    note = note
                });
        }

        private static void EnsureDefaults()
        {
            if (wallet == null)
            {
                wallet = new DeverQuestRewardWallet();
            }

            wallet.Sanitize();

            if (!wallet.categories.Any(
                    category => category.categoryId == GameTimeId))
            {
                wallet.categories.Insert(
                    0,
                    new DeverQuestRewardCategory
                    {
                        categoryId = GameTimeId,
                        displayName = "Game Time",
                        rewardMinutesPerBlock = 10d,
                        dailyBonusMinutes = 30d,
                        isBuiltIn = true
                    });
            }

            if (!wallet.categories.Any(
                    category => category.categoryId == OtherFunId))
            {
                wallet.categories.Add(
                    new DeverQuestRewardCategory
                    {
                        categoryId = OtherFunId,
                        displayName = "Other Fun",
                        rewardMinutesPerBlock = 5d,
                        dailyBonusMinutes = 15d,
                        isBuiltIn = true
                    });
            }

            if (!wallet.legacyBalancesMigratedToCopper)
            {
                long legacyCopper = 0L;
                foreach (DeverQuestRewardCategory category
                         in wallet.categories)
                {
                    legacyCopper +=
                        (long)Math.Round(
                            Math.Max(0d, category.balanceMinutes));
                    category.balanceMinutes = 0d;
                }

                DeverQuestAdventurerService.MigrateLegacyCopper(
                    legacyCopper);
                wallet.legacyBalancesMigratedToCopper = true;
            }

            Save();
        }

        private static void Load()
        {
            string json =
                EditorPrefs.GetString(WalletKey, string.Empty);

            if (string.IsNullOrWhiteSpace(json))
            {
                wallet = new DeverQuestRewardWallet();
                return;
            }

            try
            {
                wallet =
                    JsonUtility.FromJson<DeverQuestRewardWallet>(json) ??
                    new DeverQuestRewardWallet();

                wallet.Sanitize();
            }
            catch
            {
                Debug.LogWarning(
                    "[DeverQuest] Reward wallet could not be loaded.");

                wallet = new DeverQuestRewardWallet();
            }
        }
    }
}

//----- DeverQuestRewardService.cs END -----
