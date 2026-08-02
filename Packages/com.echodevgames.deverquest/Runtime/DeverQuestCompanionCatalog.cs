using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewCompanionCatalog",
        menuName = "DeverQuest/Companions/Companion Catalog")]
    public sealed class DeverQuestCompanionCatalog : ScriptableObject
    {
        public string displayName = "Guild Companion Stable";
        [TextArea(3, 8)]
        public string description = string.Empty;
        public List<DeverQuestCompanionProfile> companions =
            new List<DeverQuestCompanionProfile>();

        private void OnEnable()
        {
            companions = companions ??
                         new List<DeverQuestCompanionProfile>();
        }

        private void OnValidate()
        {
            displayName = displayName?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            companions = companions ??
                         new List<DeverQuestCompanionProfile>();
            companions.RemoveAll(value => value == null);
        }
    }
}
