using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    /// <summary>
    /// Local Editor-only mixer preferences. These values do not enter Guild
    /// records, Quest assets, or shared Chronicle data.
    /// </summary>
    internal static class DeverQuestAudioMixerSettings
    {
        private const string Prefix =
            "EchoDevGames.DeverQuest.AudioMixer.";

        private const string PreferHostKey =
            Prefix + "PreferSupportedHost";
        private const string MasterVolumeKey =
            Prefix + "MasterVolume";
        private const string MusicVolumeKey =
            Prefix + "MusicVolume";
        private const string AmbienceVolumeKey =
            Prefix + "AmbienceVolume";
        private const string CueVolumeKey =
            Prefix + "CueVolume";
        private const string MasterMuteKey =
            Prefix + "MasterMute";
        private const string MusicMuteKey =
            Prefix + "MusicMute";
        private const string AmbienceMuteKey =
            Prefix + "AmbienceMute";
        private const string CueMuteKey =
            Prefix + "CueMute";
        private const string DuckEnabledKey =
            Prefix + "DuckEnabled";
        private const string DuckVolumeKey =
            Prefix + "DuckVolume";
        private const string PauseWhenUnfocusedKey =
            Prefix + "PauseWhenUnfocused";

        public static bool PreferSupportedHost
        {
            get => EditorPrefs.GetBool(PreferHostKey, true);
            set => EditorPrefs.SetBool(PreferHostKey, value);
        }

        public static float MasterVolume
        {
            get => ReadVolume(MasterVolumeKey, 1f);
            set => WriteVolume(MasterVolumeKey, value);
        }

        public static float MusicVolume
        {
            get => ReadVolume(MusicVolumeKey, 1f);
            set => WriteVolume(MusicVolumeKey, value);
        }

        public static float AmbienceVolume
        {
            get => ReadVolume(AmbienceVolumeKey, 1f);
            set => WriteVolume(AmbienceVolumeKey, value);
        }

        public static float CueVolume
        {
            get => ReadVolume(CueVolumeKey, 1f);
            set => WriteVolume(CueVolumeKey, value);
        }

        public static bool MasterMute
        {
            get => EditorPrefs.GetBool(MasterMuteKey, false);
            set => EditorPrefs.SetBool(MasterMuteKey, value);
        }

        public static bool MusicMute
        {
            get => EditorPrefs.GetBool(MusicMuteKey, false);
            set => EditorPrefs.SetBool(MusicMuteKey, value);
        }

        public static bool AmbienceMute
        {
            get => EditorPrefs.GetBool(AmbienceMuteKey, false);
            set => EditorPrefs.SetBool(AmbienceMuteKey, value);
        }

        public static bool CueMute
        {
            get => EditorPrefs.GetBool(CueMuteKey, false);
            set => EditorPrefs.SetBool(CueMuteKey, value);
        }

        public static bool DuckLongFormDuringCues
        {
            get => EditorPrefs.GetBool(DuckEnabledKey, true);
            set => EditorPrefs.SetBool(DuckEnabledKey, value);
        }

        public static float DuckVolume
        {
            get => ReadVolume(DuckVolumeKey, 0.45f);
            set => WriteVolume(DuckVolumeKey, value);
        }

        public static bool PauseWhenEditorUnfocused
        {
            get => EditorPrefs.GetBool(
                PauseWhenUnfocusedKey,
                false);
            set => EditorPrefs.SetBool(
                PauseWhenUnfocusedKey,
                value);
        }

        public static float EffectiveLongFormVolume(
            DeverQuestEditorAudioChannel channel,
            float profileVolume,
            bool cueActive)
        {
            if (MasterMute || IsChannelMuted(channel))
            {
                return 0f;
            }

            float channelVolume =
                channel == DeverQuestEditorAudioChannel.Ambience
                    ? AmbienceVolume
                    : MusicVolume;
            float duck = cueActive && DuckLongFormDuringCues
                ? DuckVolume
                : 1f;

            return Mathf.Clamp01(
                profileVolume *
                MasterVolume *
                channelVolume *
                duck);
        }

        public static float EffectiveCueVolume(float profileVolume)
        {
            if (MasterMute || CueMute)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                profileVolume * MasterVolume * CueVolume);
        }

        public static void ResetDefaults()
        {
            PreferSupportedHost = true;
            MasterVolume = 1f;
            MusicVolume = 1f;
            AmbienceVolume = 1f;
            CueVolume = 1f;
            MasterMute = false;
            MusicMute = false;
            AmbienceMute = false;
            CueMute = false;
            DuckLongFormDuringCues = true;
            DuckVolume = 0.45f;
            PauseWhenEditorUnfocused = false;
        }

        private static bool IsChannelMuted(
            DeverQuestEditorAudioChannel channel)
        {
            return channel == DeverQuestEditorAudioChannel.Ambience
                ? AmbienceMute
                : MusicMute;
        }

        private static float ReadVolume(
            string key,
            float fallback)
        {
            return Mathf.Clamp01(
                EditorPrefs.GetFloat(key, fallback));
        }

        private static void WriteVolume(
            string key,
            float value)
        {
            EditorPrefs.SetFloat(key, Mathf.Clamp01(value));
        }
    }
}
