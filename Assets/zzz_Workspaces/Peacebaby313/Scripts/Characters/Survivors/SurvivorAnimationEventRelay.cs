//----- SurvivorAnimationEventRelay.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class SurvivorAnimationEventRelay : MonoBehaviour
{
    [SerializeField]
    private SurvivorFollower2D survivorFollower;

    private void Reset()
    {
        ResolveController();
    }

    private void Awake()
    {
        ResolveController();

        if (survivorFollower == null)
        {
            Debug.LogError(
                "[SURVIVOR ANIMATION] SurvivorFollower2D " +
                "was not found in the parent hierarchy.",
                this);
        }
    }

    public void Anim_PlayFootstep()
    {
        survivorFollower?.Anim_PlayFootstep();
    }

    public void Anim_PlayClimbStep()
    {
        survivorFollower?.Anim_PlayClimbStep();
    }

    private void ResolveController()
    {
        if (survivorFollower != null)
        {
            return;
        }

        survivorFollower =
            GetComponentInParent<SurvivorFollower2D>();
    }
}

//----- SurvivorAnimationEventRelay.cs END -----