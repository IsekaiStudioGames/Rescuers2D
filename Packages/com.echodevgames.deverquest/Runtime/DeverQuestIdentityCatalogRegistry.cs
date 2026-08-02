using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "GuildIdentityRegistry",
        menuName = "DeverQuest/Identity/Guild Identity Registry")]
    public sealed class DeverQuestIdentityCatalogRegistry :
        ScriptableObject
    {
        public DeverQuestIdentityCatalog activeCatalog;
        [TextArea(2, 5)]
        public string guildNotes = string.Empty;
    }
}
