//----- LockedDoorAnimationRelay2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class LockedDoorAnimationRelay2D : MonoBehaviour
{
    [SerializeField] private LockedDoor2D lockedDoor;

    private void Awake()
    {
        if (lockedDoor == null)
        {
            lockedDoor =
                GetComponentInParent<LockedDoor2D>();
        }

        if (lockedDoor == null)
        {
            Debug.LogError(
                $"{nameof(LockedDoorAnimationRelay2D)} on '{name}' " +
                $"could not find a parent {nameof(LockedDoor2D)}.",
                this);
        }
    }

    // Called by the final frame of the opening animation.
    public void Anim_FinishOpening()
    {
        if (lockedDoor == null)
        {
            return;
        }

        lockedDoor.Anim_FinishOpening();
    }
}

//----- LockedDoorAnimationRelay2D.cs END -----
