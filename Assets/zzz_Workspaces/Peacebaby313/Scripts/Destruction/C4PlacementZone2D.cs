//----- C4PlacementZone2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class C4PlacementZone2D : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField]
    private C4Charge2D c4Prefab;

    [Tooltip(
        "The exact position and rotation where the C4 appears.")]
    [SerializeField]
    private Transform placementPoint;

    [Header("Item Requirement")]
    [SerializeField]
    private string requiredItemId = "c4";

    [SerializeField, Min(1)]
    private int requiredQuantity = 1;

    [SerializeField]
    private RescuerInventoryOwner requiredRescuer =
        RescuerInventoryOwner.Firefighter;

    [Header("Optional Feedback")]
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    [Header("Runtime State")]
    [SerializeField]
    private bool hasPlacedC4;

    private TeamInventory teamInventory;
    private C4Charge2D placedCharge;

    public bool HasPlacedC4 =>
        hasPlacedC4;

    public bool CanInteract =>
        !hasPlacedC4 &&
        c4Prefab != null;

    private void Awake()
    {
        ResolveReferences();
    }

    public void TryPlaceC4(
        RescuerInventoryOwner requestingRescuer)
    {
        if (!CanInteract)
        {
            return;
        }

        ResolveReferences();

        if (requestingRescuer != requiredRescuer)
        {
            ShowWrongRescuerMessage();
            return;
        }

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                $"could not find a {nameof(TeamInventory)}.",
                this);

            return;
        }

        if (c4Prefab == null)
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                "has no C4 Prefab assigned.",
                this);

            return;
        }

        if (string.IsNullOrWhiteSpace(requiredItemId))
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                "has no Required Item Id.",
                this);

            return;
        }

        bool canUseC4 =
            teamInventory.CanUseItem(
                requiredItemId,
                requiredQuantity,
                requestingRescuer);

        if (!canUseC4)
        {
            ShowMissingC4Message(requestingRescuer);
            return;
        }

        bool consumedC4 =
            teamInventory.TryUseItem(
                requiredItemId,
                requiredQuantity,
                requestingRescuer);

        if (!consumedC4)
        {
            Debug.LogError(
                $"The placement zone validated " +
                $"'{requiredItemId}' but could not consume it.",
                this);

            return;
        }

        SpawnAndArmC4(requestingRescuer);
    }

    private void SpawnAndArmC4(
        RescuerInventoryOwner requestingRescuer)
    {
        Vector3 spawnPosition =
            placementPoint != null
                ? placementPoint.position
                : transform.position;

        Quaternion spawnRotation =
            placementPoint != null
                ? placementPoint.rotation
                : transform.rotation;

        placedCharge =
            Instantiate(
                c4Prefab,
                spawnPosition,
                spawnRotation);

        hasPlacedC4 = true;

        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowSuccess(
                $"{GetOwnerDisplayName(requestingRescuer)} " +
                "placed the C4.");
        }

        Debug.Log(
            $"{requestingRescuer} placed C4 at '{name}'.",
            this);

        placedCharge.Arm();
    }

    private void ResolveReferences()
    {
        if (teamInventory == null)
        {
            teamInventory =
                FindFirstObjectByType<TeamInventory>();
        }

        if (feedbackPresenter == null)
        {
            feedbackPresenter =
                FindFirstObjectByType<HUDFeedbackPresenter>();
        }
    }

    private void ShowWrongRescuerMessage()
    {
        if (feedbackPresenter == null)
        {
            return;
        }

        feedbackPresenter.ShowWarning(
            $"{GetOwnerDisplayName(requiredRescuer)} " +
            "must place the C4.");
    }

    private void ShowMissingC4Message(
        RescuerInventoryOwner requestingRescuer)
    {
        if (feedbackPresenter == null)
        {
            return;
        }

        feedbackPresenter.ShowWarning(
            $"{GetOwnerDisplayName(requestingRescuer)} " +
            "does not have access to C4.");
    }

    private static string GetOwnerDisplayName(
        RescuerInventoryOwner owner)
    {
        return owner switch
        {
            RescuerInventoryOwner.Firefighter =>
                "The Firefighter",

            RescuerInventoryOwner.RiotOfficer =>
                "The Riot Officer",

            RescuerInventoryOwner.Specialist =>
                "The Specialist",

            _ => "This rescuer"
        };
    }

    private void OnValidate()
    {
        requiredItemId =
            requiredItemId?.Trim() ?? string.Empty;

        requiredQuantity =
            Mathf.Max(1, requiredQuantity);
    }
}

//----- C4PlacementZone2D.cs END -----