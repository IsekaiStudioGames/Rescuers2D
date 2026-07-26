using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HUDFeedbackType
{
    Information,
    Success,
    Warning,
    Error
}

[DisallowMultipleComponent]
public sealed class HUDFeedbackPresenter : MonoBehaviour
{
    [Header("Presentation References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float defaultVisibleDuration = 2f;
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

    [Header("Feedback Colors")]
    [SerializeField]
    private Color informationColor =
        new Color32(17, 24, 39, 230);

    [SerializeField]
    private Color successColor =
        new Color32(34, 94, 55, 230);

    [SerializeField]
    private Color warningColor =
        new Color32(128, 91, 22, 230);

    [SerializeField]
    private Color errorColor =
        new Color32(122, 48, 48, 230);

    private Coroutine activeMessageRoutine;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        HideImmediate();
    }

    public void ShowMessage(
        string message,
        HUDFeedbackType feedbackType =
            HUDFeedbackType.Information,
        float visibleDuration = -1f)
    {
        if (!ValidateReferences())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            HideImmediate();
            return;
        }

        if (activeMessageRoutine != null)
        {
            StopCoroutine(activeMessageRoutine);
        }

        float resolvedDuration = visibleDuration >= 0f
            ? visibleDuration
            : defaultVisibleDuration;

        activeMessageRoutine = StartCoroutine(
            ShowMessageRoutine(
                message,
                feedbackType,
                resolvedDuration));
    }

    public void ShowInformation(string message)
    {
        ShowMessage(message, HUDFeedbackType.Information);
    }

    public void ShowSuccess(string message)
    {
        ShowMessage(message, HUDFeedbackType.Success);
    }

    public void ShowWarning(string message)
    {
        ShowMessage(message, HUDFeedbackType.Warning);
    }

    public void ShowError(string message)
    {
        ShowMessage(message, HUDFeedbackType.Error);
    }

    public void HideImmediate()
    {
        if (activeMessageRoutine != null)
        {
            StopCoroutine(activeMessageRoutine);
            activeMessageRoutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
        }
    }

    private IEnumerator ShowMessageRoutine(
        string message,
        HUDFeedbackType feedbackType,
        float visibleDuration)
    {
        feedbackText.text = message;
        backgroundImage.color =
            ResolveBackgroundColor(feedbackType);

        canvasGroup.alpha = 1f;

        if (visibleDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                visibleDuration);
        }

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(
                    1f,
                    0f,
                    elapsedTime / fadeDuration);

                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        feedbackText.text = string.Empty;
        activeMessageRoutine = null;
    }

    private Color ResolveBackgroundColor(
        HUDFeedbackType feedbackType)
    {
        return feedbackType switch
        {
            HUDFeedbackType.Success => successColor,
            HUDFeedbackType.Warning => warningColor,
            HUDFeedbackType.Error => errorColor,
            _ => informationColor
        };
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (canvasGroup == null)
        {
            Debug.LogError(
                $"{nameof(HUDFeedbackPresenter)} on '{name}' is missing its Canvas Group reference.",
                this);

            valid = false;
        }

        if (backgroundImage == null)
        {
            Debug.LogError(
                $"{nameof(HUDFeedbackPresenter)} on '{name}' is missing its Background Image reference.",
                this);

            valid = false;
        }

        if (feedbackText == null)
        {
            Debug.LogError(
                $"{nameof(HUDFeedbackPresenter)} on '{name}' is missing its Feedback Text reference.",
                this);

            valid = false;
        }

        return valid;
    }

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        backgroundImage = GetComponent<Image>();

        Transform textTransform =
            transform.Find("FeedbackText");

        if (textTransform != null)
        {
            feedbackText =
                textTransform.GetComponent<TMP_Text>();
        }
    }
}