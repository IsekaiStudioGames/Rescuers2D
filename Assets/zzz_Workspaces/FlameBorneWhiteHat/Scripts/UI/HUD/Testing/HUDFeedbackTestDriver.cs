using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HUDFeedbackTestDriver : MonoBehaviour
{
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    private IEnumerator Start()
    {
        if (feedbackPresenter == null)
        {
            Debug.LogError(
                $"{nameof(HUDFeedbackTestDriver)} on '{name}' is missing its Feedback Presenter reference.",
                this);

            enabled = false;
            yield break;
        }

        feedbackPresenter.ShowMessage(
            "Inventory updated.",
            HUDFeedbackType.Information,
            1.25f);

        yield return new WaitForSecondsRealtime(2f);

        feedbackPresenter.ShowMessage(
            "Item added.",
            HUDFeedbackType.Success,
            1.25f);

        yield return new WaitForSecondsRealtime(2f);

        feedbackPresenter.ShowMessage(
            "No open inventory slot.",
            HUDFeedbackType.Warning,
            1.25f);

        yield return new WaitForSecondsRealtime(2f);

        feedbackPresenter.ShowMessage(
            "Action unavailable.",
            HUDFeedbackType.Error,
            1.25f);
    }

    private void Reset()
    {
        feedbackPresenter =
            FindFirstObjectByType<HUDFeedbackPresenter>();
    }
}
