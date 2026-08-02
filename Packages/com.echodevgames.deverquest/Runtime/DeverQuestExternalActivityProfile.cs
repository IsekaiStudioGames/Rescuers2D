using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    public sealed class DeverQuestExternalActivityProvider
    {
        public string displayName = "External Tool";
        [Tooltip("Executable name without .exe, such as aseprite.")]
        public string processName = string.Empty;
        [Tooltip("Optional case-insensitive text required in the window title.")]
        public string windowTitleContains = string.Empty;
        public bool enabled = true;
        [Min(1)]
        public int inputFreshnessSeconds = 30;
    }

    [CreateAssetMenu(
        fileName = "NewExternalActivityProfile",
        menuName = "DeverQuest/Activity/External Activity Profile")]
    public sealed class DeverQuestExternalActivityProfile :
        ScriptableObject
    {
        public string displayName = "External Creative Tools";
        [TextArea(2, 5)]
        public string description =
            "Foreground creative applications allowed to keep a Quest active " +
            "while recent keyboard or pointer input is detected.";
        public List<DeverQuestExternalActivityProvider> providers =
            new List<DeverQuestExternalActivityProvider>();

        public int ProviderCount =>
            providers?.Count ?? 0;

        private void OnValidate()
        {
            if (providers == null)
            {
                providers =
                    new List<DeverQuestExternalActivityProvider>();
            }

            foreach (DeverQuestExternalActivityProvider provider
                     in providers)
            {
                if (provider == null)
                {
                    continue;
                }

                provider.displayName =
                    provider.displayName?.Trim() ?? string.Empty;
                provider.processName =
                    provider.processName?.Trim() ?? string.Empty;
                provider.windowTitleContains =
                    provider.windowTitleContains?.Trim() ?? string.Empty;
                provider.inputFreshnessSeconds =
                    Mathf.Max(1, provider.inputFreshnessSeconds);
            }
        }
    }
}
