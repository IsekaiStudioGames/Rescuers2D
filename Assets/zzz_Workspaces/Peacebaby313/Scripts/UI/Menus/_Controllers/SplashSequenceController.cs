//----- SplashSequenceController.cs START -----

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class SplashSequenceController : MonoBehaviour
{
    [Header("Sequence")]
    [SerializeField] private SplashSequenceData sequenceData;
    [SerializeField] private bool playOnStart = true;

    [Header("Scene Handoff")]
    [SerializeField] private SceneLoadService sceneLoadService;
    [SerializeField] private string nextSceneName = "01_MainMenu";

    [Header("UI References")]
    [SerializeField] private CanvasGroup splashCanvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image splashImage;
    [SerializeField] private Image glowImage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private Coroutine sequenceRoutine;

    private Vector3 glowBaseScale = Vector3.one;
    private float glowElapsedTime;

    public bool IsPlaying => sequenceRoutine != null;

    private void Awake()
    {
        if (glowImage != null)
            glowBaseScale = glowImage.rectTransform.localScale;

        ResetPresentation();
    }

    private void Start()
    {
        if (playOnStart)
            PlaySequence();
    }


    // Begins the configured splash sequence.
    // Duplicate requests are ignored while the sequence is playing.

    public void PlaySequence()
    {
        if (sequenceRoutine != null)
            return;

        sequenceRoutine =
            StartCoroutine(PlaySequenceRoutine());
    }

    private IEnumerator PlaySequenceRoutine()
    {
        if (sequenceData == null)
        {
            Debug.LogError(
                "[SPLASH] No SplashSequenceData is assigned. " +
                "Attempting to load the next scene.");

            sequenceRoutine = null;

            yield return LoadNextScene();
            yield break;
        }

        if (sequenceData.Entries.Count == 0)
        {
            Debug.LogWarning(
                "[SPLASH] The assigned sequence contains no entries. " +
                "Attempting to load the next scene.");

            sequenceRoutine = null;

            yield return LoadNextScene();
            yield break;
        }

        foreach (SplashEntry entry in sequenceData.Entries)
        {
            if (entry == null)
                continue;

            yield return PlayEntry(entry);
        }

        ResetPresentation();

        sequenceRoutine = null;

        yield return LoadNextScene();
    }

    private IEnumerator PlayEntry(SplashEntry entry)
    {
        ConfigureEntry(entry);

        glowElapsedTime = 0f;

        PlaySound(entry);

        yield return Fade(
            entry,
            fromAlpha: 0f,
            toAlpha: 1f,
            duration: entry.FadeInDuration);

        yield return Hold(
            entry,
            entry.HoldDuration);

        yield return Fade(
            entry,
            fromAlpha: 1f,
            toAlpha: 0f,
            duration: entry.FadeOutDuration);

        if (audioSource != null)
            audioSource.Stop();
    }

    private IEnumerator Fade(
        SplashEntry entry,
        float fromAlpha,
        float toAlpha,
        float duration)
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
            float deltaTime = Time.unscaledDeltaTime;

            elapsedTime += deltaTime;
            glowElapsedTime += deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / duration);

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
            yield break;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float deltaTime = Time.unscaledDeltaTime;

            elapsedTime += deltaTime;
            glowElapsedTime += deltaTime;

            SetPresentationAlpha(1f);
            UpdateGlow(entry);

            yield return null;
        }
    }

    private void ConfigureEntry(SplashEntry entry)
    {
        if (backgroundImage != null)
            backgroundImage.color = entry.BackgroundColor;

        if (splashImage != null)
        {
            splashImage.sprite = entry.SplashSprite;
            splashImage.preserveAspect = true;
            splashImage.enabled =
                entry.SplashSprite != null;
        }

        if (glowImage != null)
        {
            glowImage.sprite = entry.SplashSprite;
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

    private void PlaySound(SplashEntry entry)
    {
        if (audioSource == null)
            return;

        audioSource.Stop();

        audioSource.clip = entry.Sound;
        audioSource.volume = entry.SoundVolume;

        if (audioSource.clip != null)
            audioSource.Play();
    }

    private void UpdateGlow(SplashEntry entry)
    {
        if (glowImage == null || !glowImage.enabled)
            return;

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

        Color glowColor = glowImage.color;
        glowColor.a = glowAlpha;
        glowImage.color = glowColor;
    }

    private void SetPresentationAlpha(float alpha)
    {
        if (splashCanvasGroup == null)
            return;

        splashCanvasGroup.alpha =
            Mathf.Clamp01(alpha);
    }

    private IEnumerator LoadNextScene()
    {
        
        if (sceneLoadService == null)
        {
            Debug.LogError(
                "[SPLASH] No SceneLoadService is assigned. " +
                $"Unable to load scene '{nextSceneName}'.");

            yield break;
        }

        yield return
            sceneLoadService.LoadSceneRoutine(
                nextSceneName);
    }

    private void ResetPresentation()
    {
        if (audioSource != null)
            audioSource.Stop();

        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0f;
            splashCanvasGroup.interactable = false;
            splashCanvasGroup.blocksRaycasts = false;
        }

        if (backgroundImage != null)
            backgroundImage.color = Color.black;

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
        if (audioSource != null)
            audioSource.Stop();
    }
}

//----- SplashSequenceController.cs END -----

