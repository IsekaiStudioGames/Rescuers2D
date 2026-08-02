using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.DeverQuest
{
    /// <summary>
    /// A hidden Editor-only AudioSource host. It uses Unity's normal
    /// AudioSource, AudioListener, and Editor player-loop APIs rather than
    /// Unity's shared Inspector preview transport.
    /// </summary>
    [InitializeOnLoad]
    internal static class DeverQuestSupportedAudioHost
    {
        private sealed class ChannelState
        {
            public AudioSource source;
            public AudioClip clip;
            public bool loop;
            public bool paused;
            public bool expectedPlaying;
            public float profileVolume = 1f;
            public int savedSample;
            public double startedAt;
            public double missingSince = -1d;
        }

        private const string HostName =
            "DeverQuest Supported Audio Host";
        private const double MissingPlaybackGraceSeconds = 0.75d;

        private static readonly ChannelState MusicState =
            new ChannelState();
        private static readonly ChannelState AmbienceState =
            new ChannelState();

        private static Scene previewScene;
        private static GameObject hostObject;
        private static AudioListener ownedListener;
        private static AudioSource cueSource;
        private static AudioClip cueClip;
        private static float cueProfileVolume = 1f;
        private static bool cueActive;
        private static double cueExpectedEnd;
        private static double lastListenerRefresh;
        private static bool initialized;
        private static bool operational;
        private static bool suspendedForFocus;
        private static bool suspendedForPlayMode;
        private static string lastError = string.Empty;

        public static event Action<string> HostFailed;

        static DeverQuestSupportedAudioHost()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            EditorApplication.focusChanged -= OnFocusChanged;
            EditorApplication.focusChanged += OnFocusChanged;

            EditorApplication.playModeStateChanged -=
                OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged +=
                OnPlayModeStateChanged;

            AssemblyReloadEvents.beforeAssemblyReload -= Shutdown;
            AssemblyReloadEvents.beforeAssemblyReload += Shutdown;

            EditorApplication.quitting -= Shutdown;
            EditorApplication.quitting += Shutdown;

            AudioSettings.OnAudioConfigurationChanged -=
                OnAudioConfigurationChanged;
            AudioSettings.OnAudioConfigurationChanged +=
                OnAudioConfigurationChanged;
        }

        public static bool IsOperational
        {
            get
            {
                EnsureInitialized();
                return operational;
            }
        }

        public static string LastError
        {
            get
            {
                EnsureInitialized();
                return lastError;
            }
        }

        public static bool Play(
            DeverQuestEditorAudioChannel channel,
            AudioClip clip,
            bool loop,
            float profileVolume)
        {
            if (clip == null || !EnsureOperational())
            {
                return false;
            }

            ChannelState state = GetState(channel);
            state.clip = clip;
            state.loop = loop;
            state.paused = false;
            state.expectedPlaying = true;
            state.profileVolume = Mathf.Clamp01(profileVolume);
            state.savedSample = 0;
            state.startedAt = EditorApplication.timeSinceStartup;
            state.missingSince = -1d;

            ConfigureSource(state);
            StartSource(state, 0);
            ApplyVolumes();
            EditorApplication.QueuePlayerLoopUpdate();
            return true;
        }

        public static bool PlayCue(
            AudioClip clip,
            float profileVolume)
        {
            if (clip == null || !EnsureOperational())
            {
                return false;
            }

            StopCue();
            cueClip = clip;
            cueProfileVolume = Mathf.Clamp01(profileVolume);
            cueActive = true;
            cueExpectedEnd =
                EditorApplication.timeSinceStartup +
                Math.Max(0.05d, clip.length) +
                0.1d;

            cueSource.clip = clip;
            cueSource.loop = false;
            cueSource.volume =
                DeverQuestAudioMixerSettings
                    .EffectiveCueVolume(cueProfileVolume);
            cueSource.timeSamples = 0;
            cueSource.Play();
            ApplyVolumes();
            EditorApplication.QueuePlayerLoopUpdate();
            return true;
        }

        public static void Pause(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || state.paused)
            {
                return;
            }

            CaptureSample(state);
            state.paused = true;
            state.expectedPlaying = false;
            state.source?.Pause();
        }

        public static void Resume(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || !state.paused ||
                !EnsureOperational())
            {
                return;
            }

            state.paused = false;
            state.expectedPlaying = true;
            StartSource(state, state.savedSample);
            ApplyVolumes();
        }

        public static bool Stop(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null)
            {
                return false;
            }

            state.source?.Stop();
            ClearState(state);
            return true;
        }

        public static bool IsPlaying(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || state.paused ||
                !state.expectedPlaying)
            {
                return false;
            }

            if (!state.loop && HasReachedLogicalEnd(state))
            {
                return false;
            }

            return state.source != null && state.source.isPlaying;
        }

        public static AudioClip GetClip(
            DeverQuestEditorAudioChannel channel)
        {
            return GetState(channel).clip;
        }

        public static void SetVolume(
            DeverQuestEditorAudioChannel channel,
            float profileVolume)
        {
            ChannelState state = GetState(channel);
            state.profileVolume = Mathf.Clamp01(profileVolume);
            ApplyVolumes();
        }

        public static void ApplyVolumes()
        {
            ApplyVolume(MusicState,
                DeverQuestEditorAudioChannel.Music);
            ApplyVolume(AmbienceState,
                DeverQuestEditorAudioChannel.Ambience);
            if (cueSource != null)
            {
                cueSource.volume =
                    DeverQuestAudioMixerSettings
                        .EffectiveCueVolume(cueProfileVolume);
            }
        }

        public static bool Recover()
        {
            if (!EnsureOperational())
            {
                return false;
            }

            RefreshListenerOwnership();
            RestartExpectedChannel(MusicState);
            RestartExpectedChannel(AmbienceState);
            if (cueActive && cueClip != null &&
                EditorApplication.timeSinceStartup < cueExpectedEnd)
            {
                AudioClip clip = cueClip;
                float profileVolume = cueProfileVolume;
                StopCue();
                PlayCue(clip, profileVolume);
            }
            ApplyVolumes();
            return true;
        }

        public static void ResetHost()
        {
            StopAll();
            DestroyHost();
            initialized = false;
            operational = false;
            lastError = string.Empty;
            EnsureInitialized();
        }

        public static void StopAll()
        {
            MusicState.source?.Stop();
            AmbienceState.source?.Stop();
            StopCue();
            ClearState(MusicState);
            ClearState(AmbienceState);
        }

        public static string Describe()
        {
            EnsureInitialized();
            if (operational)
            {
                return "Supported hidden AudioSource host is active. " +
                       "Music, Ambience, and cues use separate sources.";
            }

            return string.IsNullOrWhiteSpace(lastError)
                ? "Supported AudioSource host is unavailable."
                : lastError;
        }

        private static bool EnsureOperational()
        {
            EnsureInitialized();
            return operational;
        }

        private static void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            try
            {
                previewScene = EditorSceneManager.NewPreviewScene();
                hostObject = new GameObject(HostName)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                SceneManager.MoveGameObjectToScene(
                    hostObject,
                    previewScene);

                MusicState.source = AddSource("Music");
                AmbienceState.source = AddSource("Ambience");
                cueSource = AddSource("Cues");
                RefreshListenerOwnership();

                operational =
                    MusicState.source != null &&
                    AmbienceState.source != null &&
                    cueSource != null;
                lastError = operational
                    ? string.Empty
                    : "Could not create one or more AudioSource channels.";
            }
            catch (Exception exception)
            {
                operational = false;
                lastError =
                    "Supported AudioSource host could not initialize: " +
                    exception.GetBaseException().Message;
                DestroyHost();
            }
        }

        private static AudioSource AddSource(string label)
        {
            AudioSource source = hostObject.AddComponent<AudioSource>();
            source.name = "DeverQuest " + label;
            source.hideFlags = HideFlags.HideAndDontSave;
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.priority = label == "Cues" ? 16 : 64;
            source.bypassEffects = false;
            source.bypassListenerEffects = false;
            source.bypassReverbZones = true;
            source.ignoreListenerPause = true;
            return source;
        }

        private static void ConfigureSource(ChannelState state)
        {
            state.source.clip = state.clip;
            state.source.loop = state.loop;
            state.source.spatialBlend = 0f;
        }

        private static void StartSource(
            ChannelState state,
            int startSample)
        {
            if (state.source == null || state.clip == null)
            {
                return;
            }

            state.source.Stop();
            ConfigureSource(state);
            try
            {
                int maximum = Math.Max(0, state.clip.samples - 1);
                state.source.timeSamples = Mathf.Clamp(
                    startSample,
                    0,
                    maximum);
            }
            catch
            {
                state.source.time = 0f;
            }

            state.savedSample = Math.Max(0, startSample);
            state.startedAt = EditorApplication.timeSinceStartup;
            state.source.Play();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private static void RestartExpectedChannel(ChannelState state)
        {
            if (state.clip == null || state.paused ||
                !state.expectedPlaying)
            {
                return;
            }

            CaptureSample(state);
            StartSource(state, state.savedSample);
            state.missingSince = -1d;
        }

        private static void CaptureSample(ChannelState state)
        {
            if (state.source == null || state.clip == null)
            {
                return;
            }

            try
            {
                if (state.source.timeSamples >= 0)
                {
                    state.savedSample = state.source.timeSamples;
                }
                state.startedAt = EditorApplication.timeSinceStartup;
            }
            catch
            {
                float seconds = Mathf.Max(0f, state.source.time);
                state.savedSample = state.clip.frequency > 0
                    ? Mathf.FloorToInt(seconds * state.clip.frequency)
                    : 0;
            }
        }

        private static bool HasReachedLogicalEnd(ChannelState state)
        {
            if (state.clip == null || state.loop)
            {
                return false;
            }

            if (state.source != null && state.source.isPlaying)
            {
                return false;
            }

            double elapsed = Math.Max(
                0d,
                EditorApplication.timeSinceStartup - state.startedAt);
            double startSeconds = state.clip.frequency > 0
                ? (double)state.savedSample / state.clip.frequency
                : 0d;
            return startSeconds + elapsed >=
                   Math.Max(0.01d, state.clip.length - 0.05d);
        }

        private static void Update()
        {
            if (!initialized || !operational)
            {
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (now - lastListenerRefresh >= 2d)
            {
                RefreshListenerOwnership();
                lastListenerRefresh = now;
            }

            if (cueActive &&
                EditorApplication.timeSinceStartup >= cueExpectedEnd)
            {
                StopCue();
                ApplyVolumes();
            }

            MonitorChannel(MusicState, "Music");
            MonitorChannel(AmbienceState, "Ambience");

            if (cueActive ||
                MusicState.expectedPlaying ||
                AmbienceState.expectedPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        private static void MonitorChannel(
            ChannelState state,
            string label)
        {
            if (state.clip == null || state.paused ||
                !state.expectedPlaying || suspendedForFocus ||
                suspendedForPlayMode)
            {
                state.missingSince = -1d;
                return;
            }

            if (state.source != null && state.source.isPlaying)
            {
                CaptureSample(state);
                state.missingSince = -1d;
                return;
            }

            if (!state.loop && HasReachedLogicalEnd(state))
            {
                ClearState(state);
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (state.missingSince < 0d)
            {
                state.missingSince = now;
                return;
            }

            if (now - state.missingSince <
                MissingPlaybackGraceSeconds)
            {
                return;
            }

            RestartExpectedChannel(state);
            if (state.source == null || !state.source.isPlaying)
            {
                FailHost(
                    label +
                    " AudioSource could not sustain Edit Mode playback.");
            }
        }

        private static void OnFocusChanged(bool focused)
        {
            if (!initialized || !operational ||
                !DeverQuestAudioMixerSettings
                    .PauseWhenEditorUnfocused)
            {
                if (focused && initialized && operational)
                {
                    RecoverLostPlaybackAfterFocus();
                }
                return;
            }

            if (!focused)
            {
                SuspendForFocus();
                return;
            }

            ResumeFromFocus();
        }

        private static void SuspendForFocus()
        {
            suspendedForFocus = true;
            PauseForSuspension(MusicState);
            PauseForSuspension(AmbienceState);
            cueSource?.Pause();
        }

        private static void ResumeFromFocus()
        {
            if (!suspendedForFocus)
            {
                RecoverLostPlaybackAfterFocus();
                return;
            }

            suspendedForFocus = false;
            ResumeAfterSuspension(MusicState);
            ResumeAfterSuspension(AmbienceState);
            if (cueActive)
            {
                cueSource?.UnPause();
            }
            ApplyVolumes();
        }

        private static void RecoverLostPlaybackAfterFocus()
        {
            RestartIfUnexpectedlyStopped(MusicState);
            RestartIfUnexpectedlyStopped(AmbienceState);
        }

        private static void OnPlayModeStateChanged(
            PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    suspendedForPlayMode = true;
                    PauseForSuspension(MusicState);
                    PauseForSuspension(AmbienceState);
                    cueSource?.Pause();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    suspendedForPlayMode = false;
                    ResumeAfterSuspension(MusicState);
                    ResumeAfterSuspension(AmbienceState);
                    if (cueActive)
                    {
                        cueSource?.UnPause();
                    }
                    ApplyVolumes();
                    break;
            }
        }

        private static void PauseForSuspension(ChannelState state)
        {
            if (state.clip == null || state.paused ||
                !state.expectedPlaying)
            {
                return;
            }

            CaptureSample(state);
            state.source?.Pause();
        }

        private static void ResumeAfterSuspension(ChannelState state)
        {
            if (state.clip == null || state.paused ||
                !state.expectedPlaying)
            {
                return;
            }

            StartSource(state, state.savedSample);
        }

        private static void RestartIfUnexpectedlyStopped(
            ChannelState state)
        {
            if (state.clip == null || state.paused ||
                !state.expectedPlaying ||
                (state.source != null && state.source.isPlaying))
            {
                return;
            }

            RestartExpectedChannel(state);
        }

        private static void OnAudioConfigurationChanged(
            bool deviceWasChanged)
        {
            if (!initialized || !operational)
            {
                return;
            }

            EditorApplication.delayCall += () => Recover();
        }

        private static void RefreshListenerOwnership()
        {
            if (hostObject == null)
            {
                return;
            }

            bool externalListenerExists =
                Resources.FindObjectsOfTypeAll<AudioListener>()
                    .Any(listener =>
                        listener != null &&
                        listener != ownedListener &&
                        listener.enabled &&
                        listener.gameObject.activeInHierarchy &&
                        !EditorUtility.IsPersistent(listener));

            if (externalListenerExists)
            {
                if (ownedListener != null)
                {
                    ownedListener.enabled = false;
                }
                return;
            }

            if (ownedListener == null)
            {
                ownedListener =
                    hostObject.AddComponent<AudioListener>();
                ownedListener.hideFlags =
                    HideFlags.HideAndDontSave;
            }
            ownedListener.enabled = true;
        }

        private static void ApplyVolume(
            ChannelState state,
            DeverQuestEditorAudioChannel channel)
        {
            if (state.source == null)
            {
                return;
            }

            state.source.volume =
                DeverQuestAudioMixerSettings
                    .EffectiveLongFormVolume(
                        channel,
                        state.profileVolume,
                        cueActive);
        }

        private static ChannelState GetState(
            DeverQuestEditorAudioChannel channel)
        {
            return channel == DeverQuestEditorAudioChannel.Ambience
                ? AmbienceState
                : MusicState;
        }

        private static void StopCue()
        {
            cueSource?.Stop();
            cueClip = null;
            cueProfileVolume = 1f;
            cueActive = false;
            cueExpectedEnd = 0d;
        }

        private static void ClearState(ChannelState state)
        {
            state.clip = null;
            state.loop = false;
            state.paused = false;
            state.expectedPlaying = false;
            state.profileVolume = 1f;
            state.savedSample = 0;
            state.startedAt = 0d;
            state.missingSince = -1d;
        }

        private static void FailHost(string reason)
        {
            if (!operational)
            {
                return;
            }

            CaptureSample(MusicState);
            CaptureSample(AmbienceState);
            operational = false;
            lastError = reason;
            MusicState.source?.Stop();
            AmbienceState.source?.Stop();
            StopCue();
            HostFailed?.Invoke(reason);
        }

        private static void DestroyHost()
        {
            if (hostObject != null)
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
            }
            hostObject = null;
            ownedListener = null;
            cueSource = null;

            if (previewScene.IsValid())
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
            previewScene = default;
        }

        private static void Shutdown()
        {
            StopAll();
            DestroyHost();
            initialized = false;
            operational = false;
        }
    }
}
