using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewEncounterProfile",
        menuName = "DeverQuest/Encounters/Encounter Profile")]
    public sealed class DeverQuestEncounterProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string encounterId = string.Empty;
        public string displayName = "New Encounter";
        [TextArea(3, 8)]
        public string storyIntroduction = string.Empty;
        public List<DeverQuestEncounterWave> waves =
            new List<DeverQuestEncounterWave>();
        public bool allowInjury = true;
        public bool allowCharacterDeath;
        public int victoryCopperBonus;
        public int victoryExperienceBonus;
        [Header("Pace and Survival")]
        public DeverQuestEncounterMode encounterMode =
            DeverQuestEncounterMode.Fixed;
        public int parRounds = 6;
        public int earlyVictoryCopperBonus;
        public int earlyVictoryExperienceBonus;
        public int survivalWaveMinutes = 15;
        public int difficultyIncreaseEveryWaves = 2;
        public int wagonOfferEveryWaves = 3;
        public int survivalCopperGrowthPerWave = 2;
        public int survivalExperienceGrowthPerWave = 5;
        [Range(5, 90)]
        public int lowHitPointPausePercent = 25;
        public bool pauseWhenEncumbered = true;

        public string EncounterId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(encounterId))
                {
                    encounterId = Guid.NewGuid().ToString("N");
                }
                return encounterId;
            }
        }

        private void OnValidate()
        {
            _ = EncounterId;
            waves = waves ?? new List<DeverQuestEncounterWave>();
            foreach (DeverQuestEncounterWave wave in waves)
            {
                if (wave != null)
                {
                    wave.count = Mathf.Max(1, wave.count);
                }
            }
            victoryCopperBonus = Mathf.Max(0, victoryCopperBonus);
            victoryExperienceBonus =
                Mathf.Max(0, victoryExperienceBonus);
            parRounds = Mathf.Max(1, parRounds);
            earlyVictoryCopperBonus =
                Mathf.Max(0, earlyVictoryCopperBonus);
            earlyVictoryExperienceBonus =
                Mathf.Max(0, earlyVictoryExperienceBonus);
            survivalWaveMinutes =
                Mathf.Max(1, survivalWaveMinutes);
            difficultyIncreaseEveryWaves =
                Mathf.Max(1, difficultyIncreaseEveryWaves);
            wagonOfferEveryWaves =
                Mathf.Max(1, wagonOfferEveryWaves);
            survivalCopperGrowthPerWave =
                Mathf.Max(0, survivalCopperGrowthPerWave);
            survivalExperienceGrowthPerWave =
                Mathf.Max(0, survivalExperienceGrowthPerWave);
            lowHitPointPausePercent =
                Mathf.Clamp(lowHitPointPausePercent, 5, 90);
        }
    }
}
