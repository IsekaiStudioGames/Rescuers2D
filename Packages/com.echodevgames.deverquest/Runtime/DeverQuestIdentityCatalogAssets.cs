using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestAlignment
    {
        LawfulGood = 0,
        NeutralGood = 1,
        ChaoticGood = 2,
        LawfulNeutral = 3,
        TrueNeutral = 4,
        ChaoticNeutral = 5,
        LawfulEvil = 6,
        NeutralEvil = 7,
        ChaoticEvil = 8
    }

    [Serializable]
    public sealed class DeverQuestAbilityAdjustment
    {
        public DeverQuestAbility ability;
        [Range(-5, 5)]
        public int amount;
    }

    public abstract class DeverQuestIdentityAsset : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string identityId = string.Empty;
        public string displayName = string.Empty;
        [TextArea(3, 8)]
        public string lore = string.Empty;

        public string IdentityId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(identityId))
                {
                    identityId = Guid.NewGuid().ToString("N");
                }
                return identityId;
            }
        }

        protected virtual void OnEnable()
        {
            SanitizeIdentity();
        }

        protected virtual void OnValidate()
        {
            SanitizeIdentity();
        }

        protected void SanitizeIdentity()
        {
            _ = IdentityId;
            displayName = displayName?.Trim() ?? string.Empty;
            lore = lore?.Trim() ?? string.Empty;
        }
    }

    }
