//----- CloudFloatUtility.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class CloudFloatUtility : MonoBehaviour
{
    [Header("Float Distance")]
    [SerializeField, Min(0f)]
    private float horizontalDistance = 0.5f;

    [SerializeField, Min(0f)]
    private float verticalDistance = 0.2f;

    [Header("Float Speed")]
    [SerializeField, Min(0f)]
    private float horizontalSpeed = 0.25f;

    [SerializeField, Min(0f)]
    private float verticalSpeed = 0.4f;

    [Header("Variation")]
    [Tooltip("Prevents multiple clouds from moving in perfect sync.")]
    [SerializeField]
    private bool useRandomStartingPhase = true;

    private Vector3 startingLocalPosition;
    private float horizontalPhase;
    private float verticalPhase;

    private void Awake()
    {
        startingLocalPosition = transform.localPosition;

        if (useRandomStartingPhase)
        {
            horizontalPhase = Random.Range(0f, Mathf.PI * 2f);
            verticalPhase = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void Update()
    {
        float horizontalOffset =
            Mathf.Sin(
                Time.time * horizontalSpeed +
                horizontalPhase) *
            horizontalDistance;

        float verticalOffset =
            Mathf.Sin(
                Time.time * verticalSpeed +
                verticalPhase) *
            verticalDistance;

        transform.localPosition =
            startingLocalPosition +
            new Vector3(
                horizontalOffset,
                verticalOffset,
                0f);
    }

    private void OnDisable()
    {
        transform.localPosition =
            startingLocalPosition;
    }

    private void OnValidate()
    {
        horizontalDistance =
            Mathf.Max(0f, horizontalDistance);

        verticalDistance =
            Mathf.Max(0f, verticalDistance);

        horizontalSpeed =
            Mathf.Max(0f, horizontalSpeed);

        verticalSpeed =
            Mathf.Max(0f, verticalSpeed);
    }
}

//----- CloudFloatUtility.cs END -----