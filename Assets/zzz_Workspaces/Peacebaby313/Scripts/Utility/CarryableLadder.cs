//----- CarryableLadder.cs START -----

using System.Collections;
using UnityEngine;

public class CarryableLadder : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Tooltip("The ladder colliders used for physical collision.")]
    [SerializeField] private Collider2D[] physicalColliders;

    [Header("Attachment")]
    [Tooltip(
        "The point on the ladder that snaps to the " +
        "Firefighter's LadderHoldPoint."
    )]
    [SerializeField] private Transform firefighterAttachPoint;

    [Header("Extension")]
    [Tooltip(
        "The transform containing the second ladder SpriteRenderer. " +
        "This object slides upward when the ladder extends.")]
    [SerializeField] private Transform extensionSection;

    [Tooltip(
        "The extension section's local position while retracted.")]
    [SerializeField] private Vector3 retractedLocalPosition;

    [Tooltip(
        "The extension section's local position when fully extended.")]
    [SerializeField]
    private Vector3 extendedLocalPosition =
        new Vector3(0f, 3f, 0f);

    [SerializeField, Min(0.01f)]
    private float extensionDuration = 0.5f;

    [Tooltip(
        "Enable these objects only after the extension finishes. " +
        "Assign the upper climbing trigger and upper top trigger.")]
    [SerializeField] private GameObject[] extensionGameplayObjects;

    [Header("Runtime State")]
    [SerializeField]
    private LadderState currentState =
        LadderState.Placed;

    [SerializeField] private bool isExtended;
    [SerializeField] private bool isChangingExtension;

    private Transform originalParent;
    private RigidbodyType2D originalBodyType;
    private float originalGravityScale;
    private bool originalSimulated;
    private bool hasCachedPhysicsSettings;

    private Coroutine extensionRoutine;

    public enum LadderState
    {
        Placed,
        Carried
    }

    public LadderState CurrentState => currentState;

    public bool IsCarried =>
        currentState == LadderState.Carried;

    public bool IsExtended => isExtended;

    public bool IsChangingExtension =>
        isChangingExtension;

    public bool CanBePickedUp =>
        currentState == LadderState.Placed &&
        !isChangingExtension;

    public bool CanBeClimbed =>
        currentState == LadderState.Placed &&
        !isChangingExtension;

    public bool CanChangeExtension =>
        currentState == LadderState.Placed &&
        !isChangingExtension;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        originalParent = transform.parent;
        CachePhysicsSettings();

        InitializeExtension();
    }

    public void ToggleExtension()
    {
        if (!CanChangeExtension ||
            extensionSection == null)
        {
            return;
        }

        SetExtended(!isExtended);
    }

    public void SetExtended(bool shouldExtend)
    {
        if (!CanChangeExtension ||
            extensionSection == null ||
            shouldExtend == isExtended)
        {
            return;
        }

        if (extensionRoutine != null)
        {
            StopCoroutine(extensionRoutine);
        }

        extensionRoutine = StartCoroutine(
            ChangeExtensionRoutine(shouldExtend)
        );
    }

    private IEnumerator ChangeExtensionRoutine(
        bool shouldExtend)
    {
        isChangingExtension = true;

        // The upper climbing and top triggers should not be
        // usable while the ladder section is moving.
        SetExtensionGameplayObjectsEnabled(false);

        Vector3 startPosition =
            extensionSection.localPosition;

        Vector3 targetPosition =
            shouldExtend
                ? extendedLocalPosition
                : retractedLocalPosition;

        float elapsedTime = 0f;

        while (elapsedTime < extensionDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / extensionDuration
            );

            // Smooth the beginning and end of the movement.
            float smoothedProgress =
                Mathf.SmoothStep(0f, 1f, progress);

            extensionSection.localPosition =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    smoothedProgress
                );

            yield return null;
        }

        extensionSection.localPosition =
            targetPosition;

        isExtended = shouldExtend;
        isChangingExtension = false;
        extensionRoutine = null;

        SetExtensionGameplayObjectsEnabled(isExtended);
    }

    public void AttachTo(Transform firefighterHoldPoint)
    {
        if (firefighterHoldPoint == null ||
            firefighterAttachPoint == null ||
            IsCarried ||
            isChangingExtension)
        {
            return;
        }

        CachePhysicsSettings();

        currentState = LadderState.Carried;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        SetPhysicalCollidersEnabled(false);
        SetExtensionGameplayObjectsEnabled(false);

        transform.SetParent(
            firefighterHoldPoint,
            true
        );

        AlignAttachmentPoints(
            firefighterHoldPoint
        );
    }

    public void Detach()
    {
        if (!IsCarried)
        {
            return;
        }

        Vector3 worldPosition = transform.position;
        Quaternion worldRotation = transform.rotation;
        Vector3 worldScale = transform.lossyScale;

        transform.SetParent(originalParent, true);

        transform.position = worldPosition;
        transform.rotation = worldRotation;

        SetWorldScale(worldScale);

        currentState = LadderState.Placed;

        SetPhysicalCollidersEnabled(true);

        if (isExtended)
        {
            SetExtensionGameplayObjectsEnabled(true);
        }

        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            rb.gravityScale = originalGravityScale;
            rb.simulated = originalSimulated;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void InitializeExtension()
    {
        if (extensionSection == null)
        {
            SetExtensionGameplayObjectsEnabled(false);
            return;
        }

        extensionSection.localPosition =
            isExtended
                ? extendedLocalPosition
                : retractedLocalPosition;

        SetExtensionGameplayObjectsEnabled(
            isExtended && !IsCarried
        );
    }

    private void SetExtensionGameplayObjectsEnabled(
        bool enabled)
    {
        if (extensionGameplayObjects == null)
        {
            return;
        }

        foreach (GameObject gameplayObject
                 in extensionGameplayObjects)
        {
            if (gameplayObject != null)
            {
                gameplayObject.SetActive(enabled);
            }
        }
    }

    private void AlignAttachmentPoints(
        Transform firefighterHoldPoint)
    {
        Quaternion rotationDifference =
            firefighterHoldPoint.rotation *
            Quaternion.Inverse(
                firefighterAttachPoint.rotation
            );

        transform.rotation =
            rotationDifference *
            transform.rotation;

        Vector3 positionDifference =
            firefighterHoldPoint.position -
            firefighterAttachPoint.position;

        transform.position += positionDifference;
    }

    private void CachePhysicsSettings()
    {
        if (rb == null || hasCachedPhysicsSettings)
        {
            return;
        }

        originalBodyType = rb.bodyType;
        originalGravityScale = rb.gravityScale;
        originalSimulated = rb.simulated;
        hasCachedPhysicsSettings = true;
    }

    private void SetPhysicalCollidersEnabled(bool enabled)
    {
        if (physicalColliders == null)
        {
            return;
        }

        foreach (Collider2D physicalCollider
                 in physicalColliders)
        {
            if (physicalCollider != null)
            {
                physicalCollider.enabled = enabled;
            }
        }
    }

    private void SetWorldScale(Vector3 desiredWorldScale)
    {
        Transform parent = transform.parent;

        if (parent == null)
        {
            transform.localScale = desiredWorldScale;
            return;
        }

        Vector3 parentScale = parent.lossyScale;

        transform.localScale = new Vector3(
            SafeDivide(
                desiredWorldScale.x,
                parentScale.x
            ),
            SafeDivide(
                desiredWorldScale.y,
                parentScale.y
            ),
            SafeDivide(
                desiredWorldScale.z,
                parentScale.z
            )
        );
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f))
        {
            return value;
        }

        return value / divisor;
    }
}

//----- CarryableLadder.cs END -----