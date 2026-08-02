//----- DestructibleWorldAudio.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(WorldAudioEmitter))]
public sealed class DestructibleWorldAudio : MonoBehaviour
{
    [Header("Runtime Emitter")]
    [SerializeField]
    private WorldAudioEmitter worldAudio;

    [Header("Material Cues")]
    [SerializeField]
    private SfxCueData impactCue;

    [SerializeField]
    private SfxCueData destructionCue;

    private bool hasPlayedDestruction;

    private void Reset()
    {
        worldAudio =
            GetComponent<WorldAudioEmitter>();
    }

    private void Awake()
    {
        ResolveEmitter();
    }

    private void OnEnable()
    {
        hasPlayedDestruction =
            false;
    }

    public void PlayImpactAtOrigin()
    {
        if (!ResolveEmitter())
        {
            return;
        }

        worldAudio.PlayAtOrigin(
            impactCue);
    }

    public void PlayImpactAt(
        Vector3 hitPosition)
    {
        if (!ResolveEmitter())
        {
            return;
        }

        worldAudio.PlayAtPosition(
            impactCue,
            hitPosition);
    }

    public void PlayDestruction()
    {
        if (hasPlayedDestruction)
        {
            return;
        }

        hasPlayedDestruction =
            true;

        if (!ResolveEmitter())
        {
            return;
        }

        worldAudio.PlayAtOrigin(
            destructionCue);
    }

    public void ResetAudioState()
    {
        hasPlayedDestruction =
            false;
    }

    private bool ResolveEmitter()
    {
        if (worldAudio == null)
        {
            worldAudio =
                GetComponent<WorldAudioEmitter>();
        }

        if (worldAudio != null)
        {
            return true;
        }

        Debug.LogError(
            $"[DESTRUCTIBLE AUDIO] '{name}' requires a " +
            "WorldAudioEmitter.",
            this);

        return false;
    }
}

//----- DestructibleWorldAudio.cs END -----