using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestArchivedBattle
    {
        public string archiveId = string.Empty;
        public string sessionId = string.Empty;
        public string questContractId = string.Empty;
        public string questRunId = string.Empty;
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string developerName = string.Empty;
        public string adventurerName = string.Empty;
        public long archivedUtcTicks;
        public DeverQuestBattleResult battle =
            new DeverQuestBattleResult();

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(archiveId))
            {
                archiveId = Guid.NewGuid().ToString("N");
            }
            sessionId = sessionId?.Trim() ?? string.Empty;
            questContractId = questContractId?.Trim() ?? string.Empty;
            questRunId = questRunId?.Trim() ?? string.Empty;
            projectName = projectName?.Trim() ?? string.Empty;
            taskName = taskName?.Trim() ?? string.Empty;
            developerName = developerName?.Trim() ?? string.Empty;
            adventurerName = adventurerName?.Trim() ?? string.Empty;
            archivedUtcTicks = Math.Max(0L, archivedUtcTicks);
            battle = battle ?? new DeverQuestBattleResult();
            battle.defeatedMonsters = battle.defeatedMonsters ??
                                      new List<string>();
            battle.loot = battle.loot ?? new List<string>();
            battle.combatLog = battle.combatLog ??
                               new List<string>();
            battle.damageEvents = battle.damageEvents ??
                                  new List<DeverQuestDamageEvent>();
            battle.actionEvents = battle.actionEvents ??
                                  new List<DeverQuestCombatActionEvent>();
            battle.encounterName =
                battle.encounterName?.Trim() ?? string.Empty;
            battle.encounterId =
                battle.encounterId?.Trim() ?? string.Empty;
            battle.companionName =
                battle.companionName?.Trim() ?? string.Empty;
            battle.seed = battle.seed?.Trim() ?? string.Empty;
        }
    }

    [Serializable]
    internal sealed class DeverQuestTacticalArchiveData
    {
        public List<DeverQuestArchivedBattle> records =
            new List<DeverQuestArchivedBattle>();

        public void Sanitize()
        {
            records = records ?? new List<DeverQuestArchivedBattle>();
            records.RemoveAll(value => value == null);
            foreach (DeverQuestArchivedBattle record in records)
            {
                record.Sanitize();
            }
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestTacticalArchiveService
    {
        private const int MaximumRecords = 100;

        private static string ArchivePath => Path.GetFullPath(
            Path.Combine(
                Application.dataPath,
                "..",
                "Library",
                "DeverQuest",
                "TacticalArchive.json"));

        private static DeverQuestTacticalArchiveData data;

        static DeverQuestTacticalArchiveService()
        {
            Load();
        }

        public static IReadOnlyList<DeverQuestArchivedBattle> Records
        {
            get
            {
                EnsureLoaded();
                return data.records
                    .OrderByDescending(value => value.archivedUtcTicks)
                    .ToList();
            }
        }

        public static void Record(
            DeverQuestSession session,
            DeverQuestBattleResult battle)
        {
            if (session == null || battle == null)
            {
                return;
            }

            try
            {
                EnsureLoaded();
                string identity = Identity(session, battle);
                DeverQuestArchivedBattle existing =
                    data.records.FirstOrDefault(value =>
                        string.Equals(
                            Identity(value),
                            identity,
                            StringComparison.Ordinal));

                DeverQuestArchivedBattle record = existing ??
                    new DeverQuestArchivedBattle();
                record.sessionId = session.sessionId;
                record.questContractId = session.questContractId;
                record.questRunId = session.questContractRunId;
                record.projectName = session.projectName;
                record.taskName = session.taskName;
                record.developerName = session.developerName;
                record.adventurerName =
                    DeverQuestAdventurerService.Adventurer.characterName;
                record.archivedUtcTicks = battle.resolvedUtcTicks > 0L
                    ? battle.resolvedUtcTicks
                    : DateTime.UtcNow.Ticks;
                record.battle = CloneBattle(battle);
                record.Sanitize();

                if (existing == null)
                {
                    data.records.Add(record);
                }
                TrimAndSave();
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest Tactical Archive] Battle recording " +
                    "failed without interrupting the Encounter: " +
                    exception.Message);
            }
        }

        public static int ImportSession(DeverQuestSession session)
        {
            if (session?.battleResults == null)
            {
                return 0;
            }

            int before = Records.Count;
            foreach (DeverQuestBattleResult battle in
                     session.battleResults.Where(value => value != null))
            {
                Record(session, battle);
            }
            return Math.Max(0, Records.Count - before);
        }

        public static bool Remove(string archiveId)
        {
            EnsureLoaded();
            int removed = data.records.RemoveAll(value =>
                value != null &&
                string.Equals(
                    value.archiveId,
                    archiveId,
                    StringComparison.Ordinal));
            if (removed <= 0)
            {
                return false;
            }
            Save();
            return true;
        }

        public static void Clear()
        {
            data = new DeverQuestTacticalArchiveData();
            try
            {
                if (File.Exists(ArchivePath))
                {
                    File.Delete(ArchivePath);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest Tactical Archive] The local archive " +
                    "file could not be removed: " + exception.Message);
            }
        }

        public static bool CanWrite(out string reason)
        {
            reason = string.Empty;
            try
            {
                string directory = Path.GetDirectoryName(ArchivePath);
                if (string.IsNullOrWhiteSpace(directory))
                {
                    reason = "The Tactical Archive directory could not be " +
                             "resolved.";
                    return false;
                }
                Directory.CreateDirectory(directory);
                string probePath = Path.Combine(
                    directory,
                    "TacticalArchive.probe");
                File.WriteAllText(probePath, "ok");
                bool success = string.Equals(
                    File.ReadAllText(probePath),
                    "ok",
                    StringComparison.Ordinal);
                File.Delete(probePath);
                if (!success)
                {
                    reason = "The Tactical Archive write probe did not " +
                             "round-trip through the Library folder.";
                }
                return success;
            }
            catch (Exception exception)
            {
                reason = exception.Message;
                return false;
            }
        }

        private static DeverQuestBattleResult CloneBattle(
            DeverQuestBattleResult battle)
        {
            string json = JsonUtility.ToJson(battle);
            return JsonUtility.FromJson<DeverQuestBattleResult>(json) ??
                   new DeverQuestBattleResult();
        }

        private static string Identity(
            DeverQuestSession session,
            DeverQuestBattleResult battle)
        {
            return (session.sessionId ?? string.Empty) + "|" +
                   (battle.stageId ?? string.Empty) + "|" +
                   battle.survivalWave + "|" +
                   (battle.seed ?? string.Empty) + "|" +
                   battle.resolvedUtcTicks;
        }

        private static string Identity(
            DeverQuestArchivedBattle record)
        {
            DeverQuestBattleResult battle =
                record?.battle ?? new DeverQuestBattleResult();
            return (record?.sessionId ?? string.Empty) + "|" +
                   (battle.stageId ?? string.Empty) + "|" +
                   battle.survivalWave + "|" +
                   (battle.seed ?? string.Empty) + "|" +
                   battle.resolvedUtcTicks;
        }

        private static void EnsureLoaded()
        {
            if (data == null)
            {
                Load();
            }
        }

        private static void Load()
        {
            try
            {
                string json = File.Exists(ArchivePath)
                    ? File.ReadAllText(ArchivePath)
                    : string.Empty;
                if (string.IsNullOrWhiteSpace(json))
                {
                    data = new DeverQuestTacticalArchiveData();
                    return;
                }

                data = JsonUtility.FromJson<
                    DeverQuestTacticalArchiveData>(json) ??
                       new DeverQuestTacticalArchiveData();
            }
            catch (Exception exception)
            {
                data = new DeverQuestTacticalArchiveData();
                Debug.LogWarning(
                    "[DeverQuest Tactical Archive] The local archive " +
                    "could not be loaded and was reset in memory: " +
                    exception.Message);
            }
            data.Sanitize();
        }

        private static void TrimAndSave()
        {
            data.records = data.records
                .Where(value => value != null)
                .OrderByDescending(value => value.archivedUtcTicks)
                .Take(MaximumRecords)
                .ToList();
            Save();
        }

        private static void Save()
        {
            EnsureLoaded();
            data.Sanitize();
            string temporaryPath = ArchivePath + ".tmp";
            try
            {
                string directory = Path.GetDirectoryName(ArchivePath);
                if (string.IsNullOrWhiteSpace(directory))
                {
                    return;
                }
                Directory.CreateDirectory(directory);
                File.WriteAllText(
                    temporaryPath,
                    JsonUtility.ToJson(data, true));
                if (File.Exists(ArchivePath))
                {
                    File.Delete(ArchivePath);
                }
                File.Move(temporaryPath, ArchivePath);
            }
            catch (Exception exception)
            {
                if (File.Exists(temporaryPath))
                {
                    try
                    {
                        File.Delete(temporaryPath);
                    }
                    catch
                    {
                    }
                }
                Debug.LogWarning(
                    "[DeverQuest Tactical Archive] The local archive " +
                    "could not be saved: " + exception.Message);
            }
        }
    }
}
