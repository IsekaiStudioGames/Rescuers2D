//----- DeverQuestPlaylist.cs START -----

using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestRepeatMode
    {
        Off = 0,
        All = 1,
        One = 2
    }

    [CreateAssetMenu(
        fileName = "DeverQuestPlaylist",
        menuName = "DeverQuest/Playlist")]
    public sealed class DeverQuestPlaylist : ScriptableObject
    {
        [SerializeField]
        private List<AudioClip> tracks = new List<AudioClip>();
        [SerializeField]
        private List<int> trackWeights = new List<int>();

        [SerializeField]
        private bool shuffle;

        [SerializeField]
        private DeverQuestRepeatMode repeatMode =
            DeverQuestRepeatMode.All;

        [SerializeField, Range(0f, 1f)]
        private float volume = 0.75f;

        public IReadOnlyList<AudioClip> Tracks => tracks;

        public bool Shuffle
        {
            get => shuffle;
            set => shuffle = value;
        }

        public DeverQuestRepeatMode RepeatMode
        {
            get => repeatMode;
            set => repeatMode = value;
        }

        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp01(value);
        }

        public int TrackCount => tracks?.Count ?? 0;

        public AudioClip GetTrack(int index)
        {
            if (tracks == null ||
                index < 0 ||
                index >= tracks.Count)
            {
                return null;
            }

            return tracks[index];
        }

        public int GetTrackWeight(int index)
        {
            return trackWeights != null &&
                   index >= 0 &&
                   index < trackWeights.Count
                ? Mathf.Max(1, trackWeights[index])
                : 1;
        }

        private void OnValidate()
        {
            volume = Mathf.Clamp01(volume);

            if (tracks == null)
            {
                tracks = new List<AudioClip>();
            }
            trackWeights = trackWeights ?? new List<int>();
            while (trackWeights.Count < tracks.Count)
            {
                trackWeights.Add(1);
            }
            if (trackWeights.Count > tracks.Count)
            {
                trackWeights.RemoveRange(
                    tracks.Count,
                    trackWeights.Count - tracks.Count);
            }
            for (int index = 0;
                 index < trackWeights.Count;
                 index++)
            {
                trackWeights[index] =
                    Mathf.Max(1, trackWeights[index]);
            }
        }
    }
}

//----- DeverQuestPlaylist.cs END -----
