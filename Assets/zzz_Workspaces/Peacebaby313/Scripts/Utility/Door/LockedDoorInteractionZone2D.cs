//----- LockedDoorInteractionZone2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class LockedDoorInteractionZone2D :
    MonoBehaviour
{
    [SerializeField] private LockedDoor2D lockedDoor;

    private void Awake()
    {
        Collider2D interactionCollider =
            GetComponent<Collider2D>();

        if (!interactionCollider.isTrigger)
        {
            Debug.LogWarning(
                $"{nameof(LockedDoorInteractionZone2D)} on " +
                $"'{name}' requires Is Trigger. It has been " +
                "enabled automatically.",
                this);

            interactionCollider.isTrigger = true;
        }

        if (lockedDoor == null)
        {
            lockedDoor =
                GetComponentInParent<LockedDoor2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FirefighterController firefighter =
            other.GetComponentInParent<FirefighterController>();

        if (firefighter == null ||
            lockedDoor == null)
        {
            return;
        }

        //firefighter.EnterLockedDoorZone(lockedDoor);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FirefighterController firefighter =
            other.GetComponentInParent<FirefighterController>();

        if (firefighter == null ||
            lockedDoor == null)
        {
            return;
        }

        //firefighter.ExitLockedDoorZone(lockedDoor);
    }
}

//----- LockedDoorInteractionZone2D.cs END -----