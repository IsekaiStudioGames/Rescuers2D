//----- DeverQuestQuestProfile.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewDeverQuestProfile",
        menuName = "DeverQuest/Quest Profile")]
    public sealed class DeverQuestQuestProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string profileId = string.Empty;

        [Header("Quest Identity")]
        public string displayName = "New Quest Profile";
        [TextArea(2, 5)]
        public string description = string.Empty;
        public bool availableToMembers = true;
        public int minimumAdventurerLevel = 1;

        [Header("Quest Defaults")]
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string department = "Programming";
        [TextArea(3, 8)]
        public string goalTemplate = string.Empty;
        [InspectorName("Predicted Task Length (Minutes)")]
        public int suggestedFocusMinutes = 50;

        [Header("Spoils")]
        public int baseCopper = 10;
        public int baseExperience = 10;
        public int workBlockMinutes = 30;
        public int copperPerWorkBlock = 25;
        public int experiencePerWorkBlock = 50;

        public string ProfileId
        {
            get
            {
                EnsureId();
                return profileId;
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
            if (string.IsNullOrWhiteSpace(profileId))
            {
                profileId = Guid.NewGuid().ToString("N");
            }
        }

        private void Sanitize()
        {
            displayName = displayName?.Trim() ?? string.Empty;
            projectName = projectName?.Trim() ?? string.Empty;
            taskName = taskName?.Trim() ?? string.Empty;
            department = department?.Trim() ?? string.Empty;
            minimumAdventurerLevel =
                Mathf.Max(1, minimumAdventurerLevel);
            suggestedFocusMinutes = Mathf.Max(1, suggestedFocusMinutes);
            baseCopper = Mathf.Max(0, baseCopper);
            baseExperience = Mathf.Max(0, baseExperience);
            workBlockMinutes = Mathf.Max(1, workBlockMinutes);
            copperPerWorkBlock = Mathf.Max(0, copperPerWorkBlock);
            experiencePerWorkBlock =
                Mathf.Max(0, experiencePerWorkBlock);
        }
    }
}

//----- DeverQuestQuestProfile.cs END -----
