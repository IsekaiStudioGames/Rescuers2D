using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestCombatTarget
    {
        Enemy = 0,
        Self = 1,
        Ally = 2,
        AllEnemies = 3,
        AllAllies = 4
    }

    public enum DeverQuestCombatEffectType
    {
        DirectDamage = 0,
        DamageOverTime = 1,
        Heal = 2,
        HealOverTime = 3,
        LifeDrain = 4,
        ManaRestore = 5,
        Root = 6,
        Snare = 7,
        Stun = 8,
        Silence = 9,
        Shield = 10,
        AttackBuff = 11,
        AttackDebuff = 12,
        ArmorBuff = 13,
        ArmorDebuff = 14,
        Cleanse = 15,
        Dispel = 16,
        ReturnToGuild = 17
    }

    public enum DeverQuestTacticalStyle
    {
        Balanced = 0,
        Aggressive = 1,
        Defensive = 2,
        Support = 3,
        Controller = 4
    }

    [Serializable]
    public sealed class DeverQuestCombatEffect
    {
        public DeverQuestCombatEffectType effectType =
            DeverQuestCombatEffectType.DirectDamage;
        public DeverQuestCombatTarget target =
            DeverQuestCombatTarget.Enemy;
        public string dice = "1d6";
        public int flatAmount;
        public int durationRounds = 1;
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Arcane;
        public bool saveNegates;
        public DeverQuestAbility savingAbility =
            DeverQuestAbility.Constitution;
        public int difficultyClass = 10;
        public bool breaksOnDamage;

        public void Sanitize()
        {
            dice = dice?.Trim() ?? string.Empty;
            durationRounds = Mathf.Max(1, durationRounds);
            difficultyClass = Mathf.Max(1, difficultyClass);
        }
    }

    [CreateAssetMenu(
        fileName = "NewCombatTechnique",
        menuName = "DeverQuest/Combat/Attack Technique")]
    public sealed class DeverQuestAttackTechnique : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string techniqueId = string.Empty;
        public string displayName = "New Technique";
        [TextArea(2, 6)]
        public string description = string.Empty;
        public DeverQuestAbility attackAbility =
            DeverQuestAbility.Strength;
        public int minimumCharacterLevel = 1;
        public int manaCost;
        public int cooldownRounds;
        public List<DeverQuestCombatEffect> effects =
            new List<DeverQuestCombatEffect>();

        public string TechniqueId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(techniqueId))
                {
                    techniqueId = Guid.NewGuid().ToString("N");
                }
                return techniqueId;
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
            _ = TechniqueId;
            displayName = displayName?.Trim() ?? string.Empty;
            minimumCharacterLevel =
                Mathf.Max(1, minimumCharacterLevel);
            manaCost = Mathf.Max(0, manaCost);
            cooldownRounds = Mathf.Max(0, cooldownRounds);
            effects = effects ?? new List<DeverQuestCombatEffect>();
            foreach (DeverQuestCombatEffect effect in effects)
            {
                effect?.Sanitize();
            }
        }
    }

    [Serializable]
    public sealed class DeverQuestAbilitySlot
    {
        public DeverQuestSpell spell;
        public DeverQuestAttackTechnique technique;
        [Range(0, 100)]
        public int priority = 50;
        [Range(0, 100)]
        public int useBelowHitPointPercent = 100;
        public bool maintainEffect;
    }

    }
