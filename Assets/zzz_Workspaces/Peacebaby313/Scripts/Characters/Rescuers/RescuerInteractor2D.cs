//----- RescuerInteractor2D.cs START -----

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class RescuerInteractor2D : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField]
    private RescuerInventoryOwner inventoryOwner =
        RescuerInventoryOwner.Firefighter;

    [Header("Interaction")]
    [Tooltip(
        "Optional origin used to determine which nearby door is closest. " +
        "Defaults to this transform.")]
    [SerializeField]
    private Transform interactionOrigin;

    [Header("Runtime")]
    [SerializeField]
    private LockedDoor2D currentDoor;

    private readonly HashSet<LockedDoor2D> nearbyDoors =
        new HashSet<LockedDoor2D>();

    public RescuerInventoryOwner InventoryOwner =>
        inventoryOwner;

    public LockedDoor2D CurrentDoor =>
        currentDoor;

    private void Awake()
    {
        if (interactionOrigin == null)
        {
            interactionOrigin = transform;
        }
    }

    public void Interact()
    {
        RemoveInvalidDoors();
        SelectClosestDoor();

        if (currentDoor == null ||
            !currentDoor.CanInteract)
        {
            return;
        }

        currentDoor.TryOpen(inventoryOwner);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegisterDoor(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // This repairs the nearby-door state if a trigger-enter
        // message was missed due to enabling, spawning, or switching.
        RegisterDoor(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LockedDoor2D door =
            other.GetComponentInParent<LockedDoor2D>();

        if (door == null)
        {
            return;
        }

        nearbyDoors.Remove(door);

        if (currentDoor == door)
        {
            currentDoor = null;
            SelectClosestDoor();
        }
    }

    private void RegisterDoor(Collider2D other)
    {
        LockedDoor2D door =
            other.GetComponentInParent<LockedDoor2D>();

        if (door == null ||
            !door.CanInteract)
        {
            return;
        }

        nearbyDoors.Add(door);
        SelectClosestDoor();
    }

    private void SelectClosestDoor()
    {
        currentDoor = null;

        Vector3 origin =
            interactionOrigin != null
                ? interactionOrigin.position
                : transform.position;

        float closestSquaredDistance =
            float.PositiveInfinity;

        foreach (LockedDoor2D door in nearbyDoors)
        {
            if (door == null ||
                !door.CanInteract)
            {
                continue;
            }

            float squaredDistance =
                (door.transform.position - origin)
                .sqrMagnitude;

            if (squaredDistance >=
                closestSquaredDistance)
            {
                continue;
            }

            closestSquaredDistance =
                squaredDistance;

            currentDoor = door;
        }
    }

    private void RemoveInvalidDoors()
    {
        nearbyDoors.RemoveWhere(
            door =>
                door == null ||
                !door.CanInteract);
    }

    private void OnDisable()
    {
        nearbyDoors.Clear();
        currentDoor = null;
    }
}

//----- RescuerInteractor2D.cs END -----