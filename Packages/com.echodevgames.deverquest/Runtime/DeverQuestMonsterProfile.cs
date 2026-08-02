using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewMonsterProfile",
        menuName = "DeverQuest/Encounters/Monster Profile")]
    public sealed class DeverQuestMonsterProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string monsterId = string.Empty;

        public string displayName = "New Monster";

        [TextArea(2, 6)]
        public string description = string.Empty;

        public int level = 1;
        public int maximumHitPoints = 8;
        public int armorClass = 10;
        public int attackModifier = 2;
        public string damageDice = "1d4";
        public DeverQuestDamageType attackDamageType =
            DeverQuestDamageType.Bludgeoning;
        public DeverQuestCreatureType creatureType =
            DeverQuestCreatureType.Humanoid;
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();
        public int initiativeModifier;
        public int victoryCopper = 5;
        public int victoryExperience = 10;
        public DeverQuestAbilityProfile abilityProfile;
        public List<DeverQuestDropEntry> dropTable =
            new List<DeverQuestDropEntry>();

        public string MonsterId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(monsterId))
                {
                    monsterId = Guid.NewGuid().ToString("N");
                }

                return monsterId;
            }
        }

        private void OnEnable()
        {
            Sanitize();
        }

        private void OnValidate()
        {
            Sanitize();
        }

        private void Sanitize()
        {
            _ = MonsterId;
            displayName = displayName?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            level = Mathf.Max(1, level);
            maximumHitPoints = Mathf.Max(1, maximumHitPoints);
            armorClass = Mathf.Max(1, armorClass);
            damageDice = damageDice?.Trim() ?? string.Empty;
            victoryCopper = Mathf.Max(0, victoryCopper);
            victoryExperience = Mathf.Max(0, victoryExperience);
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
            dropTable = dropTable ?? new List<DeverQuestDropEntry>();
        }
    }
}
