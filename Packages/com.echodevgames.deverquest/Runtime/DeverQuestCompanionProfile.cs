using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewCompanionProfile",
        menuName = "DeverQuest/Companions/Companion Profile")]
    public sealed class DeverQuestCompanionProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string companionId = string.Empty;

        public string displayName = "New Companion";

        [TextArea(3, 8)]
        public string lore = string.Empty;

        public DeverQuestCompanionKind kind =
            DeverQuestCompanionKind.BondedBeast;
        public DeverQuestCompanionRole role =
            DeverQuestCompanionRole.Striker;
        public DeverQuestCreatureType creatureType =
            DeverQuestCreatureType.Beast;
        public int minimumAdventurerLevel = 1;
        public bool requiresCompanionClass = true;
        public List<string> allowedClassIds = new List<string>();
        public List<string> allowedClassNames = new List<string>();
        public int maximumHitPoints = 8;
        public int hitPointsPerLevel = 2;
        public int armorClass = 11;
        public int attackModifier = 2;
        public string damageDice = "1d4";
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Piercing;
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();

        [Range(0, 100)]
        public int startingLoyalty = 50;
        public int recruitCopperCost;
        public int recoveryCopperCost = 10;

        public string CompanionId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(companionId))
                {
                    companionId = Guid.NewGuid().ToString("N");
                }

                return companionId;
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
            _ = CompanionId;
            displayName = displayName?.Trim() ?? string.Empty;
            lore = lore?.Trim() ?? string.Empty;
            minimumAdventurerLevel =
                Mathf.Max(1, minimumAdventurerLevel);
            maximumHitPoints = Mathf.Max(1, maximumHitPoints);
            hitPointsPerLevel = Mathf.Max(0, hitPointsPerLevel);
            armorClass = Mathf.Max(1, armorClass);
            damageDice = damageDice?.Trim() ?? string.Empty;
            startingLoyalty = Mathf.Clamp(startingLoyalty, 0, 100);
            recruitCopperCost = Mathf.Max(0, recruitCopperCost);
            recoveryCopperCost = Mathf.Max(0, recoveryCopperCost);
            allowedClassIds = allowedClassIds ?? new List<string>();
            allowedClassNames =
                allowedClassNames ?? new List<string>();
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }
    }
}
