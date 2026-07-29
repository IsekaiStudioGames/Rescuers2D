//----- SplashSequenceData.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SplashSequence_New",
    menuName = "Rescuers2D/UI/Splash Sequence Data")]
public sealed class SplashSequenceData : ScriptableObject
{
    [SerializeField]
    private SplashEntry[] entries = Array.Empty<SplashEntry>();

    public IReadOnlyList<SplashEntry> Entries => entries;
}

[Serializable]
public sealed class SplashEntry
{
    [Header("Identity")]
    [SerializeField] private string splashId = "Splash";

    [Header("Visuals")]
    [SerializeField] private Sprite splashSprite;
    [SerializeField] private Color backgroundColor = Color.black;

    [Header("Audio")]
    [SerializeField] private SfxCueData sound;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float fadeInDuration = 0.4f;
    [SerializeField, Min(0f)] private float holdDuration = 1.25f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 0.4f;

    [Header("Glow")]
    [SerializeField] private bool useGlowPulse = true;

    [SerializeField, Min(0.01f)]
    private float glowPulseSpeed = 1.5f;

    [SerializeField, Range(1f, 1.25f)]
    private float glowMaximumScale = 1.05f;

    [SerializeField, Range(0f, 1f)]
    private float glowMaximumAlpha = 0.3f;

    public string SplashId => splashId;

    public Sprite SplashSprite => splashSprite;
    public Color BackgroundColor => backgroundColor;

    public SfxCueData Sound => sound;

    public float SoundVolume => soundVolume;

    public float FadeInDuration => fadeInDuration;
    public float HoldDuration => holdDuration;
    public float FadeOutDuration => fadeOutDuration;

    public bool UseGlowPulse => useGlowPulse;
    public float GlowPulseSpeed => glowPulseSpeed;
    public float GlowMaximumScale => glowMaximumScale;
    public float GlowMaximumAlpha => glowMaximumAlpha;
}

//----- SplashSequenceData.cs END -----