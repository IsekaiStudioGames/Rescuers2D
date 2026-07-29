//----- FeedbackMenuController.cs START -----

using System;
using UnityEngine;

public enum FeedbackMenuState
{
    None,
    PlayerFeedback,
    Win,
    Lose
}

[DisallowMultipleComponent]
public sealed class FeedbackMenuController : MonoBehaviour
{
    public event Action<FeedbackMenuState> OnMenuStateChanged;

    [Header("Menu Hierarchy")]
    [SerializeField]
    private GameObject feedbackRoot;

    [SerializeField]
    private GameObject playerFeedbackRoot;

    [SerializeField]
    private GameObject winRoot;

    [SerializeField]
    private GameObject loseRoot;

    [Header("Startup")]
    [SerializeField]
    private FeedbackMenuState startingState =
        FeedbackMenuState.PlayerFeedback;

    public FeedbackMenuState CurrentState { get; private set; } =
        FeedbackMenuState.None;

    public bool IsMenuOpen =>
        CurrentState != FeedbackMenuState.None;

    private void Awake()
    {
        EnsureMenuHierarchyIsActive();
        SetMenuState(startingState);
    }

    public void SetMenuState(FeedbackMenuState newState)
    {
        EnsureMenuHierarchyIsActive();

        bool showPlayerFeedback =
            newState == FeedbackMenuState.PlayerFeedback;

        bool showWin =
            newState == FeedbackMenuState.Win;

        bool showLose =
            newState == FeedbackMenuState.Lose;

        SetRootActive(
            playerFeedbackRoot,
            showPlayerFeedback);

        SetRootActive(
            winRoot,
            showWin);

        SetRootActive(
            loseRoot,
            showLose);

        FeedbackMenuState previousState =
            CurrentState;

        CurrentState = newState;

        if (previousState != CurrentState)
        {
            OnMenuStateChanged?.Invoke(CurrentState);
        }

        //Debug.Log(
        //    $"[FEEDBACK MENU] State changed to {CurrentState}.",
        //    this);
    }

    [ContextMenu("Show Player Feedback")]
    public void ShowPlayerFeedback()
    {
        SetMenuState(
            FeedbackMenuState.PlayerFeedback);
    }

    [ContextMenu("Show Win")]
    public void ShowWin()
    {
        SetMenuState(
            FeedbackMenuState.Win);
    }

    [ContextMenu("Show Lose")]
    public void ShowLose()
    {
        SetMenuState(
            FeedbackMenuState.Lose);
    }

    [ContextMenu("Hide All")]
    public void HideAll()
    {
        SetMenuState(
            FeedbackMenuState.None);
    }

    private void EnsureMenuHierarchyIsActive()
    {
        if (feedbackRoot != null &&
            !feedbackRoot.activeSelf)
        {
            feedbackRoot.SetActive(true);

            Debug.LogWarning(
                "[FEEDBACK MENU] FeedbackRoot was inactive " +
                "and has been reactivated.",
                feedbackRoot);
        }
    }

    private static void SetRootActive(
        GameObject menuRoot,
        bool shouldBeActive)
    {
        if (menuRoot != null &&
            menuRoot.activeSelf != shouldBeActive)
        {
            menuRoot.SetActive(shouldBeActive);
        }
    }

    private void OnValidate()
    {
        ValidateReference(
            feedbackRoot,
            nameof(feedbackRoot));

        ValidateReference(
            playerFeedbackRoot,
            nameof(playerFeedbackRoot));

        ValidateReference(
            winRoot,
            nameof(winRoot));

        ValidateReference(
            loseRoot,
            nameof(loseRoot));
    }

    private void ValidateReference(
        GameObject referencedObject,
        string fieldName)
    {
        if (referencedObject == null)
        {
            return;
        }

        if (referencedObject == gameObject)
        {
            Debug.LogError(
                $"[FEEDBACK MENU] {fieldName} references " +
                "the controller's own GameObject. The controller " +
                "must not disable itself.",
                this);
        }
    }
}

//----- FeedbackMenuController.cs END -----