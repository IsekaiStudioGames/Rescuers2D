using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewClassDefinition",
        menuName = "DeverQuest/Identity/Class Definition")]
    public sealed class DeverQuestClassDefinition :
        DeverQuestIdentityAsset
    {
        public string department = "Programming";
        public DeverQuestAbility primaryAbility =
            DeverQuestAbility.Strength;
        public int hitDie = 8;
        public bool usesMana;
        public bool supportsCompanion;
        public string companionTradition = string.Empty;
        public DeverQuestCompanionProfile starterCompanion;
        public DeverQuestAbilityProfile abilityProfile;
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int charisma = 10;
        public int luck = 10;
        public List<string> proficientSaves = new List<string>();
        public List<string> classFeatures = new List<string>();

        protected override void OnValidate()
        {
            base.OnValidate();
            department = department?.Trim() ?? string.Empty;
            hitDie = Mathf.Max(4, hitDie);
            strength = Mathf.Clamp(strength, 1, 30);
            dexterity = Mathf.Clamp(dexterity, 1, 30);
            constitution = Mathf.Clamp(constitution, 1, 30);
            intelligence = Mathf.Clamp(intelligence, 1, 30);
            wisdom = Mathf.Clamp(wisdom, 1, 30);
            charisma = Mathf.Clamp(charisma, 1, 30);
            luck = Mathf.Clamp(luck, 1, 30);
            proficientSaves = proficientSaves ??
                              new List<string>();
            classFeatures = classFeatures ?? new List<string>();
        }
    }
}
