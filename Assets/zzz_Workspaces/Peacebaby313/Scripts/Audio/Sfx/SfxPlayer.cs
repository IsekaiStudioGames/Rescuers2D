//----- SfxPlayer.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public sealed class SfxPlayer : MonoBehaviour
{
    private const float MinimumPitch = 0.01f;
    private const float MaximumPitch = 3f;

    private enum PlaybackSpace
    {
        TwoDimensional,
        WorldPosition,
        Attached
    }

    private sealed class CueSelectionState
    {
        public int LastVariationIndex = -1;
        public int NextSequentialIndex;

        public readonly List<int> ShuffleBag =
            new List<int>();
    }

    [Header("Pool")]
    [Min(1)]
    [SerializeField]
    private int initialPoolSize = 16;

    [Min(1)]
    [SerializeField]
    private int maximumPoolSize = 32;

    [SerializeField]
    private bool allowPoolExpansion = true;

    [Header("Routing")]
    [Tooltip(
        "Used when a cue does not assign its own mixer group.")]
    [SerializeField]
    private AudioMixerGroup fallbackMixerGroup;

    [Header("Diagnostics")]
    [SerializeField]
    private bool logRejectedRequests;

    [SerializeField]
    private bool logVoiceStealing;

    private readonly List<SfxVoice> voices =
        new List<SfxVoice>();

    private readonly Dictionary<SfxCueData, float>
        lastPlaybackTimes =
            new Dictionary<SfxCueData, float>();

    private readonly Dictionary<SfxCueData, CueSelectionState>
        selectionStates =
            new Dictionary<SfxCueData, CueSelectionState>();

    private long playSequence;

    public event Action<SfxCueData, SfxPlaybackHandle>
        OnVoiceStarted;

    public event Action<SfxCueData, SfxPlaybackHandle>
        OnVoiceStopped;

    public int PoolSize =>
        voices.Count;

    public int ActiveVoiceCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < voices.Count; i++)
            {
                if (voices[i].IsActive)
                    count++;
            }

            return count;
        }
    }

    public int AvailableVoiceCount =>
        PoolSize - ActiveVoiceCount;

    private void Awake()
    {
        BuildInitialPool();
    }

    private void Update()
    {
        for (int i = 0; i < voices.Count; i++)
        {
            SfxVoice voice = voices[i];

            if (voice.IsActive &&
                voice.ShouldRelease())
            {
                ReleaseVoice(voice);
            }
        }
    }

    private void OnDisable()
    {
        StopAll();
    }

    public SfxPlaybackHandle Play(
        SfxCueData cue,
        float volumeMultiplier = 1f,
        float pitchMultiplier = 1f)
    {
        return TryPlay(
            cue,
            transform.position,
            null,
            PlaybackSpace.TwoDimensional,
            volumeMultiplier,
            pitchMultiplier);
    }

    public SfxPlaybackHandle PlayAtPosition(
        SfxCueData cue,
        Vector3 position,
        float volumeMultiplier = 1f,
        float pitchMultiplier = 1f)
    {
        return TryPlay(
            cue,
            position,
            null,
            PlaybackSpace.WorldPosition,
            volumeMultiplier,
            pitchMultiplier);
    }

    public SfxPlaybackHandle PlayAttached(
        SfxCueData cue,
        Transform target,
        float volumeMultiplier = 1f,
        float pitchMultiplier = 1f)
    {
        return TryPlay(
            cue,
            target != null
                ? target.position
                : transform.position,
            target,
            PlaybackSpace.Attached,
            volumeMultiplier,
            pitchMultiplier);
    }

    public bool Stop(
        SfxPlaybackHandle handle)
    {
        if (!handle.IsValid ||
            handle.VoiceId < 0 ||
            handle.VoiceId >= voices.Count)
        {
            return false;
        }

        SfxVoice voice =
            voices[handle.VoiceId];

        if (!voice.Matches(handle))
            return false;

        ReleaseVoice(voice);

        return true;
    }

    public int Stop(
        SfxCueData cue)
    {
        if (cue == null)
            return 0;

        int stoppedCount = 0;

        for (int i = 0; i < voices.Count; i++)
        {
            SfxVoice voice = voices[i];

            if (!voice.IsActive ||
                voice.Cue != cue)
            {
                continue;
            }

            ReleaseVoice(voice);
            stoppedCount++;
        }

        return stoppedCount;
    }

    public int StopAll()
    {
        int stoppedCount = 0;

        for (int i = 0; i < voices.Count; i++)
        {
            SfxVoice voice = voices[i];

            if (!voice.IsActive)
                continue;

            ReleaseVoice(voice);
            stoppedCount++;
        }

        return stoppedCount;
    }

    public bool IsPlaying(
        SfxPlaybackHandle handle)
    {
        if (!handle.IsValid ||
            handle.VoiceId < 0 ||
            handle.VoiceId >= voices.Count)
        {
            return false;
        }

        return voices[handle.VoiceId]
            .Matches(handle);
    }

    private SfxPlaybackHandle TryPlay(
        SfxCueData cue,
        Vector3 position,
        Transform target,
        PlaybackSpace playbackSpace,
        float volumeMultiplier,
        float pitchMultiplier)
    {
        if (!isActiveAndEnabled)
            return SfxPlaybackHandle.Invalid;

        if (cue == null)
        {
            Reject(
                "Cannot play a null cue.");

            return SfxPlaybackHandle.Invalid;
        }

        if (!cue.IsPlayable)
        {
            Reject(
                $"Cue '{cue.name}' is not playable.");

            return SfxPlaybackHandle.Invalid;
        }

        if (playbackSpace == PlaybackSpace.Attached &&
            target == null)
        {
            Reject(
                $"Cue '{cue.DisplayName}' cannot attach " +
                "to a null target.");

            return SfxPlaybackHandle.Invalid;
        }

        if (!HasSelectableVariation(cue))
        {
            Reject(
                $"Cue '{cue.DisplayName}' has no variation " +
                $"available for {cue.SelectionMode} selection.");

            return SfxPlaybackHandle.Invalid;
        }

        float currentTime =
            Time.unscaledTime;

        if (IsCoolingDown(
                cue,
                currentTime))
        {
            Reject(
                $"Cue '{cue.DisplayName}' is cooling down.");

            return SfxPlaybackHandle.Invalid;
        }

        int voiceLimit =
            cue.MaximumSimultaneousVoices;

        if (voiceLimit > 0 &&
            CountVoicesForCue(cue) >= voiceLimit)
        {
            Reject(
                $"Cue '{cue.DisplayName}' reached its " +
                $"voice limit of {voiceLimit}.");

            return SfxPlaybackHandle.Invalid;
        }

        SfxVoice voice =
            AcquireVoice(cue);

        if (voice == null)
            return SfxPlaybackHandle.Invalid;

        if (!TrySelectVariation(
                cue,
                out SfxVariation variation))
        {
            Reject(
                $"Cue '{cue.DisplayName}' could not " +
                "select a variation.");

            return SfxPlaybackHandle.Invalid;
        }

        float randomizedVolume =
            UnityEngine.Random.Range(
                cue.RandomVolumeRange.x,
                cue.RandomVolumeRange.y);

        float finalVolume =
            cue.CueVolume *
            variation.VolumeMultiplier *
            randomizedVolume *
            Mathf.Max(0f, volumeMultiplier);

        float randomizedPitch =
            UnityEngine.Random.Range(
                cue.RandomPitchRange.x,
                cue.RandomPitchRange.y);

        float finalPitch =
            Mathf.Clamp(
                (randomizedPitch +
                 variation.PitchOffset) *
                Mathf.Abs(pitchMultiplier),
                MinimumPitch,
                MaximumPitch);

        AudioMixerGroup mixerGroup =
            cue.MixerGroup != null
                ? cue.MixerGroup
                : fallbackMixerGroup;

        playSequence++;

        SfxPlaybackHandle handle =
            voice.Play(
                cue,
                variation.Clip,
                mixerGroup,
                position,
                playbackSpace ==
                    PlaybackSpace.Attached
                        ? target
                        : null,
                playbackSpace ==
                    PlaybackSpace.TwoDimensional,
                finalVolume,
                finalPitch,
                playSequence);

        lastPlaybackTimes[cue] =
            currentTime;

        OnVoiceStarted?.Invoke(
            cue,
            handle);

        return handle;
    }

    private void BuildInitialPool()
    {
        if (voices.Count > 0)
            return;

        for (int i = 0;
             i < initialPoolSize;
             i++)
        {
            CreateVoice();
        }
    }

    private SfxVoice CreateVoice()
    {
        SfxVoice voice =
            new SfxVoice(
                voices.Count,
                transform);

        voices.Add(voice);

        return voice;
    }

    private SfxVoice AcquireVoice(
        SfxCueData incomingCue)
    {
        for (int i = 0; i < voices.Count; i++)
        {
            if (!voices[i].IsActive)
                return voices[i];
        }

        if (allowPoolExpansion &&
            voices.Count < maximumPoolSize)
        {
            return CreateVoice();
        }

        SfxVoice victim =
            FindStealableVoice(
                incomingCue.Priority);

        if (victim == null)
        {
            Reject(
                $"Pool exhausted. Cue " +
                $"'{incomingCue.DisplayName}' was not " +
                "important enough to replace an active voice.");

            return null;
        }

        if (logVoiceStealing)
        {
            Debug.Log(
                $"[SFX PLAYER] '{incomingCue.DisplayName}' " +
                $"replaced '{victim.Cue.DisplayName}' on " +
                $"voice {victim.VoiceId}.",
                this);
        }

        ReleaseVoice(victim);

        return victim;
    }

    private SfxVoice FindStealableVoice(
        int incomingPriority)
    {
        SfxVoice bestCandidate = null;

        for (int i = 0; i < voices.Count; i++)
        {
            SfxVoice candidate = voices[i];

            if (!candidate.IsActive)
                continue;

            if (candidate.Priority <
                incomingPriority)
            {
                continue;
            }

            if (bestCandidate == null)
            {
                bestCandidate = candidate;
                continue;
            }

            bool isLessImportant =
                candidate.Priority >
                bestCandidate.Priority;

            bool isEquallyImportantAndOlder =
                candidate.Priority ==
                bestCandidate.Priority &&
                candidate.StartOrder <
                bestCandidate.StartOrder;

            if (isLessImportant ||
                isEquallyImportantAndOlder)
            {
                bestCandidate = candidate;
            }
        }

        return bestCandidate;
    }

    private void ReleaseVoice(
        SfxVoice voice)
    {
        if (voice == null ||
            !voice.IsActive)
        {
            return;
        }

        SfxCueData stoppedCue =
            voice.Cue;

        SfxPlaybackHandle stoppedHandle =
            voice.CurrentHandle;

        voice.Release();

        OnVoiceStopped?.Invoke(
            stoppedCue,
            stoppedHandle);
    }

    private bool IsCoolingDown(
        SfxCueData cue,
        float currentTime)
    {
        if (cue.Cooldown <= 0f)
            return false;

        if (!lastPlaybackTimes.TryGetValue(
                cue,
                out float lastPlaybackTime))
        {
            return false;
        }

        return currentTime -
               lastPlaybackTime <
               cue.Cooldown;
    }

    private int CountVoicesForCue(
        SfxCueData cue)
    {
        int count = 0;

        for (int i = 0; i < voices.Count; i++)
        {
            if (voices[i].IsActive &&
                voices[i].Cue == cue)
            {
                count++;
            }
        }

        return count;
    }

    private bool HasSelectableVariation(
        SfxCueData cue)
    {
        bool requiresPositiveWeight =
            cue.SelectionMode !=
            SfxSelectionMode.Sequential;

        for (int i = 0;
             i < cue.VariationCount;
             i++)
        {
            if (!cue.TryGetVariation(
                    i,
                    out SfxVariation variation))
            {
                continue;
            }

            if (!requiresPositiveWeight ||
                variation.SelectionWeight > 0f)
            {
                return true;
            }
        }

        return false;
    }

    private bool TrySelectVariation(
        SfxCueData cue,
        out SfxVariation variation)
    {
        variation = null;

        CueSelectionState state =
            GetSelectionState(cue);

        int selectedIndex;

        switch (cue.SelectionMode)
        {
            case SfxSelectionMode.Sequential:
                if (!TrySelectSequentialIndex(
                        cue,
                        state,
                        out selectedIndex))
                {
                    return false;
                }

                break;

            case SfxSelectionMode.ShuffleBag:
                if (!TrySelectShuffleBagIndex(
                        cue,
                        state,
                        out selectedIndex))
                {
                    return false;
                }

                break;

            case SfxSelectionMode
                .RandomWithoutImmediateRepeat:

                if (!TrySelectWeightedIndex(
                        cue,
                        null,
                        state.LastVariationIndex,
                        out selectedIndex))
                {
                    return false;
                }

                break;

            default:
                if (!TrySelectWeightedIndex(
                        cue,
                        null,
                        -1,
                        out selectedIndex))
                {
                    return false;
                }

                break;
        }

        if (!cue.TryGetVariation(
                selectedIndex,
                out variation))
        {
            return false;
        }

        state.LastVariationIndex =
            selectedIndex;

        return true;
    }

    private CueSelectionState GetSelectionState(
        SfxCueData cue)
    {
        if (selectionStates.TryGetValue(
                cue,
                out CueSelectionState state))
        {
            return state;
        }

        state = new CueSelectionState();

        selectionStates.Add(
            cue,
            state);

        return state;
    }

    private bool TrySelectSequentialIndex(
        SfxCueData cue,
        CueSelectionState state,
        out int selectedIndex)
    {
        selectedIndex = -1;

        int variationCount =
            cue.VariationCount;

        for (int offset = 0;
             offset < variationCount;
             offset++)
        {
            int index =
                (state.NextSequentialIndex +
                 offset) %
                variationCount;

            if (!cue.TryGetVariation(
                    index,
                    out _))
            {
                continue;
            }

            selectedIndex = index;

            state.NextSequentialIndex =
                (index + 1) %
                variationCount;

            return true;
        }

        return false;
    }

    private bool TrySelectShuffleBagIndex(
        SfxCueData cue,
        CueSelectionState state,
        out int selectedIndex)
    {
        if (state.ShuffleBag.Count == 0)
        {
            RefillShuffleBag(
                cue,
                state.ShuffleBag);
        }

        int excludedIndex =
            state.ShuffleBag.Count > 1
                ? state.LastVariationIndex
                : -1;

        if (!TrySelectWeightedIndex(
                cue,
                state.ShuffleBag,
                excludedIndex,
                out selectedIndex))
        {
            return false;
        }

        state.ShuffleBag.Remove(
            selectedIndex);

        return true;
    }

    private static void RefillShuffleBag(
        SfxCueData cue,
        List<int> shuffleBag)
    {
        shuffleBag.Clear();

        for (int i = 0;
             i < cue.VariationCount;
             i++)
        {
            if (!cue.TryGetVariation(
                    i,
                    out SfxVariation variation))
            {
                continue;
            }

            if (variation.SelectionWeight > 0f)
            {
                shuffleBag.Add(i);
            }
        }
    }

    private static bool TrySelectWeightedIndex(
        SfxCueData cue,
        List<int> allowedIndices,
        int excludedIndex,
        out int selectedIndex)
    {
        if (TrySelectWeightedIndexInternal(
                cue,
                allowedIndices,
                excludedIndex,
                out selectedIndex))
        {
            return true;
        }

        if (excludedIndex >= 0)
        {
            return TrySelectWeightedIndexInternal(
                cue,
                allowedIndices,
                -1,
                out selectedIndex);
        }

        return false;
    }

    private static bool TrySelectWeightedIndexInternal(
        SfxCueData cue,
        List<int> allowedIndices,
        int excludedIndex,
        out int selectedIndex)
    {
        selectedIndex = -1;
        float totalWeight = 0f;

        int candidateCount =
            allowedIndices != null
                ? allowedIndices.Count
                : cue.VariationCount;

        for (int i = 0;
             i < candidateCount;
             i++)
        {
            int index =
                allowedIndices != null
                    ? allowedIndices[i]
                    : i;

            if (index == excludedIndex)
                continue;

            if (!cue.TryGetVariation(
                    index,
                    out SfxVariation variation))
            {
                continue;
            }

            totalWeight +=
                Mathf.Max(
                    0f,
                    variation.SelectionWeight);
        }

        if (totalWeight <= 0f)
            return false;

        float roll =
            UnityEngine.Random.value *
            totalWeight;

        float accumulatedWeight = 0f;
        int lastValidIndex = -1;

        for (int i = 0;
             i < candidateCount;
             i++)
        {
            int index =
                allowedIndices != null
                    ? allowedIndices[i]
                    : i;

            if (index == excludedIndex)
                continue;

            if (!cue.TryGetVariation(
                    index,
                    out SfxVariation variation))
            {
                continue;
            }

            float weight =
                Mathf.Max(
                    0f,
                    variation.SelectionWeight);

            if (weight <= 0f)
                continue;

            lastValidIndex = index;
            accumulatedWeight += weight;

            if (roll <= accumulatedWeight)
            {
                selectedIndex = index;
                return true;
            }
        }

        selectedIndex = lastValidIndex;

        return selectedIndex >= 0;
    }

    private void Reject(
        string reason)
    {
        if (!logRejectedRequests)
            return;

        Debug.LogWarning(
            $"[SFX PLAYER] {reason}",
            this);
    }

    private void OnValidate()
    {
        initialPoolSize =
            Mathf.Max(
                1,
                initialPoolSize);

        maximumPoolSize =
            Mathf.Max(
                initialPoolSize,
                maximumPoolSize);
    }
}

//----- SfxPlayer.cs END -----