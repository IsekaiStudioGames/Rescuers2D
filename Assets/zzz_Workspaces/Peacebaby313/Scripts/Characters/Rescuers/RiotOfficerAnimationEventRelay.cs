//----- RiotOfficerAnimationEventRelay.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class RiotOfficerAnimationEventRelay
    : MonoBehaviour
{
    [SerializeField]
    private RiotOfficerController riotOfficerController;

    private void Reset()
    {
        ResolveController();
    }

    private void Awake()
    {
        ResolveController();

        if (riotOfficerController == null)
        {
            Debug.LogError(
                "[RIOT OFFICER ANIMATION] " +
                "RiotOfficerController was not found " +
                "in the parent hierarchy.",
                this);
        }
    }

    public void Anim_PlayFootstep()
    {
        riotOfficerController?.Anim_PlayFootstep();
    }

    public void Anim_PlayBashImpact()
    {
        riotOfficerController?.Anim_PlayBashImpact();
    }


    private void ResolveController()
    {
        if (riotOfficerController != null)
        {
            return;
        }

        riotOfficerController =
            GetComponentInParent<RiotOfficerController>();
    }
}

//----- RiotOfficerAnimationEventRelay.cs END -----