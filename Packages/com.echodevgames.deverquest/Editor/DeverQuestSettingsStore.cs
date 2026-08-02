//----- DeverQuestSettingsStore.cs START -----

using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestSettingsStore
    {
        private const string EditorPrefsKey =
            "EchoDevGames.DeverQuest.Profile.v1";

        private static DeverQuestProfile profile;

        static DeverQuestSettingsStore()
        {
            Load();
        }

        public static DeverQuestProfile Profile
        {
            get
            {
                if (profile == null)
                {
                    Load();
                }

                return profile;
            }
        }

        public static void Save()
        {
            Profile.Sanitize();

            string json = JsonUtility.ToJson(Profile);
            EditorPrefs.SetString(EditorPrefsKey, json);
        }

        public static void ResetProfile()
        {
            profile = new DeverQuestProfile();
            EditorPrefs.DeleteKey(EditorPrefsKey);
        }

        private static void Load()
        {
            string json = EditorPrefs.GetString(EditorPrefsKey, string.Empty);

            if (string.IsNullOrWhiteSpace(json))
            {
                profile = new DeverQuestProfile();
                return;
            }

            try
            {
                profile = JsonUtility.FromJson<DeverQuestProfile>(json);

                if (profile == null)
                {
                    profile = new DeverQuestProfile();
                }

                profile.Sanitize();
            }
            catch
            {
                Debug.LogWarning(
                    "[DeverQuest] Saved profile could not be read. " +
                    "A fresh profile has been loaded.");

                profile = new DeverQuestProfile();
            }
        }
    }
}

//----- DeverQuestSettingsStore.cs END -----
