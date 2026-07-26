//----- CharacterCameraFollow2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class CharacterCameraFollow2D : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 1f, -10f);

    [SerializeField, Min(0f)]
    private float smoothTime = 0.2f;

    [Header("Switching")]
    [Tooltip(
        "When enabled, the camera immediately moves to the newly " +
        "selected character instead of traveling across the level.")]
    [SerializeField] private bool snapWhenTargetChanges = true;

    private Vector3 followVelocity;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition =
            target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogWarning(
                $"{nameof(CharacterCameraFollow2D)} received " +
                "a missing target.",
                this);

            return;
        }

        target = newTarget;
        followVelocity = Vector3.zero;

        if (snapWhenTargetChanges)
        {
            transform.position =
                target.position + offset;
        }
    }
}

//----- CharacterCameraFollow2D.cs END -----