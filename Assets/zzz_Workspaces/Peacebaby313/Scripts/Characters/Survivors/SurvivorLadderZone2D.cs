//----- SurvivorLadderZone2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class SurvivorLadderZone2D : MonoBehaviour
{
    [SerializeField]
    private CarryableLadder ladder;

    [Tooltip(
        "Optional centerline used to align the survivor " +
        "while climbing.")]
    [SerializeField]
    private Transform ladderCenter;

    private void Awake()
    {
        if (ladder == null)
        {
            ladder =
                GetComponentInParent<CarryableLadder>();
        }

        if (ladderCenter == null)
        {
            ladderCenter = transform;
        }
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        SurvivorFollower2D survivor =
            other.GetComponentInParent<
                SurvivorFollower2D>();

        if (survivor == null)
        {
            return;
        }

        survivor.EnterLadderZone(
            ladder,
            ladderCenter);
    }

    private void OnTriggerExit2D(
        Collider2D other)
    {
        SurvivorFollower2D survivor =
            other.GetComponentInParent<
                SurvivorFollower2D>();

        if (survivor == null)
        {
            return;
        }

        survivor.ExitLadderZone(ladder);
    }

    private void OnValidate()
    {
        Collider2D zoneCollider =
            GetComponent<Collider2D>();

        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }

        if (ladder == null)
        {
            ladder =
                GetComponentInParent<CarryableLadder>();
        }
    }
}

//----- SurvivorLadderZone2D.cs END -----