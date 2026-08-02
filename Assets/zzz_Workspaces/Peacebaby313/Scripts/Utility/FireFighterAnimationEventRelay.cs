//----- FireFighterAnimationEventRelay.cs START -----

using UnityEngine;

public class FirefighterAnimationEventRelay : MonoBehaviour
{
    [SerializeField]
    private FirefighterController firefighterController;

    private void Awake()
    {
        if (firefighterController == null)
        {
            firefighterController =
                GetComponentInParent<FirefighterController>();
        }
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
}

// ----- FireFighterAnimationEventRelay.cs END -----