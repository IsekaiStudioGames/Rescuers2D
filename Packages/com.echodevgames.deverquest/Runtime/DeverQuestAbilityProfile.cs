using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewAbilityProfile",
        menuName = "DeverQuest/Combat/Ability Profile")]
    public sealed class DeverQuestAbilityProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string profileId = string.Empty;
        public string displayName = "New Ability Profile";
        [TextArea(2, 6)]
        public string description = string.Empty;
        public DeverQuestTacticalStyle tacticalStyle =
            DeverQuestTacticalStyle.Balanced;
        public List<DeverQuestAbilitySlot> abilities =
            new List<DeverQuestAbilitySlot>();

        public string ProfileId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(profileId))
                {
                    profileId = Guid.NewGuid().ToString("N");
                }
                return profileId;
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
            _ = ProfileId;
            displayName = displayName?.Trim() ?? string.Empty;
            abilities = abilities ?? new List<DeverQuestAbilitySlot>();
            abilities.RemoveAll(value => value == null);
            foreach (DeverQuestAbilitySlot ability in abilities)
            {
                ability.priority = Mathf.Clamp(
                    ability.priority, 0, 100);
                ability.useBelowHitPointPercent = Mathf.Clamp(
                    ability.useBelowHitPointPercent, 0, 100);
            }
        }
    }
}
