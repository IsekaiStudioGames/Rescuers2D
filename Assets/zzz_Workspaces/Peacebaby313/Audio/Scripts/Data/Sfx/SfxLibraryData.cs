//----- SfxLibraryData.cs START -----

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SfxLibrary_",
    menuName = "Rescuers2D/Audio/SFX Library")]
public sealed class SfxLibraryData : ScriptableObject
{
    [SerializeField]
    private SfxCueData[] cues =
        new SfxCueData[0];

    public IReadOnlyList<SfxCueData> Cues =>
        cues;

    public int Count =>
        cues != null
            ? cues.Length
            : 0;

    public bool TryGetCue(
        string cueId,
        out SfxCueData cue)
    {
        cue = null;

        if (cues == null ||
            string.IsNullOrWhiteSpace(cueId))
        {
            return false;
        }

        string normalizedId =
            cueId.Trim();

        for (int i = 0; i < cues.Length; i++)
        {
            SfxCueData candidate =
                cues[i];

            if (candidate == null)
                continue;

            if (string.Equals(
                    candidate.CueId,
                    normalizedId,
                    System.StringComparison.OrdinalIgnoreCase))
            {
                cue = candidate;
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        if (cues == null)
        {
            cues =
                new SfxCueData[0];

            return;
        }

        HashSet<string> ids =
            new HashSet<string>(
                System.StringComparer.OrdinalIgnoreCase);

        HashSet<SfxCueData> assets =
            new HashSet<SfxCueData>();

        for (int i = 0; i < cues.Length; i++)
        {
            SfxCueData cue =
                cues[i];

            if (cue == null)
            {
                Debug.LogWarning(
                    $"[SFX LIBRARY] {name} has a null cue " +
                    $"at index {i}.",
                    this);

                continue;
            }

            if (!assets.Add(cue))
            {
                Debug.LogWarning(
                    $"[SFX LIBRARY] {name} contains the asset " +
                    $"{cue.name} more than once.",
                    this);
            }

            if (string.IsNullOrWhiteSpace(cue.CueId))
            {
                Debug.LogWarning(
                    $"[SFX LIBRARY] Cue {cue.name} has no ID.",
                    cue);

                continue;
            }

            if (!ids.Add(cue.CueId.Trim()))
            {
                Debug.LogError(
                    $"[SFX LIBRARY] {name} contains duplicate " +
                    $"Cue ID '{cue.CueId}'.",
                    this);
            }

            if (!cue.IsPlayable)
            {
                Debug.LogWarning(
                    $"[SFX LIBRARY] Cue {cue.name} is not " +
                    "currently playable.",
                    cue);
            }
        }
    }
}

//----- SfxLibraryData.cs END -----