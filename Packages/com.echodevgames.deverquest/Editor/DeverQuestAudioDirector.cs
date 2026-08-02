using System;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestAudioCue
    {
        IdleWarning,
        IdlePaused,
        FocusCheckIn,
        Hydration,
        MovementBreak,
        MealReminder,
        StageComplete,
        EncounterAttack,
        EncounterDanger,
        EncounterVictory,
        EncounterDefeat,
        QuestComplete,
        LevelUp,
        Purchase,
        Error
    }

    [InitializeOnLoad]
    internal static class DeverQuestAudioDirector
    {
        private const string WarningGuidKey =
            "EchoDevGames.DeverQuest.Audio.WarningProfile";
        private const string AmbienceGuidKey =
            "EchoDevGames.DeverQuest.Audio.AmbienceProfile";
        private const string AmbienceIndexKey =
            "EchoDevGames.DeverQuest.Audio.AmbienceIndex";

        private static DeverQuestWarningAudioProfile warningProfile;
        private static DeverQuestAmbienceProfile ambienceProfile;
        private static int ambienceIndex;
        private static AudioClip ambienceClip;

        static DeverQuestAudioDirector()
        {
            warningProfile =
                LoadAsset<DeverQuestWarningAudioProfile>(
                    WarningGuidKey);
            ambienceProfile =
                LoadAsset<DeverQuestAmbienceProfile>(
                    AmbienceGuidKey);
            ambienceIndex = Math.Max(
                0,
                EditorPrefs.GetInt(AmbienceIndexKey, 0));
            DeverQuestSessionStore.SessionStarted -=
                OnSessionStarted;
            DeverQuestSessionStore.SessionStarted +=
                OnSessionStarted;
            DeverQuestSessionStore.SessionCompleted -=
                OnSessionEnded;
            DeverQuestSessionStore.SessionCompleted +=
                OnSessionEnded;
            DeverQuestSessionStore.SessionDiscarded -=
                OnSessionEnded;
            DeverQuestSessionStore.SessionDiscarded +=
                OnSessionEnded;
        }

        public static DeverQuestWarningAudioProfile WarningProfile =>
            warningProfile;
        public static DeverQuestAmbienceProfile AmbienceProfile =>
            ambienceProfile;
        public static AudioClip CurrentAmbience => ambienceClip;
        public static int AmbienceIndex => ambienceIndex;
        public static bool AmbiencePlaying =>
            ambienceClip != null &&
            DeverQuestAudioTransport.IsPlaying(
                DeverQuestEditorAudioChannel.Ambience);

        public static void SetWarningProfile(
            DeverQuestWarningAudioProfile profile)
        {
            warningProfile = profile;
            SaveAsset(WarningGuidKey, profile);
        }

        public static void SetAmbienceProfile(
            DeverQuestAmbienceProfile profile)
        {
            StopAmbience();
            ambienceProfile = profile;
            ambienceIndex = 0;
            SaveAsset(AmbienceGuidKey, profile);
            EditorPrefs.SetInt(AmbienceIndexKey, ambienceIndex);
        }

        public static bool PlayCue(DeverQuestAudioCue cue)
        {
            AudioClip clip = ClipFor(cue);
            return clip != null &&
                   DeverQuestAudioTransport.PlayCue(
                       clip,
                       warningProfile.volume);
        }

        public static void SelectAmbience(int index)
        {
            if (ambienceProfile == null ||
                ambienceProfile.ambienceClips == null ||
                ambienceProfile.ambienceClips.Count == 0)
            {
                return;
            }

            index = Math.Max(
                0,
                Math.Min(
                    ambienceProfile.ambienceClips.Count - 1,
                    index));
            if (index == ambienceIndex)
            {
                return;
            }

            bool resumePlayback = AmbiencePlaying;
            StopAmbience();
            ambienceIndex = index;
            EditorPrefs.SetInt(
                AmbienceIndexKey, ambienceIndex);

            if (resumePlayback)
            {
                PlayAmbience();
            }
        }

        public static bool RecoverAudioTransport()
        {
            return DeverQuestAudioTransport
                .Recover();
        }

        public static void ResetAllAudio()
        {
            DeverQuestAudioTransport.ResetAll();
            ambienceClip = null;
            DeverQuestPlaylistPlayer.ClearPlaybackState();
        }

        internal static void ClearAmbiencePlaybackState()
        {
            ambienceClip = null;
        }

        public static void PlayAmbience()
        {
            if (ambienceProfile == null ||
                ambienceProfile.ambienceClips == null ||
                ambienceProfile.ambienceClips.Count == 0)
            {
                return;
            }
            ambienceIndex = Math.Max(
                0,
                Math.Min(
                    ambienceProfile.ambienceClips.Count - 1,
                    ambienceIndex));
            ambienceClip = null;
            for (int attempt = 0;
                 attempt < ambienceProfile.ambienceClips.Count;
                 attempt++)
            {
                int candidate =
                    (ambienceIndex + attempt) %
                    ambienceProfile.ambienceClips.Count;
                AudioClip clip =
                    ambienceProfile.ambienceClips[candidate];
                if (clip == null)
                {
                    continue;
                }
                ambienceIndex = candidate;
                ambienceClip = clip;
                break;
            }
            if (ambienceClip == null)
            {
                return;
            }
            bool started =
                DeverQuestAudioTransport.Play(
                    DeverQuestEditorAudioChannel.Ambience,
                    ambienceClip,
                    true,
                    ambienceProfile.volume);
            if (!started)
            {
                ambienceClip = null;
            }
            EditorPrefs.SetInt(
                AmbienceIndexKey, ambienceIndex);
        }

        public static void StopAmbience()
        {
            if (ambienceClip != null)
            {
                DeverQuestAudioTransport.Stop(
                    DeverQuestEditorAudioChannel.Ambience);
            }
            ambienceClip = null;
        }

        public static void NextAmbience()
        {
            if (ambienceProfile == null ||
                ambienceProfile.ambienceClips == null ||
                ambienceProfile.ambienceClips.Count == 0)
            {
                return;
            }
            StopAmbience();
            if (ambienceProfile.shuffle &&
                ambienceProfile.ambienceClips.Count > 1)
            {
                int next = ambienceIndex;
                while (next == ambienceIndex)
                {
                    next = UnityEngine.Random.Range(
                        0,
                        ambienceProfile.ambienceClips.Count);
                }
                ambienceIndex = next;
            }
            else
            {
                ambienceIndex =
                    (ambienceIndex + 1) %
                    ambienceProfile.ambienceClips.Count;
            }
            PlayAmbience();
        }

        public static void ApplyVolumes()
        {
            if (ambienceClip != null &&
                ambienceProfile != null)
            {
                DeverQuestAudioTransport.SetVolume(
                    DeverQuestEditorAudioChannel.Ambience,
                    ambienceProfile.volume);
            }
            DeverQuestPlaylistPlayer.ApplyVolume();
        }

        private static void OnSessionStarted()
        {
            if (ambienceProfile != null &&
                ambienceProfile.playDuringActiveQuest &&
                !AmbiencePlaying)
            {
                PlayAmbience();
            }
        }

        private static void OnSessionEnded()
        {
            if (ambienceProfile != null &&
                ambienceProfile.playDuringActiveQuest)
            {
                StopAmbience();
            }
        }

        private static AudioClip ClipFor(
            DeverQuestAudioCue cue)
        {
            if (warningProfile == null)
            {
                return null;
            }
            switch (cue)
            {
                case DeverQuestAudioCue.IdleWarning:
                    return warningProfile.idleWarning;
                case DeverQuestAudioCue.IdlePaused:
                    return warningProfile.idlePaused;
                case DeverQuestAudioCue.FocusCheckIn:
                    return warningProfile.focusCheckIn;
                case DeverQuestAudioCue.Hydration:
                    return warningProfile.hydration;
                case DeverQuestAudioCue.MovementBreak:
                    return warningProfile.movementBreak;
                case DeverQuestAudioCue.MealReminder:
                    return warningProfile.mealReminder;
                case DeverQuestAudioCue.StageComplete:
                    return warningProfile.stageComplete;
                case DeverQuestAudioCue.EncounterAttack:
                    return warningProfile.encounterAttack;
                case DeverQuestAudioCue.EncounterDanger:
                    return warningProfile.encounterDanger;
                case DeverQuestAudioCue.EncounterVictory:
                    return warningProfile.encounterVictory;
                case DeverQuestAudioCue.EncounterDefeat:
                    return warningProfile.encounterDefeat;
                case DeverQuestAudioCue.QuestComplete:
                    return warningProfile.questComplete;
                case DeverQuestAudioCue.LevelUp:
                    return warningProfile.levelUp;
                case DeverQuestAudioCue.Purchase:
                    return warningProfile.purchase;
                default:
                    return warningProfile.error;
            }
        }

        private static T LoadAsset<T>(string key)
            where T : UnityEngine.Object
        {
            string guid = EditorPrefs.GetString(key, string.Empty);
            return string.IsNullOrWhiteSpace(guid)
                ? null
                : AssetDatabase.LoadAssetAtPath<T>(
                    AssetDatabase.GUIDToAssetPath(guid));
        }

        private static void SaveAsset(
            string key,
            UnityEngine.Object asset)
        {
            string path =
                asset == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(asset);
            EditorPrefs.SetString(
                key,
                string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : AssetDatabase.AssetPathToGUID(path));
        }
    }
}
