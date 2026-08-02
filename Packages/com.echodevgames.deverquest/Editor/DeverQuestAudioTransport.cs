using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestAudioTransportMode
    {
        SupportedAudioSourceHost = 0,
        LegacyPreviewFallback = 1,
        Unavailable = 2
    }

    /// <summary>
    /// Routes DeverQuest audio through the supported hidden AudioSource host
    /// when available, with the previous preview bridge retained as a
    /// compatibility fallback.
    /// </summary>
    internal static class DeverQuestAudioTransport
    {
        private static string fallbackReason = string.Empty;

        static DeverQuestAudioTransport()
        {
            DeverQuestSupportedAudioHost.HostFailed -= OnHostFailed;
            DeverQuestSupportedAudioHost.HostFailed += OnHostFailed;
        }

        public static DeverQuestAudioTransportMode Mode
        {
            get
            {
                if (DeverQuestAudioMixerSettings.PreferSupportedHost &&
                    DeverQuestSupportedAudioHost.IsOperational)
                {
                    return DeverQuestAudioTransportMode
                        .SupportedAudioSourceHost;
                }

                return DeverQuestEditorAudioBridge.IsAvailable
                    ? DeverQuestAudioTransportMode
                        .LegacyPreviewFallback
                    : DeverQuestAudioTransportMode.Unavailable;
            }
        }

        public static bool IsAvailable =>
            Mode != DeverQuestAudioTransportMode.Unavailable;

        public static bool UsingSupportedHost =>
            Mode == DeverQuestAudioTransportMode
                .SupportedAudioSourceHost;

        public static bool VolumeSupported =>
            UsingSupportedHost ||
            DeverQuestEditorAudioBridge.VolumeSupported;

        public static bool IndependentVolumeSupported =>
            UsingSupportedHost ||
            DeverQuestEditorAudioBridge.IndependentVolumeSupported;

        public static bool PlaybackStatusSupported =>
            UsingSupportedHost ||
            DeverQuestEditorAudioBridge.PlaybackStatusSupported;

        public static bool InspectorPreviewIsolated =>
            UsingSupportedHost;

        public static string DisplayName
        {
            get
            {
                switch (Mode)
                {
                    case DeverQuestAudioTransportMode
                        .SupportedAudioSourceHost:
                        return "Supported AudioSource Host";
                    case DeverQuestAudioTransportMode
                        .LegacyPreviewFallback:
                        return "Legacy Preview Fallback";
                    default:
                        return "Unavailable";
                }
            }
        }

        public static string StatusMessage
        {
            get
            {
                if (UsingSupportedHost)
                {
                    return DeverQuestSupportedAudioHost.Describe();
                }

                if (Mode == DeverQuestAudioTransportMode
                    .LegacyPreviewFallback)
                {
                    if (!DeverQuestAudioMixerSettings.PreferSupportedHost)
                    {
                        return "The supported AudioSource host is disabled " +
                               "by this Editor profile. The legacy preview " +
                               "fallback is active.";
                    }

                    string reason = string.IsNullOrWhiteSpace(
                        fallbackReason)
                        ? DeverQuestSupportedAudioHost.LastError
                        : fallbackReason;
                    return string.IsNullOrWhiteSpace(reason)
                        ? "Using Unity's shared preview transport for " +
                          "compatibility. Inspector previews may interrupt it."
                        : "Using the legacy preview fallback: " + reason;
                }

                return "No Editor audio transport is available.";
            }
        }

        public static bool Play(
            DeverQuestEditorAudioChannel channel,
            AudioClip clip,
            bool loop,
            float profileVolume)
        {
            if (clip == null)
            {
                return false;
            }

            if (UsingSupportedHost)
            {
                if (DeverQuestSupportedAudioHost.Play(
                        channel,
                        clip,
                        loop,
                        profileVolume))
                {
                    return true;
                }

                fallbackReason =
                    DeverQuestSupportedAudioHost.LastError;
            }

            return DeverQuestEditorAudioBridge.Play(
                channel,
                clip,
                loop,
                EffectiveFallbackVolume(channel, profileVolume));
        }

        public static bool PlayCue(
            AudioClip clip,
            float profileVolume)
        {
            if (clip == null)
            {
                return false;
            }

            if (UsingSupportedHost &&
                DeverQuestSupportedAudioHost.PlayCue(
                    clip,
                    profileVolume))
            {
                return true;
            }

            return DeverQuestEditorAudioBridge.PlayCue(
                clip,
                DeverQuestAudioMixerSettings
                    .EffectiveCueVolume(profileVolume));
        }

        public static void Pause(
            DeverQuestEditorAudioChannel channel)
        {
            if (UsingSupportedHost)
            {
                DeverQuestSupportedAudioHost.Pause(channel);
                return;
            }

            DeverQuestEditorAudioBridge.Pause(channel);
        }

        public static void Resume(
            DeverQuestEditorAudioChannel channel)
        {
            if (UsingSupportedHost)
            {
                DeverQuestSupportedAudioHost.Resume(channel);
                return;
            }

            DeverQuestEditorAudioBridge.Resume(channel);
        }

        public static bool Stop(
            DeverQuestEditorAudioChannel channel)
        {
            if (UsingSupportedHost)
            {
                return DeverQuestSupportedAudioHost.Stop(channel);
            }

            return DeverQuestEditorAudioBridge.Stop(channel);
        }

        public static bool IsPlaying(
            DeverQuestEditorAudioChannel channel)
        {
            if (UsingSupportedHost)
            {
                return DeverQuestSupportedAudioHost.IsPlaying(channel);
            }

            return DeverQuestEditorAudioBridge.IsPlaying(channel);
        }

        public static AudioClip GetClip(
            DeverQuestEditorAudioChannel channel)
        {
            if (UsingSupportedHost)
            {
                return DeverQuestSupportedAudioHost.GetClip(channel);
            }

            return DeverQuestEditorAudioBridge.GetClip(channel);
        }

        public static void SetVolume(
            DeverQuestEditorAudioChannel channel,
            float profileVolume)
        {
            if (UsingSupportedHost)
            {
                DeverQuestSupportedAudioHost.SetVolume(
                    channel,
                    profileVolume);
                return;
            }

            DeverQuestEditorAudioBridge.SetVolume(
                channel,
                EffectiveFallbackVolume(channel, profileVolume));
        }

        public static void ApplyMixerSettings()
        {
            if (UsingSupportedHost)
            {
                DeverQuestSupportedAudioHost.ApplyVolumes();
                return;
            }

            DeverQuestEditorAudioBridge.SetGlobalVolume(
                DeverQuestAudioMixerSettings.MasterMute
                    ? 0f
                    : DeverQuestAudioMixerSettings.MasterVolume);
        }

        public static bool Recover()
        {
            if (UsingSupportedHost)
            {
                return DeverQuestSupportedAudioHost.Recover();
            }

            return DeverQuestEditorAudioBridge
                .RecoverNativeTransport();
        }

        public static void ReinitializeSupportedHost()
        {
            ResetAll();
            fallbackReason = string.Empty;
            DeverQuestSupportedAudioHost.ResetHost();
        }

        public static void ResetAll()
        {
            DeverQuestSupportedAudioHost.StopAll();
            DeverQuestEditorAudioBridge.ResetTransport();
        }

        public static void SetPreferSupportedHost(bool value)
        {
            if (DeverQuestAudioMixerSettings.PreferSupportedHost == value)
            {
                return;
            }

            ResetAll();
            DeverQuestAudioMixerSettings.PreferSupportedHost = value;
            fallbackReason = string.Empty;
            if (value)
            {
                DeverQuestSupportedAudioHost.ResetHost();
            }
        }

        private static float EffectiveFallbackVolume(
            DeverQuestEditorAudioChannel channel,
            float profileVolume)
        {
            return DeverQuestAudioMixerSettings
                .EffectiveLongFormVolume(
                    channel,
                    profileVolume,
                    false);
        }

        private static void OnHostFailed(string reason)
        {
            fallbackReason = reason;
            DeverQuestPlaylistPlayer.ClearPlaybackState();
            DeverQuestAudioDirector.ClearAmbiencePlaybackState();
            Debug.LogWarning(
                "[DeverQuest] Supported audio host failed. " +
                "Playback state was cleared and the legacy preview transport " +
                "will be used until the host is reinitialized. " + reason);
        }
    }
}
