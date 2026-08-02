//----- FireFighterAnimationEventRelay.cs START -----

using System;
using UnityEngine;
[DisallowMultipleComponent] //Why do i need this?
public class FirefighterAnimationEventRelay : MonoBehaviour
{
    [SerializeField]
    private FirefighterController firefighterController;

    private void Reset()
    {
        ResolveController();
    }



    private void Awake()
    {
        ResolveController();

        if (firefighterController == null)
        {
            Debug.LogError(
                "[FIREFIGHTER ANIMATION] FirefighterController " +
                "was not found in the parent hierarchy.",
                this);
        }
    }

    public void Anim_PlayAxeSwing()
    {
        firefighterController?.Anim_PlayAxeSwing();
    }

    public void Anim_EnableAxeDamage()
    {
        firefighterController?.Anim_EnableAxeDamage();
    }

    public void Anim_DisableAxeDamage()
    {
        firefighterController?.Anim_DisableAxeDamage();
    }

    public void Anim_AxeFinished()
    {
        firefighterController?.Anim_AxeFinished();
    }

    public void Anim_PlayFootstep()
    {
        firefighterController?.Anim_PlayFootstep();
    }

    public void Anim_PlayClimbStep()
    {
        firefighterController.Anim_PlayClimbStep();
    }


    private void ResolveController()
    {
        if (firefighterController != null)
            return;

        firefighterController =
            GetComponentInParent<FirefighterController>();
    }
}

// ----- FireFighterAnimationEventRelay.cs END -----