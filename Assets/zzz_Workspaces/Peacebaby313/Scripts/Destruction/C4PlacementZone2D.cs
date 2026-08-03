//----- C4PlacementZone2D.cs START -----

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(DestructibleTilemap2D))]
public sealed class C4PlacementZone2D : MonoBehaviour
{
    private const RescuerInventoryOwner RequiredRescuer =
        RescuerInventoryOwner.RiotOfficer;

    private static readonly List<C4PlacementZone2D>
        ActiveZones = new();

    [Header("Tile Placement")]
    [SerializeField]
    private Tilemap placementTilemap;

    [SerializeField]
    private DestructibleTilemap2D destructibleTilemap;

    [Tooltip(
        "How close the Riot Officer must be to a destructible " +
        "tile's center.")]
    [SerializeField, Min(0.1f)]
    private float maximumPlacementDistance = 1.75f;

    [Tooltip(
        "Applied after snapping the C4 to the center of the tile.")]
    [SerializeField]
    private Vector3 placementOffset;

    [SerializeField]
    private Vector3 placementEulerRotation;

    [Header("C4")]
    [SerializeField]
    private C4Charge2D c4Prefab;

    [Tooltip(
        "No other C4 can be placed this close to an active charge. " +
        "The lock remains until the existing charge is destroyed.")]
    [SerializeField, Min(0.1f)]
    private float nearbyChargeBlockingRadius = 3f;

    [Header("Inventory Requirement")]
    [SerializeField]
    private string requiredItemId = "c4";

    [SerializeField, Min(1)]
    private int requiredQuantity = 1;

    [Header("Optional Feedback")]
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    private TeamInventory teamInventory;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        ActiveZones.Clear();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        if (!ActiveZones.Contains(this))
        {
            ActiveZones.Add(this);
        }
    }

    private void OnDisable()
    {
        ActiveZones.Remove(this);
    }

    /// <summary>
    /// Searches every active destructible Tilemap and places C4 on
    /// the closest occupied tile within placement distance.
    /// </summary>
    public static bool TryPlaceNearest(
        Vector2 requesterWorldPosition,
        RescuerInventoryOwner requestingRescuer)
    {
        HUDFeedbackPresenter fallbackFeedback =
            FindFirstObjectByType<HUDFeedbackPresenter>();

        if (requestingRescuer != RequiredRescuer)
        {
            fallbackFeedback?.ShowWarning(
                "Only the Riot Officer can place C4.");

            return false;
        }

        C4PlacementZone2D closestZone = null;
        Vector3 closestPlacementPosition = default;

        float closestSquaredDistance =
            float.PositiveInfinity;

        for (int index = ActiveZones.Count - 1;
             index >= 0;
             index--)
        {
            C4PlacementZone2D zone =
                ActiveZones[index];

            if (zone == null)
            {
                ActiveZones.RemoveAt(index);
                continue;
            }

            if (!zone.isActiveAndEnabled)
            {
                continue;
            }

            if (!zone.TryGetClosestPlacementPosition(
                    requesterWorldPosition,
                    out Vector3 placementPosition,
                    out float squaredDistance))
            {
                continue;
            }

            if (squaredDistance >= closestSquaredDistance)
            {
                continue;
            }

            closestZone =
                zone;

            closestPlacementPosition =
                placementPosition;

            closestSquaredDistance =
                squaredDistance;
        }

        if (closestZone == null)
        {
            fallbackFeedback?.ShowWarning(
                "The Riot Officer must stand next to a " +
                "destructible tile to place C4.");

            return false;
        }

        return closestZone.TryConsumeAndPlaceC4(
            closestPlacementPosition,
            requestingRescuer,
            fallbackFeedback);
    }

    private bool TryGetClosestPlacementPosition(
        Vector2 requesterWorldPosition,
        out Vector3 placementPosition,
        out float closestSquaredDistance)
    {
        placementPosition =
            default;

        closestSquaredDistance =
            float.PositiveInfinity;

        ResolveReferences();

        if (placementTilemap == null ||
            destructibleTilemap == null)
        {
            return false;
        }

        float searchDistance =
            Mathf.Max(
                0.1f,
                maximumPlacementDistance);

        float maximumSquaredDistance =
            searchDistance * searchDistance;

        Vector3 minimumWorldPosition =
            new(
                requesterWorldPosition.x - searchDistance,
                requesterWorldPosition.y - searchDistance,
                placementTilemap.transform.position.z);

        Vector3 maximumWorldPosition =
            new(
                requesterWorldPosition.x + searchDistance,
                requesterWorldPosition.y + searchDistance,
                placementTilemap.transform.position.z);

        Vector3Int firstCell =
            placementTilemap.WorldToCell(
                minimumWorldPosition);

        Vector3Int secondCell =
            placementTilemap.WorldToCell(
                maximumWorldPosition);

        int minimumX =
            Mathf.Min(
                firstCell.x,
                secondCell.x);

        int maximumX =
            Mathf.Max(
                firstCell.x,
                secondCell.x);

        int minimumY =
            Mathf.Min(
                firstCell.y,
                secondCell.y);

        int maximumY =
            Mathf.Max(
                firstCell.y,
                secondCell.y);

        int cellZ =
            placementTilemap
                .WorldToCell(requesterWorldPosition)
                .z;

        bool foundTile =
            false;

        for (int x = minimumX;
             x <= maximumX;
             x++)
        {
            for (int y = minimumY;
                 y <= maximumY;
                 y++)
            {
                Vector3Int cellPosition =
                    new(x, y, cellZ);

                if (!placementTilemap.HasTile(
                        cellPosition))
                {
                    continue;
                }

                Vector3 cellCenter =
                    placementTilemap.GetCellCenterWorld(
                        cellPosition);

                Vector2 cellCenter2D =
                    cellCenter;

                float squaredDistance =
                    (cellCenter2D - requesterWorldPosition)
                    .sqrMagnitude;

                if (squaredDistance >
                    maximumSquaredDistance)
                {
                    continue;
                }

                if (squaredDistance >=
                    closestSquaredDistance)
                {
                    continue;
                }

                foundTile =
                    true;

                closestSquaredDistance =
                    squaredDistance;

                placementPosition =
                    cellCenter + placementOffset;
            }
        }

        return foundTile;
    }

    private bool TryConsumeAndPlaceC4(
        Vector3 placementPosition,
        RescuerInventoryOwner requestingRescuer,
        HUDFeedbackPresenter fallbackFeedback)
    {
        ResolveReferences();

        HUDFeedbackPresenter feedback =
            feedbackPresenter != null
                ? feedbackPresenter
                : fallbackFeedback;

        if (requestingRescuer != RequiredRescuer)
        {
            feedback?.ShowWarning(
                "Only the Riot Officer can place C4.");

            return false;
        }

        if (C4Charge2D.IsPlacementBlocked(
                placementPosition,
                nearbyChargeBlockingRadius))
        {
            feedback?.ShowWarning(
                "Another C4 is already active in this area.");

            return false;
        }

        if (c4Prefab == null)
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                "has no C4 Prefab assigned.",
                this);

            feedback?.ShowWarning(
                "C4 placement is not configured.");

            return false;
        }

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                $"could not find a {nameof(TeamInventory)}.",
                this);

            feedback?.ShowWarning(
                "The team inventory could not be found.");

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                requiredItemId))
        {
            Debug.LogError(
                $"{nameof(C4PlacementZone2D)} on '{name}' " +
                "has no Required Item Id.",
                this);

            return false;
        }

        bool canUseC4 =
            teamInventory.CanUseItem(
                requiredItemId,
                requiredQuantity,
                requestingRescuer);

        if (!canUseC4)
        {
            feedback?.ShowWarning(
                "The Riot Officer does not have access to C4.");

            return false;
        }

        Quaternion placementRotation =
            transform.rotation *
            Quaternion.Euler(
                placementEulerRotation);

        C4Charge2D newCharge =
            Instantiate(
                c4Prefab,
                placementPosition,
                placementRotation);

        if (newCharge == null)
        {
            Debug.LogError(
                "The C4 prefab could not be instantiated.",
                this);

            return false;
        }

        bool consumedC4 =
            teamInventory.TryUseItem(
                requiredItemId,
                requiredQuantity,
                requestingRescuer);

        if (!consumedC4)
        {
            Destroy(
                newCharge.gameObject);

            Debug.LogError(
                "C4 inventory validation succeeded, but the " +
                "inventory transaction failed.",
                this);

            return false;
        }

        // Gives the charge its originating Tilemap before it is armed.
        newCharge.Initialize(
            destructibleTilemap);

        newCharge.PlayPlacementFeedback();
        newCharge.Arm();

        feedback?.ShowSuccess(
            "The Riot Officer placed the C4.");

        Debug.Log(
            $"C4 placed on destructible Tilemap " +
            $"'{placementTilemap.name}' at " +
            $"{placementPosition}.",
            this);

        return true;
    }

    private void ResolveReferences()
    {
        if (placementTilemap == null)
        {
            placementTilemap =
                GetComponent<Tilemap>();
        }

        if (destructibleTilemap == null)
        {
            destructibleTilemap =
                GetComponent<DestructibleTilemap2D>();
        }

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

    private void OnValidate()
    {
        maximumPlacementDistance =
            Mathf.Max(
                0.1f,
                maximumPlacementDistance);

        nearbyChargeBlockingRadius =
            Mathf.Max(
                0.1f,
                nearbyChargeBlockingRadius);

        requiredQuantity =
            Mathf.Max(
                1,
                requiredQuantity);

        requiredItemId =
            requiredItemId?.Trim() ??
            string.Empty;

        ResolveReferences();
    }
}

//----- C4PlacementZone2D.cs END -----