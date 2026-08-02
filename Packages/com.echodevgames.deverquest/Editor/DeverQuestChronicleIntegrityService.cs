using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestIntegrityStatus
    {
        Legacy = 0,
        Verified = 1,
        Modified = 2,
        Unavailable = 3
    }

    [Serializable]
    internal sealed class DeverQuestAuditEvent
    {
        public string eventId = string.Empty;
        public string eventType = string.Empty;
        public string createdUtc = string.Empty;
        public string actor = string.Empty;
        public string recordHash = string.Empty;
        public string correctionHash = string.Empty;
        public string previousEventHash = string.Empty;
        public string eventHash = string.Empty;
        public string note = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestAuditJournal
    {
        public int formatVersion = 1;
        public string dataFileName = string.Empty;
        public List<DeverQuestAuditEvent> events =
            new List<DeverQuestAuditEvent>();
    }

    [Serializable]
    internal sealed class DeverQuestCorrection
    {
        public string correctionId = string.Empty;
        public string sessionId = string.Empty;
        public string sessionTitle = string.Empty;
        public string requestedBy = string.Empty;
        public string requestedUtc = string.Empty;
        public string reason = string.Empty;
        public string correctedValue = string.Empty;
        public string status = "Pending";
        public string reviewedBy = string.Empty;
        public string reviewedUtc = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestCorrectionJournal
    {
        public int formatVersion = 1;
        public List<DeverQuestCorrection> corrections =
            new List<DeverQuestCorrection>();
    }

    internal static class DeverQuestChronicleIntegrityService
    {
        public static string AuditPath(string dataPath)
        {
            return dataPath + ".audit.json";
        }

        public static string CorrectionPath(string dataPath)
        {
            return dataPath + ".corrections.json";
        }

        public static void Seal(
            string dataPath,
            string actor,
            string eventType,
            string note)
        {
            if (!File.Exists(dataPath))
            {
                return;
            }

            string auditPath = AuditPath(dataPath);
            DeverQuestAuditJournal journal = LoadAudit(auditPath);
            string previous = journal.events.Count == 0
                ? string.Empty
                : journal.events[journal.events.Count - 1].eventHash;
            DeverQuestAuditEvent entry = new DeverQuestAuditEvent
            {
                eventId = Guid.NewGuid().ToString("N"),
                eventType = eventType ?? "Chronicle Sealed",
                createdUtc = DateTime.UtcNow.ToString("O"),
                actor = actor ?? string.Empty,
                recordHash = HashFile(dataPath),
                correctionHash =
                    File.Exists(CorrectionPath(dataPath))
                        ? HashFile(CorrectionPath(dataPath))
                        : string.Empty,
                previousEventHash = previous,
                note = note ?? string.Empty
            };
            entry.eventHash = HashText(Canonical(entry));
            journal.dataFileName = Path.GetFileName(dataPath);
            journal.events.Add(entry);
            WriteJson(auditPath, journal);
        }

        public static DeverQuestIntegrityStatus Verify(
            string dataPath,
            out string message)
        {
            message = string.Empty;
            if (!File.Exists(dataPath))
            {
                message = "Chronicle data file is unavailable.";
                return DeverQuestIntegrityStatus.Unavailable;
            }

            string auditPath = AuditPath(dataPath);
            if (!File.Exists(auditPath))
            {
                message = "Created before integrity seals were enabled.";
                return DeverQuestIntegrityStatus.Legacy;
            }

            try
            {
                DeverQuestAuditJournal journal = LoadAudit(auditPath);
                if (journal.events.Count == 0)
                {
                    message = "Audit journal is empty.";
                    return DeverQuestIntegrityStatus.Modified;
                }

                string previous = string.Empty;
                foreach (DeverQuestAuditEvent entry in journal.events)
                {
                    if (entry.previousEventHash != previous ||
                        entry.eventHash != HashText(Canonical(entry)))
                    {
                        message = "Audit hash chain does not match.";
                        return DeverQuestIntegrityStatus.Modified;
                    }
                    previous = entry.eventHash;
                }

                if (journal.events[journal.events.Count - 1].recordHash !=
                    HashFile(dataPath))
                {
                    message = "Chronicle changed after its last seal.";
                    return DeverQuestIntegrityStatus.Modified;
                }
                DeverQuestAuditEvent latest =
                    journal.events[journal.events.Count - 1];
                string currentCorrectionHash =
                    File.Exists(CorrectionPath(dataPath))
                        ? HashFile(CorrectionPath(dataPath))
                        : string.Empty;
                if (latest.correctionHash != currentCorrectionHash)
                {
                    message = "Correction journal changed after its last seal.";
                    return DeverQuestIntegrityStatus.Modified;
                }

                message = $"{journal.events.Count} chained audit event(s).";
                return DeverQuestIntegrityStatus.Verified;
            }
            catch (Exception exception)
            {
                message = exception.Message;
                return DeverQuestIntegrityStatus.Unavailable;
            }
        }

        public static List<DeverQuestCorrection> LoadCorrections(
            string dataPath)
        {
            string path = CorrectionPath(dataPath);
            if (!File.Exists(path))
            {
                return new List<DeverQuestCorrection>();
            }
            DeverQuestCorrectionJournal journal =
                JsonUtility.FromJson<DeverQuestCorrectionJournal>(
                    File.ReadAllText(path));
            return journal?.corrections ??
                   new List<DeverQuestCorrection>();
        }

        public static void AddCorrection(
            string dataPath,
            string sessionId,
            string sessionTitle,
            string actor,
            string reason,
            string correctedValue)
        {
            string path = CorrectionPath(dataPath);
            DeverQuestCorrectionJournal journal =
                File.Exists(path)
                    ? JsonUtility.FromJson<DeverQuestCorrectionJournal>(
                        File.ReadAllText(path))
                    : new DeverQuestCorrectionJournal();
            if (journal == null)
            {
                journal = new DeverQuestCorrectionJournal();
            }
            if (journal.corrections == null)
            {
                journal.corrections =
                    new List<DeverQuestCorrection>();
            }
            journal.corrections.Add(new DeverQuestCorrection
            {
                correctionId = Guid.NewGuid().ToString("N"),
                sessionId = sessionId ?? string.Empty,
                sessionTitle = sessionTitle ?? string.Empty,
                requestedBy = actor ?? string.Empty,
                requestedUtc = DateTime.UtcNow.ToString("O"),
                reason = reason?.Trim() ?? string.Empty,
                correctedValue = correctedValue?.Trim() ?? string.Empty
            });
            WriteJson(path, journal);
            Seal(dataPath, actor, "Correction Requested", reason);
        }

        public static void ReviewCorrection(
            string dataPath,
            string correctionId,
            string status,
            string actor,
            string projectName)
        {
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ReviewCorrections,
                    projectName))
            {
                return;
            }
            string path = CorrectionPath(dataPath);
            DeverQuestCorrectionJournal journal =
                JsonUtility.FromJson<DeverQuestCorrectionJournal>(
                    File.ReadAllText(path));
            DeverQuestCorrection correction =
                journal?.corrections?.FirstOrDefault(
                    item => item.correctionId == correctionId);
            if (correction == null)
            {
                return;
            }
            correction.status = status;
            correction.reviewedBy = actor ?? string.Empty;
            correction.reviewedUtc = DateTime.UtcNow.ToString("O");
            WriteJson(path, journal);
            Seal(dataPath, actor, "Correction " + status,
                correction.reason);
            DeverQuestGuildAccountService.AddAudit(
                "Correction " + status,
                correction.sessionTitle,
                projectName);
        }

        public static int GetRequestedChronicleIndex(
            string developerFolder,
            string dateKey)
        {
            string marker = Path.Combine(
                developerFolder, $".{dateKey}.chronicle");
            if (!File.Exists(marker))
            {
                return 1;
            }
            return int.TryParse(File.ReadAllText(marker), out int value)
                ? Math.Max(1, value)
                : 1;
        }

        public static int StartNewChronicle(
            string developerFolder,
            string dateKey)
        {
            Directory.CreateDirectory(developerFolder);
            int next = GetRequestedChronicleIndex(
                developerFolder, dateKey) + 1;
            File.WriteAllText(
                Path.Combine(developerFolder, $".{dateKey}.chronicle"),
                next.ToString(CultureInfo.InvariantCulture));
            return next;
        }

        private static DeverQuestAuditJournal LoadAudit(string path)
        {
            if (!File.Exists(path))
            {
                return new DeverQuestAuditJournal();
            }
            DeverQuestAuditJournal result =
                JsonUtility.FromJson<DeverQuestAuditJournal>(
                    File.ReadAllText(path));
            return result ?? new DeverQuestAuditJournal();
        }

        private static string Canonical(DeverQuestAuditEvent entry)
        {
            return string.Join("\n",
                entry.eventId, entry.eventType, entry.createdUtc,
                entry.actor, entry.recordHash, entry.previousEventHash,
                entry.correctionHash, entry.note);
        }

        private static string HashFile(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                return ToHex(sha.ComputeHash(stream));
            }
        }

        private static string HashText(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return ToHex(sha.ComputeHash(
                    Encoding.UTF8.GetBytes(value ?? string.Empty)));
            }
        }

        private static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        private static void WriteJson<T>(string path, T value)
        {
            File.WriteAllText(path, JsonUtility.ToJson(value, true),
                new UTF8Encoding(false));
        }
    }
}
