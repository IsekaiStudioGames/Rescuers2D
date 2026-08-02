using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewAncestry",
        menuName = "DeverQuest/Identity/Ancestry")]
    public sealed class DeverQuestAncestry :
        DeverQuestIdentityAsset
    {
        public bool playable = true;
        public bool sapient = true;
        public string size = "Medium";
        public int movementSpeed = 30;
        public int naturalArmorBonus;
        public int hitPointBonus;
        public int manaBonus;
        public List<DeverQuestAbilityAdjustment> abilityAdjustments =
            new List<DeverQuestAbilityAdjustment>();
        public List<string> languages = new List<string>();
        public List<string> innateTraits = new List<string>();
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();
        public List<string> eligibleClassIds = new List<string>();
        public List<string> restrictedClassIds = new List<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            SanitizeAncestry();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SanitizeAncestry();
        }

        private void SanitizeAncestry()
        {
            movementSpeed = Mathf.Max(1, movementSpeed);
            abilityAdjustments = abilityAdjustments ??
                                 new List<DeverQuestAbilityAdjustment>();
            languages = languages ?? new List<string>();
            innateTraits = innateTraits ?? new List<string>();
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
            eligibleClassIds = eligibleClassIds ?? new List<string>();
            restrictedClassIds = restrictedClassIds ?? new List<string>();
        }
    }
}
