using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewFaith",
        menuName = "DeverQuest/Identity/Faith")]
    public sealed class DeverQuestDeity :
        DeverQuestIdentityAsset
    {
        public bool allowsAgnosticFollowers = true;
        public DeverQuestAlignment alignment =
            DeverQuestAlignment.TrueNeutral;
        public List<DeverQuestAlignment> allowedAlignments =
            new List<DeverQuestAlignment>();
        public List<string> domains = new List<string>();
        public List<string> favoredClassIds = new List<string>();
        public List<string> restrictedAncestryIds =
            new List<string>();
        public string grantedTrait = string.Empty;

        protected override void OnValidate()
        {
            base.OnValidate();
            allowedAlignments = allowedAlignments ??
                                new List<DeverQuestAlignment>();
            domains = domains ?? new List<string>();
            favoredClassIds = favoredClassIds ?? new List<string>();
            restrictedAncestryIds = restrictedAncestryIds ??
                                    new List<string>();
        }
    }
}
