using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestCreatureType
    {
        Humanoid = 0,
        Beast = 1,
        Undead = 2,
        Construct = 3,
        Dragonkin = 4,
        Giant = 5,
        Ooze = 6,
        Plant = 7,
        Elemental = 8,
        Fey = 9,
        Fiend = 10,
        Celestial = 11,
        Aberration = 12,
        Monstrosity = 13,
        Spirit = 14,
        Aquatic = 15,
        Insectoid = 16
    }

    public enum DeverQuestDamageType
    {
        Bludgeoning = 0,
        Piercing = 1,
        Slashing = 2,
        Fire = 3,
        Frost = 4,
        Lightning = 5,
        Acid = 6,
        Poison = 7,
        Arcane = 8,
        Radiant = 9,
        Shadow = 10,
        Psychic = 11,
        Sonic = 12,
        Force = 13
    }

    public enum DeverQuestDamageResponse
    {
        Normal = 0,
        Vulnerable = 1,
        Resistant = 2,
        Immune = 3,
        Absorbs = 4
    }

    [Serializable]
    public sealed class DeverQuestDamageAffinity
    {
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Bludgeoning;
        public DeverQuestDamageResponse response =
            DeverQuestDamageResponse.Resistant;
    }

    [CreateAssetMenu(
        fileName = "NewCombatTypeCatalog",
        menuName = "DeverQuest/Combat/Combat Type Catalog")]
    public sealed class DeverQuestCombatTypeCatalog : ScriptableObject
    {
        public string displayName = "Guild Combat Codex";
        [TextArea(3, 8)]
        public string description =
            "The Guild's approved creature families, damage types, " +
            "and resistance rules.";
        public List<DeverQuestCreatureType> creatureTypes =
            new List<DeverQuestCreatureType>();
        public List<DeverQuestDamageType> damageTypes =
            new List<DeverQuestDamageType>();

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
            displayName = displayName?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            creatureTypes = creatureTypes ??
                            new List<DeverQuestCreatureType>();
            damageTypes = damageTypes ??
                          new List<DeverQuestDamageType>();
        }
    }
}
