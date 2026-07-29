//----- SplashSequenceController.cs START -----

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class SplashSequenceController : MonoBehaviour
{
    [Header("Sequence")]
    [SerializeField]
    private SplashSequenceData sequenceData;

    [SerializeField]
    private bool playOnStart = true;

    [Header("Click Through")]
    [SerializeField]
    private bool allowClickToAdvance = true;

    [SerializeField, Min(0f)]
    private float clickedFadeOutDuration = 0.15f;

    [Header("Scene Handoff")]
    [SerializeField]
    private SceneLoadService sceneLoadService;

    [SerializeField]
    private string nextSceneName = "01_MainMenu";

    [Header("UI References")]
    [SerializeField]
    private CanvasGroup splashCanvasGroup;

    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private Image splashImage;

    [SerializeField]
    private Image glowImage;

    private SfxPlayer sfxPlayer;

    private SfxPlaybackHandle splashSoundHandle =
        SfxPlaybackHandle.Invalid;

    private Coroutine sequenceRoutine;

    private Vector3 glowBaseScale = Vector3.one;
    private float glowElapsedTime;

    private bool advanceRequested;
    private bool isLoadingNextScene;
    private bool hasLoggedMissingSfxPlayer;

    public bool IsPlaying =>
        sequenceRoutine != null;

    private void Awake()
    {
        if (glowImage != null)
        {
            glowBaseScale =
                glowImage.rectTransform.localScale;
        }

        TryResolveSfxPlayer();
        ResetPresentation();
    }

    private void Start()
    {
        if (playOnStart)
        {
            PlaySequence();
        }
    }

    private void Update()
    {
        if (!allowClickToAdvance ||
            sequenceRoutine == null ||
            isLoadingNextScene ||
            advanceRequested)
        {
            return;
        }

        bool mouseClicked =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool screenTouched =
            Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press
                .wasPressedThisFrame;

        if (mouseClicked || screenTouched)
        {
            advanceRequested = true;
        }
    }

    // Begins the configured splash sequence.
    // Duplicate requests are ignored while the sequence is playing.

    public void PlaySequence()
    {
        if (sequenceRoutine != null ||
            isLoadingNextScene)
        {
            return;
        }

        sequenceRoutine =
            StartCoroutine(PlaySequenceRoutine());
    }

    private IEnumerator PlaySequenceRoutine()
    {
        if (sequenceData == null)
        {
            Debug.LogError(
                "[SPLASH] No SplashSequenceData is assigned. " +
                "Attempting to load the next scene.",
                this);

            yield return LoadNextScene();

            sequenceRoutine = null;
            yield break;
        }

        if (sequenceData.Entries.Count == 0)
        {
            Debug.LogWarning(
                "[SPLASH] The assigned sequence contains no entries. " +
                "Attempting to load the next scene.",
                this);

            yield return LoadNextScene();

            sequenceRoutine = null;
            yield break;
        }

        foreach (SplashEntry entry in sequenceData.Entries)
        {
            if (entry == null)
            {
                continue;
            }

            yield return PlayEntry(entry);
        }

        ResetPresentation();

        yield return LoadNextScene();

        sequenceRoutine = null;
    }

    private IEnumerator PlayEntry(SplashEntry entry)
    {
        advanceRequested = false;

        ConfigureEntry(entry);

        glowElapsedTime = 0f;

        PlaySound(entry);

        // Clicking during the fade-in immediately begins
        // the shortened click-through fade-out.

        yield return Fade(
            entry,
            fromAlpha: 0f,
            toAlpha: 1f,
            duration: entry.FadeInDuration,
            canBeAdvanced: true);

        if (!advanceRequested)
        {
            yield return Hold(
                entry,
                entry.HoldDuration);
        }

        if (!advanceRequested)
        {
            yield return Fade(
                entry,
                fromAlpha: 1f,
                toAlpha: 0f,
                duration: entry.FadeOutDuration,
                canBeAdvanced: true);
        }

        // If the player clicked during any presentation phase,
        // finish the current splash with a brief fade.

        if (advanceRequested)
        {
            float currentAlpha =
                splashCanvasGroup != null
                    ? splashCanvasGroup.alpha
                    : 0f;

            yield return Fade(
                entry,
                fromAlpha: currentAlpha,
                toAlpha: 0f,
                duration: clickedFadeOutDuration,
                canBeAdvanced: false);
        }

        SetPresentationAlpha(0f);
        StopSplashSound();

        advanceRequested = false;
    }

    private IEnumerator Fade(
        SplashEntry entry,
        float fromAlpha,
        float toAlpha,
        float duration,
        bool canBeAdvanced)
    {
        if (duration <= 0f)
        {
            SetPresentationAlpha(toAlpha);
            UpdateGlow(entry);

            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (canBeAdvanced &&
                advanceRequested)
            {
                yield break;
            }

            float deltaTime =
                Time.unscaledDeltaTime;

            elapsedTime += deltaTime;
            glowElapsedTime += deltaTime;

            float normalizedTime =
                Mathf.Clamp01(
                    elapsedTime / duration);

            float currentAlpha =
                Mathf.Lerp(
                    fromAlpha,
                    toAlpha,
                    normalizedTime);

            SetPresentationAlpha(currentAlpha);
            UpdateGlow(entry);

            yield return null;
        }

        SetPresentationAlpha(toAlpha);
        UpdateGlow(entry);
    }

    private IEnumerator Hold(
        SplashEntry entry,
        float duration)
    {
        if (duration <= 0f)
        {
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (advanceRequested)
            {
                yield break;
            }

            float deltaTime =
                Time.unscaledDeltaTime;

            elapsedTime += deltaTime;
            glowElapsedTime += deltaTime;

            SetPresentationAlpha(1f);
            UpdateGlow(entry);

            yield return null;
        }
    }

    private void ConfigureEntry(
        SplashEntry entry)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color =
                entry.BackgroundColor;
        }

        if (splashImage != null)
        {
            splashImage.sprite =
                entry.SplashSprite;

            splashImage.preserveAspect = true;

            splashImage.enabled =
                entry.SplashSprite != null;
        }

        if (glowImage != null)
        {
            glowImage.sprite =
                entry.SplashSprite;

            glowImage.preserveAspect = true;
            glowImage.raycastTarget = false;

            glowImage.rectTransform.localScale =
                glowBaseScale;

            glowImage.enabled =
                entry.UseGlowPulse &&
                entry.SplashSprite != null;
        }

        SetPresentationAlpha(0f);
    }

    private void PlaySound(
        SplashEntry entry)
    {
        StopSplashSound();

        if (entry.Sound == null)
        {
            return;
        }

        if (!TryResolveSfxPlayer())
        {
            return;
        }

        splashSoundHandle =
            sfxPlayer.Play(
                entry.Sound,
                entry.SoundVolume);
    }

    private void StopSplashSound()
    {
        if (!splashSoundHandle.IsValid)
        {
            return;
        }

        if (TryResolveSfxPlayer())
        {
            sfxPlayer.Stop(
                splashSoundHandle);
        }

        splashSoundHandle =
            SfxPlaybackHandle.Invalid;
    }

    private bool TryResolveSfxPlayer()
    {
        if (sfxPlayer != null)
        {
            return true;
        }

        ApplicationBootstrap bootstrap =
            ApplicationBootstrap.Instance;

        if (bootstrap != null)
        {
            sfxPlayer =
                bootstrap.SfxPlayer;
        }

        if (sfxPlayer != null)
        {
            hasLoggedMissingSfxPlayer = false;
            return true;
        }

        if (!hasLoggedMissingSfxPlayer)
        {
            Debug.LogWarning(
                "[SPLASH] Could not resolve the persistent SfxPlayer. " +
                "Splash audio will not play.",
                this);

            hasLoggedMissingSfxPlayer = true;
        }

        return false;
    }

    private void UpdateGlow(
        SplashEntry entry)
    {
        if (glowImage == null ||
            !glowImage.enabled)
        {
            return;
        }

        float wave =
            (Mathf.Sin(
                glowElapsedTime *
                entry.GlowPulseSpeed *
                Mathf.PI *
                2f) + 1f) * 0.5f;

        float glowScale =
            Mathf.Lerp(
                1f,
                entry.GlowMaximumScale,
                wave);

        float glowAlpha =
            Mathf.Lerp(
                0f,
                entry.GlowMaximumAlpha,
                wave);

        glowImage.rectTransform.localScale =
            glowBaseScale * glowScale;

        Color glowColor =
            glowImage.color;

        glowColor.a =
            glowAlpha;

        glowImage.color =
            glowColor;
    }

    private void SetPresentationAlpha(
        float alpha)
    {
        if (splashCanvasGroup == null)
        {
            return;
        }

        splashCanvasGroup.alpha =
            Mathf.Clamp01(alpha);
    }

    private IEnumerator LoadNextScene()
    {
        if (isLoadingNextScene)
        {
            yield break;
        }

        isLoadingNextScene = true;

        StopSplashSound();

        if (sceneLoadService == null)
        {
            Debug.LogError(
                "[SPLASH] No SceneLoadService is assigned. " +
                $"Unable to load scene '{nextSceneName}'.",
                this);

            isLoadingNextScene = false;
            yield break;
        }

        yield return
            sceneLoadService.LoadSceneRoutine(
                nextSceneName);
    }

    private void ResetPresentation()
    {
        StopSplashSound();

        advanceRequested = false;

        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0f;
            splashCanvasGroup.interactable = false;
            splashCanvasGroup.blocksRaycasts = false;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color =
                Color.black;
        }

        if (splashImage != null)
        {
            splashImage.sprite = null;
            splashImage.enabled = false;
        }

        if (glowImage != null)
        {
            glowImage.sprite = null;
            glowImage.enabled = false;

            glowImage.rectTransform.localScale =
                glowBaseScale;
        }
    }

    private void OnDisable()
    {
        StopSplashSound();

        advanceRequested = false;
        sequenceRoutine = null;
    }

    private void OnValidate()
    {
        clickedFadeOutDuration =
            Mathf.Max(
                0f,
                clickedFadeOutDuration);

        nextSceneName =
            nextSceneName?.Trim() ?? string.Empty;
    }
}

//----- SplashSequenceController.cs END -----