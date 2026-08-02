//----- RescueTentWinTrigger.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class RescueTentWinTrigger : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    private LevelLoopController levelLoopController;

    [Header("Survivor Detection")]
    [SerializeField]
    private string survivorTag = "Survivor";

    private bool hasTriggered;

    private void Awake()
    {
        if (levelLoopController == null)
        {
            levelLoopController =
                FindFirstObjectByType<LevelLoopController>();
        }

        Collider2D triggerCollider =
            GetComponent<Collider2D>();

        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered ||
            levelLoopController == null ||
            levelLoopController.LevelEnded)
        {
            return;
        }

        GameObject detectedObject =
            GetDetectionObject(other);

        if (detectedObject == null ||
            !detectedObject.CompareTag(survivorTag))
        {
            return;
        }

        hasTriggered = true;

        Debug.Log(
            $"Survivor '{detectedObject.name}' reached the tent.",
            this);

        levelLoopController.TriggerWin();
    }

    private GameObject GetDetectionObject(
        Collider2D other)
    {
        if (other.CompareTag(survivorTag))
        {
            return other.gameObject;
        }

        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.CompareTag(survivorTag))
        {
            return other.attachedRigidbody.gameObject;
        }

        Transform currentTransform = other.transform;

        while (currentTransform != null)
        {
            if (currentTransform.CompareTag(survivorTag))
            {
                return currentTransform.gameObject;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
    }

    private void OnValidate()
    {
        Collider2D triggerCollider =
            GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }
}

//----- RescueTentWinTrigger.cs END -----