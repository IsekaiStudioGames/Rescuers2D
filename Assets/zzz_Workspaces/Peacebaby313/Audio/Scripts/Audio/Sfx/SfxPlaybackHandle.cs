//----- SfxPlaybackHandle.cs START -----

using System;

[Serializable]
public struct SfxPlaybackHandle
    : IEquatable<SfxPlaybackHandle>
{
    private readonly int voiceId;
    private readonly uint generation;

    public static SfxPlaybackHandle Invalid =>
        new SfxPlaybackHandle(-1, 0);

    public int VoiceId =>
        voiceId;

    public bool IsValid =>
        voiceId >= 0 &&
        generation > 0;

    internal uint Generation =>
        generation;

    internal SfxPlaybackHandle(
        int voiceId,
        uint generation)
    {
        this.voiceId = voiceId;
        this.generation = generation;
    }

    public bool Equals(
        SfxPlaybackHandle other)
    {
        return voiceId == other.voiceId &&
               generation == other.generation;
    }

    public override bool Equals(
        object obj)
    {
        return obj is SfxPlaybackHandle other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (voiceId * 397) ^
                   (int)generation;
        }
    }

    public override string ToString()
    {
        return IsValid
            ? $"Voice {voiceId}, Generation {generation}"
            : "Invalid SFX Handle";
    }

    public static bool operator ==(
        SfxPlaybackHandle left,
        SfxPlaybackHandle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        SfxPlaybackHandle left,
        SfxPlaybackHandle right)
    {
        return !left.Equals(right);
    }
}

//----- SfxPlaybackHandle.cs END -----