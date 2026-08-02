using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewStarterLoadout",
        menuName = "DeverQuest/Character/Starter Loadout")]
    public sealed class DeverQuestStarterLoadout : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string loadoutId = string.Empty;
        public string displayName = "New Starter Loadout";
        public string characterClass = "Warrior";
        public string department = "Programming";
        public List<DeverQuestEquipment> equipment =
            new List<DeverQuestEquipment>();
        public List<DeverQuestSpell> spells =
            new List<DeverQuestSpell>();

        public string LoadoutId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(loadoutId))
                {
                    loadoutId = Guid.NewGuid().ToString("N");
                }
                return loadoutId;
            }
        }

        private void OnEnable()
        {
            _ = LoadoutId;
        }

        private void OnValidate()
        {
            _ = LoadoutId;
            displayName = displayName?.Trim() ?? string.Empty;
            characterClass = characterClass?.Trim() ?? string.Empty;
            department = department?.Trim() ?? string.Empty;
        }
    }
}
