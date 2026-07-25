using System;
using UnityEngine;

public class ScalingLadder : MonoBehaviour {

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D trigger;

    [Header("Scaling Variables")]
    [SerializeField, Range(1f, 10f)] private float scaleSpeed = 5f;
    [SerializeField] private float minLadderHeight = 0.5f;
    [SerializeField, Range(5f, 50f)] private float maxLadderHeight = 25f;
    [SerializeField] private LayerMask platformLayer;

    private float currentHeight = 0.5f;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<BoxCollider2D>();
        ResetLadderSize();
    }
    public void UpdateLadderSize(float verticalInput) {

        if (Mathf.Abs(verticalInput) < 0.1f) return;

        if (verticalInput > 0f) {

            Vector2 origin = (Vector2)transform.position + new Vector2(0f, currentHeight);
            float remainingSpace = maxLadderHeight - currentHeight;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, remainingSpace, platformLayer);

            if (hit.collider != null) {

                float distanceToCeiling = hit.distance;

                if (distanceToCeiling > 0.05f) {
                    currentHeight += distanceToCeiling;
                    UpdateLadderDimensions();
                }
                return;
            }
            currentHeight += scaleSpeed * Time.deltaTime;
        }
        else if (verticalInput < 0f) {

            currentHeight += verticalInput * scaleSpeed * Time.deltaTime;
        }
        currentHeight = Mathf.Clamp(currentHeight, minLadderHeight, maxLadderHeight);
        UpdateLadderDimensions();
    }

    private void UpdateLadderDimensions() {
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, currentHeight);
        trigger.size = new Vector2(trigger.size.x, currentHeight);
        trigger.offset = new Vector2(0f, currentHeight / 2f);
    }
    public void ResetLadderSize() {
        currentHeight = minLadderHeight;
        UpdateLadderDimensions();
    }
}