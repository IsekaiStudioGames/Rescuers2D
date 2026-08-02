//----- RescueSpecialistAnimationEventRelay.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class RescueSpecialistAnimationEventRelay : MonoBehaviour
{
    [SerializeField]
    private RescueSpecialistController specialistController;

    private void Reset()
    {
        ResolveController();
    }

    private void Awake()
    {
        ResolveController();

        if (specialistController == null)
        {
            Debug.LogError(
                "[RESCUE SPECIALIST ANIMATION] " +
                "RescueSpecialistController was not found " +
                "in the parent hierarchy.",
                this);
        }
    }

    public void Anim_PlayFootstep()
    {
        specialistController?.Anim_PlayFootstep();
    }
    public void Anim_PlayCrawlStep()
    {
        specialistController?.Anim_PlayCrawlStep();
    }
    private void ResolveController()
    {
        if (specialistController != null)
        {
            return;
        }

        specialistController =
            GetComponentInParent<RescueSpecialistController>();
    }
}

//----- RescueSpecialistAnimationEventRelay.cs END -----