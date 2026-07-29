//----- MusicLibraryData.cs START -----

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MusicLibrary_",
    menuName = "Rescuers2D/Audio/Music Library")]
public sealed class MusicLibraryData : ScriptableObject
{
    [SerializeField]
    private MusicTrackData[] tracks =
        new MusicTrackData[0];

    public IReadOnlyList<MusicTrackData> Tracks =>
        tracks;

    public int Count =>
        tracks != null
            ? tracks.Length
            : 0;

    public bool TryGetTrack(
        string trackId,
        out MusicTrackData track)
    {
        track = null;

        if (tracks == null ||
            string.IsNullOrWhiteSpace(trackId))
        {
            return false;
        }

        string normalizedId =
            trackId.Trim();

        for (int i = 0; i < tracks.Length; i++)
        {
            MusicTrackData candidate =
                tracks[i];

            if (candidate == null)
                continue;

            if (string.Equals(
                    candidate.TrackId,
                    normalizedId,
                    System.StringComparison.OrdinalIgnoreCase))
            {
                track = candidate;
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        if (tracks == null)
        {
            tracks =
                new MusicTrackData[0];

            return;
        }

        HashSet<string> ids =
            new HashSet<string>(
                System.StringComparer.OrdinalIgnoreCase);

        HashSet<MusicTrackData> assets =
            new HashSet<MusicTrackData>();

        for (int i = 0; i < tracks.Length; i++)
        {
            MusicTrackData track =
                tracks[i];

            if (track == null)
            {
                Debug.LogWarning(
                    $"[MUSIC LIBRARY] {name} has a null track " +
                    $"at index {i}.",
                    this);

                continue;
            }

            if (!assets.Add(track))
            {
                Debug.LogWarning(
                    $"[MUSIC LIBRARY] {name} contains the asset " +
                    $"{track.name} more than once.",
                    this);
            }

            if (string.IsNullOrWhiteSpace(track.TrackId))
            {
                Debug.LogWarning(
                    $"[MUSIC LIBRARY] Track {track.name} has no ID.",
                    track);

                continue;
            }

            if (!ids.Add(track.TrackId.Trim()))
            {
                Debug.LogError(
                    $"[MUSIC LIBRARY] {name} contains duplicate " +
                    $"Track ID '{track.TrackId}'.",
                    this);
            }

            if (!track.IsPlayable)
            {
                Debug.LogWarning(
                    $"[MUSIC LIBRARY] Track {track.name} is not " +
                    "currently playable.",
                    track);
            }
        }
    }
}

//----- MusicLibraryData.cs END -----