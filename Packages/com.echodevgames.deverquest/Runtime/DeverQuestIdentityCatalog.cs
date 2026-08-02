using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewIdentityCatalog",
        menuName = "DeverQuest/Identity/Identity Catalog")]
    public sealed class DeverQuestIdentityCatalog :
        ScriptableObject
    {
        public string displayName = "Guild Identity Catalog";
        public List<DeverQuestAncestry> ancestries =
            new List<DeverQuestAncestry>();
        public List<DeverQuestClassDefinition> classes =
            new List<DeverQuestClassDefinition>();
        public List<DeverQuestDeity> faiths =
            new List<DeverQuestDeity>();
        public DeverQuestAncestry defaultAncestry;
        public DeverQuestClassDefinition defaultClass;
        public DeverQuestDeity defaultFaith;

        private void OnValidate()
        {
            ancestries = ancestries ??
                         new List<DeverQuestAncestry>();
            classes = classes ??
                      new List<DeverQuestClassDefinition>();
            faiths = faiths ?? new List<DeverQuestDeity>();
            ancestries.RemoveAll(value => value == null);
            classes.RemoveAll(value => value == null);
            faiths.RemoveAll(value => value == null);
        }
    }
}
