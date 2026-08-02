using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestEncounterMode
    {
        Fixed = 0,
        Survival = 1
    }

    [Serializable]
    public sealed class DeverQuestDropEntry
    {
        public string displayName = "Coin Cache";
        [Range(0, 100)]
        public int dropChancePercent = 25;
        public int copper;
        public int experience;
        public DeverQuestEquipment equipment;
        public DeverQuestSpell spell;
        public DeverQuestShopItem shopItem;
    }

    [Serializable]
    public sealed class DeverQuestEncounterWave
    {
        public string waveTitle = "Encounter";
        public DeverQuestMonsterProfile monster;
        public int count = 1;
        public bool bossWave;
    }

    }
