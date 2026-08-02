using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestAbility
    {
        Strength = 0,
        Dexterity = 1,
        Constitution = 2,
        Intelligence = 3,
        Wisdom = 4,
        Charisma = 5,
        Agility = 6,
        Stamina = 7,
        Luck = 8
    }

    public enum DeverQuestEquipmentFamily
    {
        Unknown = 0,
        Armor = 1,
        Sword = 2,
        Axe = 3,
        Mace = 4,
        Hammer = 5,
        Dagger = 6,
        Staff = 7,
        Wand = 8,
        Spear = 9,
        Polearm = 10,
        Bow = 11,
        Shield = 12,
        Tool = 13,
        Trinket = 14,
        Clothing = 15,
        Other = 16
    }

    public enum DeverQuestEquipmentSlot
    {
        Helm = 0,
        Chest = 1,
        Hands = 2,
        Boots = 3,
        MainHand = 4,
        OffHand = 5,
        Trinket = 6,
        Face = 7,
        Neck = 8,
        EarLeft = 9,
        EarRight = 10,
        Shoulders = 11,
        Back = 12,
        Legs = 13,
        Belt = 14,
        WristLeft = 15,
        WristRight = 16,
        Shirt = 17,
        RingLeft = 18,
        RingRight = 19
    }

    [CreateAssetMenu(
        fileName = "NewDeverQuestEquipment",
        menuName = "DeverQuest/Character/Equipment")]
    public sealed class DeverQuestEquipment : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string equipmentId = string.Empty;
        public string displayName = "New Equipment";
        [TextArea(2, 5)]
        public string description = string.Empty;
        [Header("Classification")]
        public DeverQuestEquipmentSlot slot;
        public DeverQuestEquipmentFamily equipmentFamily =
            DeverQuestEquipmentFamily.Unknown;
        public bool twoHanded;
        public string requiredSkillId = string.Empty;
        public List<string> tags = new List<string>();

        [Header("Rules")]
        public int armorClassBonus;
        public DeverQuestAbility abilityBonusType;
        public int abilityBonus;
        public int minimumLevel = 1;
        public string materialTier = "Copper";
        public string rarity = "Common";
        public int copperValue = 10;
        [Min(0f)]
        public float weight = 1f;
        public string damageDice = string.Empty;
        public DeverQuestDamageType weaponDamageType =
            DeverQuestDamageType.Slashing;
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();

        public string EquipmentId
        {
            get
            {
                EnsureId();
                return equipmentId;
            }
        }

        private void OnEnable()
        {
            EnsureId();
            minimumLevel = Mathf.Max(1, minimumLevel);
            copperValue = Mathf.Max(0, copperValue);
            weight = Mathf.Max(0f, weight);
            damageDice = damageDice?.Trim() ?? string.Empty;
            requiredSkillId =
                requiredSkillId?.Trim() ?? string.Empty;
            tags = tags ?? new List<string>();
            tags.RemoveAll(value =>
                string.IsNullOrWhiteSpace(value));
            for (int index = 0; index < tags.Count; index++)
            {
                tags[index] = tags[index].Trim();
            }
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }

        private void OnValidate()
        {
            EnsureId();
            minimumLevel = Mathf.Max(1, minimumLevel);
            copperValue = Mathf.Max(0, copperValue);
            weight = Mathf.Max(0f, weight);
            damageDice = damageDice?.Trim() ?? string.Empty;
            requiredSkillId =
                requiredSkillId?.Trim() ?? string.Empty;
            tags = tags ?? new List<string>();
            tags.RemoveAll(value =>
                string.IsNullOrWhiteSpace(value));
            for (int index = 0; index < tags.Count; index++)
            {
                tags[index] = tags[index].Trim();
            }
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }

        private void EnsureId()
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                equipmentId = Guid.NewGuid().ToString("N");
            }
        }
    }

    }
