//----- DeverQuestPlaylistPlayer.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestPlaybackState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 2
    }

    [InitializeOnLoad]
    internal static class DeverQuestPlaylistPlayer
    {
        private const string PlaylistGuidKey =
            "EchoDevGames.DeverQuest.Playlist.Guid";

        private const string TrackIndexKey =
            "EchoDevGames.DeverQuest.Playlist.TrackIndex";

        private static readonly System.Random Random =
            new System.Random();

        private static readonly Stack<int> TrackHistory =
            new Stack<int>();

        private static readonly HashSet<int> ShuffleVisited =
            new HashSet<int>();

        private static DeverQuestPlaylist playlist;
        private static int trackIndex;
        private static double trackStartedEditorTime;
        private static bool pausedBySession;
        private static AudioClip playingClip;
        private static double notPlayingObservedSince = -1d;

        public static DeverQuestPlaybackState State { get; private set; }
        public static string LastError { get; private set; } = string.Empty;

        static DeverQuestPlaylistPlayer()
        {
            LoadSelection();

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            DeverQuestSessionStore.SessionStarted -= OnSessionStarted;
            DeverQuestSessionStore.SessionStarted += OnSessionStarted;
            DeverQuestSessionStore.SessionPaused -= OnSessionPaused;
            DeverQuestSessionStore.SessionPaused += OnSessionPaused;
            DeverQuestSessionStore.SessionResumed -= OnSessionResumed;
            DeverQuestSessionStore.SessionResumed += OnSessionResumed;
            DeverQuestSessionStore.SessionCompleted -= OnSessionEnded;
            DeverQuestSessionStore.SessionCompleted += OnSessionEnded;
            DeverQuestSessionStore.SessionDiscarded -= OnSessionEnded;
            DeverQuestSessionStore.SessionDiscarded += OnSessionEnded;
        }

        public static DeverQuestPlaylist Playlist => playlist;

        public static AudioClip CurrentTrack =>
            playlist?.GetTrack(trackIndex);

        public static int TrackIndex => trackIndex;

        public static void SelectTrack(int index)
        {
            if (playlist == null || playlist.TrackCount <= 0)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, playlist.TrackCount - 1);
            if (index == trackIndex)
            {
                return;
            }

            bool resumePlayback =
                State == DeverQuestPlaybackState.Playing;
            Stop();
            trackIndex = index;
            TrackHistory.Clear();
            ShuffleVisited.Clear();
            SaveSelection();

            if (resumePlayback)
            {
                PlayCurrent();
            }
        }

        public static void ClearPlaybackState()
        {
            playingClip = null;
            State = DeverQuestPlaybackState.Stopped;
            pausedBySession = false;
            notPlayingObservedSince = -1d;
            LastError = string.Empty;
        }

        public static void SetPlaylist(DeverQuestPlaylist value)
        {
            if (playlist == value)
            {
                return;
            }

            Stop();
            playlist = value;
            trackIndex = 0;
            TrackHistory.Clear();
            ShuffleVisited.Clear();
            SaveSelection();
        }

        public static void Play()
        {
            if (!ValidatePlaylist())
            {
                return;
            }

            if (State == DeverQuestPlaybackState.Paused)
            {
                Resume();
                return;
            }

            TrackHistory.Clear();
            ShuffleVisited.Clear();
            PlayCurrent();
        }

        public static void Pause()
        {
            if (State != DeverQuestPlaybackState.Playing)
            {
                return;
            }

            DeverQuestAudioTransport.Pause(
                DeverQuestEditorAudioChannel.Music);
            State = DeverQuestPlaybackState.Paused;
        }

        public static void Resume()
        {
            if (State != DeverQuestPlaybackState.Paused)
            {
                return;
            }

            DeverQuestAudioTransport.Resume(
                DeverQuestEditorAudioChannel.Music);
            State = DeverQuestPlaybackState.Playing;
            trackStartedEditorTime =
                EditorApplication.timeSinceStartup;
        }

        public static void Stop()
        {
            if (playingClip != null)
            {
                DeverQuestAudioTransport.Stop(
                    DeverQuestEditorAudioChannel.Music);
            }
            playingClip = null;
            State = DeverQuestPlaybackState.Stopped;
            pausedBySession = false;
            notPlayingObservedSince = -1d;
        }

        public static void Next()
        {
            if (!ValidatePlaylist())
            {
                return;
            }

            int nextIndex = GetNextIndex();

            if (nextIndex < 0)
            {
                Stop();
                return;
            }

            TrackHistory.Push(trackIndex);
            trackIndex = nextIndex;
            SaveSelection();
            PlayCurrent();
        }

        public static void Previous()
        {
            if (!ValidatePlaylist())
            {
                return;
            }

            if (TrackHistory.Count > 0)
            {
                trackIndex = TrackHistory.Pop();
                SaveSelection();
                PlayCurrent();
                return;
            }

            trackIndex--;

            if (trackIndex < 0)
            {
                trackIndex = playlist.RepeatMode ==
                             DeverQuestRepeatMode.All
                    ? playlist.TrackCount - 1
                    : 0;
            }

            SaveSelection();
            PlayCurrent();
        }

        public static void ApplyVolume()
        {
            if (playlist != null && CurrentTrack != null)
            {
                DeverQuestAudioTransport.SetVolume(
                    DeverQuestEditorAudioChannel.Music,
                    playlist.Volume);
            }
        }

        private static void PlayCurrent()
        {
            AudioClip clip = FindPlayableTrack();

            if (clip == null)
            {
                LastError = "The selected playlist contains no AudioClips.";
                Stop();
                return;
            }

            bool loop =
                playlist.RepeatMode == DeverQuestRepeatMode.One;

            if (!DeverQuestAudioTransport.Play(
                    DeverQuestEditorAudioChannel.Music,
                    clip,
                    loop,
                    playlist.Volume))
            {
                LastError =
                    "This Unity editor did not expose preview-audio playback.";
                State = DeverQuestPlaybackState.Stopped;
                return;
            }

            LastError = string.Empty;
            playingClip = clip;
            State = DeverQuestPlaybackState.Playing;
            pausedBySession = false;
            trackStartedEditorTime =
                EditorApplication.timeSinceStartup;
            notPlayingObservedSince = -1d;
            SaveSelection();
        }

        private static AudioClip FindPlayableTrack()
        {
            if (playlist == null || playlist.TrackCount == 0)
            {
                return null;
            }

            for (int attempts = 0;
                 attempts < playlist.TrackCount;
                 attempts++)
            {
                AudioClip clip = playlist.GetTrack(trackIndex);

                if (clip != null)
                {
                    return clip;
                }

                trackIndex =
                    (trackIndex + 1) % playlist.TrackCount;
            }

            return null;
        }

        private static int GetNextIndex()
        {
            if (playlist.TrackCount <= 1)
            {
                return playlist.RepeatMode == DeverQuestRepeatMode.Off
                    ? -1
                    : 0;
            }

            if (playlist.Shuffle)
            {
                ShuffleVisited.Add(trackIndex);

                List<int> candidates =
                    Enumerable.Range(0, playlist.TrackCount)
                        .Where(index => !ShuffleVisited.Contains(index))
                        .ToList();

                if (candidates.Count == 0)
                {
                    if (playlist.RepeatMode !=
                        DeverQuestRepeatMode.All)
                    {
                        return -1;
                    }

                    ShuffleVisited.Clear();
                    ShuffleVisited.Add(trackIndex);

                    candidates =
                        Enumerable.Range(0, playlist.TrackCount)
                            .Where(index => index != trackIndex)
                            .ToList();
                }

                int totalWeight = candidates.Sum(
                    index => playlist.GetTrackWeight(index));
                int choice = Random.Next(
                    Math.Max(1, totalWeight));
                foreach (int candidate in candidates)
                {
                    choice -= playlist.GetTrackWeight(candidate);
                    if (choice < 0)
                    {
                        return candidate;
                    }
                }
                return candidates[candidates.Count - 1];
            }

            int sequentialIndex = trackIndex + 1;

            if (sequentialIndex < playlist.TrackCount)
            {
                return sequentialIndex;
            }

            return playlist.RepeatMode == DeverQuestRepeatMode.All
                ? 0
                : -1;
        }

        private static bool ValidatePlaylist()
        {
            if (playlist == null)
            {
                LastError = "Select or create a DeverQuest playlist.";
                return false;
            }

            if (playlist.TrackCount <= 0)
            {
                LastError = "Add at least one AudioClip to the playlist.";
                return false;
            }

            if (!DeverQuestAudioTransport.IsAvailable)
            {
                LastError =
                    "No DeverQuest Editor audio transport is available.";
                return false;
            }

            return true;
        }

        private static void Update()
        {
            if (State != DeverQuestPlaybackState.Playing ||
                !DeverQuestAudioTransport.PlaybackStatusSupported ||
                CurrentTrack == null ||
                !InternalEditorUtility.isApplicationActive ||
                EditorApplication.timeSinceStartup -
                trackStartedEditorTime < 0.5d)
            {
                notPlayingObservedSince = -1d;
                return;
            }

            if (DeverQuestAudioTransport.IsPlaying(
                    DeverQuestEditorAudioChannel.Music))
            {
                notPlayingObservedSince = -1d;
                return;
            }

            double now = EditorApplication.timeSinceStartup;

            if (notPlayingObservedSince < 0d)
            {
                notPlayingObservedSince = now;
                return;
            }

            if (now - notPlayingObservedSince >= 1d)
            {
                notPlayingObservedSince = -1d;
                Next();
            }
        }

        private static void OnSessionStarted()
        {
            if (DeverQuestSettingsStore.Profile
                .autoPlayMusicOnSessionStart)
            {
                Play();
            }
        }

        private static void OnSessionPaused()
        {
            if (DeverQuestSettingsStore.Profile
                    .pauseMusicWithSession &&
                State == DeverQuestPlaybackState.Playing)
            {
                pausedBySession = true;
                Pause();
            }
        }

        private static void OnSessionResumed()
        {
            if (DeverQuestSettingsStore.Profile
                    .resumeMusicWithSession &&
                pausedBySession &&
                State == DeverQuestPlaybackState.Paused)
            {
                Resume();
                pausedBySession = false;
            }
        }

        private static void OnSessionEnded()
        {
            if (DeverQuestSettingsStore.Profile
                .stopMusicOnSessionEnd)
            {
                Stop();
            }
        }

        private static void SaveSelection()
        {
            string assetPath =
                playlist == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(playlist);

            string guid =
                string.IsNullOrWhiteSpace(assetPath)
                    ? string.Empty
                    : AssetDatabase.AssetPathToGUID(assetPath);

            EditorPrefs.SetString(PlaylistGuidKey, guid);
            EditorPrefs.SetInt(TrackIndexKey, trackIndex);
        }

        private static void LoadSelection()
        {
            string guid =
                EditorPrefs.GetString(PlaylistGuidKey, string.Empty);

            string path =
                string.IsNullOrWhiteSpace(guid)
                    ? string.Empty
                    : AssetDatabase.GUIDToAssetPath(guid);

            playlist =
                string.IsNullOrWhiteSpace(path)
                    ? null
                    : AssetDatabase.LoadAssetAtPath<DeverQuestPlaylist>(
                        path);

            trackIndex = Math.Max(
                0,
                EditorPrefs.GetInt(TrackIndexKey, 0));

            if (playlist != null &&
                trackIndex >= playlist.TrackCount)
            {
                trackIndex = 0;
            }

            State = DeverQuestPlaybackState.Stopped;
        }
    }
}

//----- DeverQuestPlaylistPlayer.cs END -----
