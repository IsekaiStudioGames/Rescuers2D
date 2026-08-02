//----- BootstrapMusic.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class BootstrapMusic : MonoBehaviour
{
    [Header("Music")]
    [SerializeField]
    private AudioClip musicClip;

    [SerializeField, Range(0f, 1f)]
    private float volume = 0.5f;

    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();

        musicSource.clip = musicClip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        if (musicClip == null)
        {
            Debug.LogWarning(
                "[MUSIC] No music clip assigned to the bootstrap.",
                this);

            return;
        }

        musicSource.Play();

        Debug.Log(
            $"[MUSIC] Playing looping track '{musicClip.name}'.",
            this);
    }
}

//----- BootstrapMusic.cs END -----