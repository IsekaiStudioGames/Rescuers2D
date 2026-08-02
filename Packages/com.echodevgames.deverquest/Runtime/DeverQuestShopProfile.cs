using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewGuildShop",
        menuName = "DeverQuest/Guild Shop/Shop Profile")]
    public sealed class DeverQuestShopProfile : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Guild Quartermaster";
        [TextArea(2, 6)]
        public string welcomeMessage =
            "Spend earned coin on provisions, training, and gear.";
        [TextArea(2, 5)]
        public string closedMessage =
            "The Quartermaster is currently away from the counter.";

        [Header("Merchant Availability")]
        public bool shopOpen = true;
        public bool availableToMembers = true;
        public bool allowPurchases = true;
        public bool buyItemsFromMembers = true;
        [Min(0)]
        public int leadershipApprovalThresholdCopper;

        [Header("Stock")]
        public List<DeverQuestShopItem> items =
            new List<DeverQuestShopItem>();

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
            displayName = displayName?.Trim() ?? string.Empty;
            welcomeMessage = welcomeMessage?.Trim() ?? string.Empty;
            closedMessage = closedMessage?.Trim() ?? string.Empty;
            leadershipApprovalThresholdCopper = Mathf.Max(
                0, leadershipApprovalThresholdCopper);
            items = items ?? new List<DeverQuestShopItem>();
        }
    }
}
