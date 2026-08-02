using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestCompanionKind
    {
        BondedBeast = 0,
        Familiar = 1,
        BoundMinion = 2,
        Spirit = 3,
        Construct = 4,
        Mercenary = 5
    }

    public enum DeverQuestCompanionRole
    {
        Striker = 0,
        Guardian = 1,
        Support = 2,
        Controller = 3
    }

    [Serializable]
    public sealed class DeverQuestCompanionState
    {
        public string instanceId = string.Empty;
        public string profileId = string.Empty;
        public string customName = string.Empty;
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public int currentHitPoints;
        [Range(0, 100)]
        public int loyalty = 50;
        public bool isActive;
        public bool isFallen;
        public int battles;
        public int victories;
        public long lifetimeDamageDealt;
        public long lifetimeDamageTaken;
        public long lifetimeHealingDone;
        public string lastBattleSummary = string.Empty;
        public string lastBattleUtc = string.Empty;
        public string recruitedUtc = string.Empty;

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                instanceId = Guid.NewGuid().ToString("N");
            }
            profileId = profileId?.Trim() ?? string.Empty;
            customName = customName?.Trim() ?? string.Empty;
            level = Mathf.Max(1, level);
            currentExperience = Math.Max(0L, currentExperience);
            lifetimeExperience = Math.Max(0L, lifetimeExperience);
            currentHitPoints = Mathf.Max(0, currentHitPoints);
            loyalty = Mathf.Clamp(loyalty, 0, 100);
            battles = Mathf.Max(0, battles);
            victories = Mathf.Max(0, victories);
            lifetimeDamageDealt = Math.Max(0L, lifetimeDamageDealt);
            lifetimeDamageTaken = Math.Max(0L, lifetimeDamageTaken);
            lifetimeHealingDone = Math.Max(0L, lifetimeHealingDone);
            lastBattleSummary = lastBattleSummary?.Trim() ?? string.Empty;
            lastBattleUtc = lastBattleUtc?.Trim() ?? string.Empty;
            recruitedUtc = recruitedUtc?.Trim() ?? string.Empty;
        }
    }
}
