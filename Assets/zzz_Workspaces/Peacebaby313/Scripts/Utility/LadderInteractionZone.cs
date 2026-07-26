using UnityEngine;

public class LadderInteractionZone : MonoBehaviour
{
    [SerializeField] private CarryableLadder ladder;
    [SerializeField] private LadderZoneType zoneType;

    public enum LadderZoneType
    {
        Pickup,
        Climbing
    }

    private void Awake()
    {
        if (ladder == null)
        {
            ladder = GetComponentInParent<CarryableLadder>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FirefighterController firefighter =
            other.GetComponentInParent<FirefighterController>();

        if (firefighter == null || ladder == null)
        {
            return;
        }

        if (zoneType == LadderZoneType.Pickup)
        {
            firefighter.EnterLadderPickupZone(ladder);
        }
        else
        {
            firefighter.EnterLadderClimbingZone(ladder);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FirefighterController firefighter =
            other.GetComponentInParent<FirefighterController>();

        if (firefighter == null || ladder == null)
        {
            return;
        }

        if (zoneType == LadderZoneType.Pickup)
        {
            firefighter.ExitLadderPickupZone(ladder);
        }
        else
        {
            firefighter.ExitLadderClimbingZone(ladder);
        }
    }
}