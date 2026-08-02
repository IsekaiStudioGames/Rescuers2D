using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewDeverQuestSpell",
        menuName = "DeverQuest/Character/Spell")]
    public sealed class DeverQuestSpell : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string spellId = string.Empty;
        public string displayName = "New Spell";
        [TextArea(2, 5)]
        public string description = string.Empty;
        public int spellLevel;
        public DeverQuestAbility castingAbility =
            DeverQuestAbility.Intelligence;
        public string damageDice = "1d6";
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Arcane;
        public string statusEffect = string.Empty;
        public int minimumCharacterLevel = 1;
        public int manaCost;
        public int cooldownRounds;
        public DeverQuestCombatTarget target =
            DeverQuestCombatTarget.Enemy;
        public List<DeverQuestCombatEffect> effects =
            new List<DeverQuestCombatEffect>();

        public string SpellId
        {
            get
            {
                EnsureId();
                return spellId;
            }
        }

        private void OnEnable()
        {
            EnsureId();
            Sanitize();
        }

        private void OnValidate()
        {
            EnsureId();
            Sanitize();
        }

        private void EnsureId()
        {
            if (string.IsNullOrWhiteSpace(spellId))
            {
                spellId = Guid.NewGuid().ToString("N");
            }
        }

        private void Sanitize()
        {
            spellLevel = Mathf.Max(0, spellLevel);
            minimumCharacterLevel =
                Mathf.Max(1, minimumCharacterLevel);
            damageDice = damageDice?.Trim() ?? string.Empty;
            statusEffect = statusEffect?.Trim() ?? string.Empty;
            manaCost = Mathf.Max(0, manaCost);
            cooldownRounds = Mathf.Max(0, cooldownRounds);
            effects = effects ?? new List<DeverQuestCombatEffect>();
            foreach (DeverQuestCombatEffect effect in effects)
            {
                effect?.Sanitize();
            }
        }
    }
}
