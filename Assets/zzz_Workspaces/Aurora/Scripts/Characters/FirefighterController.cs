using System;
using UnityEngine;

public class FirefighterController : MonoBehaviour {

    private ScalingLadder currentLadder;

    [Header("Components")]
    //[SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GroundCheck groundCheck;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;

    [Header("Ladder Settings")]
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private float placementDistance = 1.5f;

    [Header("Ladder Pickup")]
    [SerializeField] private float pickupDistance = 2f;
    [SerializeField] private LayerMask ladderLayer;

    private GameObject ladder;
    private Vector2 currentMoveInput;

    private float baseGravityScale = 3f;

    public float SlopeAngle => groundCheck != null ? groundCheck.SlopeAngle : 0f;
    public bool IsGrounded => groundCheck != null && groundCheck.IsGrounded;
    private bool isFacingRight = true;
    private bool isAdjustLadder = false;

    public enum FirefighterState { Idle, Walking, Climbing, Attacking, Stunned }
    [SerializeField] private FirefighterState currentState = FirefighterState.Idle;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();

        rb.gravityScale = baseGravityScale;
    }
    private void FixedUpdate() {
        ApplyGravity();

        if (currentState == FirefighterState.Attacking || currentState == FirefighterState.Stunned) {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (currentState == FirefighterState.Climbing) {
            HandleClimbing();
        } else {
            HandleGroundMovement();
        }
    }
    public void Move(Vector2 moveDirection) {

        currentMoveInput = moveDirection;

        if (isAdjustLadder && currentLadder != null) {
            rb.linearVelocity = Vector2.zero;
            currentLadder.UpdateLadderSize(currentMoveInput.y);
            return;
        }
    }

    private void HandleGroundMovement() {

        rb.gravityScale = baseGravityScale;

        Vector2 velocity = new(currentMoveInput.x * walkSpeed, rb.linearVelocity.y);

        if (groundCheck.IsGrounded && velocity.y > 0f) {
            velocity.y = 0f;
        }
        rb.linearVelocity = velocity;

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f) {
            ChangeState(FirefighterState.Walking);
            FlipSprite(currentMoveInput.x);
        } else {
            ChangeState(FirefighterState.Idle);
        }
    }
    private void ApplyGravity() {

        bool grounded = groundCheck.IsGrounded;

        if (!grounded && currentState != FirefighterState.Climbing) {
            Vector2 velocity = rb.linearVelocity;
            velocity.y += Physics2D.gravity.y * baseGravityScale * Time.fixedDeltaTime;
        }
    }

    public void HandleClimbing() {

        rb.gravityScale = 0f;
        rb.linearVelocity = new(currentMoveInput.x * walkSpeed / 2f, currentMoveInput.y * climbSpeed);
    }

    public void UseAxe() {

        if (currentState == FirefighterState.Climbing || currentState == FirefighterState.Stunned) return;

        ChangeState(FirefighterState.Attacking);
        //anim.SetTrigger("AxeAttack");

        Vector2 checkPosition = (Vector2)transform.position + (isFacingRight ? Vector2.right : Vector2.left) * 1f;
        Collider2D[] hit = Physics2D.OverlapCircleAll(checkPosition, 0.8f);

        foreach (var items in hit) {

            if (items.CompareTag("Debris")) Destroy(items.gameObject); // add health system?
        }
    }
    public void DeployLadder() {
        
        if (ladder != null) {
            TriggerStun();
            return;
        }

        float spawnDirection = isFacingRight ? placementDistance : -placementDistance;
        float heightOffset = 0.5f;
        Vector3 spawnPosition = new(transform.position.x + spawnDirection, transform.position.y - heightOffset, transform.position.z);

        ladder = Instantiate(ladderPrefab, spawnPosition, Quaternion.identity);

        if (ladder.TryGetComponent<ScalingLadder>(out var scalingLadder)) {

            currentLadder = scalingLadder;
            isAdjustLadder = true;
        }
    }
    public void StopDeployingLadder() {

        isAdjustLadder = false;
        currentLadder = null;

        ResetToIdle();
    }
    public bool IsLadderNearbyToRetrieve() {

        if (ladder == null) return false;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, pickupDistance, ladderLayer);
        return hit != null && hit.gameObject == ladder;
    }
    public void TryInteractLadder() {

        if (ladder != null) {

            Collider2D hit = Physics2D.OverlapCircle(transform.position, pickupDistance, ladderLayer);

            if (hit != null && hit.gameObject == ladder) {
                RetrieveLadder();
                return;
            }
        }
    }
    public void RetrieveLadder() {
        
        if (ladder != null) {
            Destroy(ladder);
            ladder = null;
            currentLadder = null;
        }
    }
    private void TriggerStun() {
        ChangeState(FirefighterState.Stunned);
        //anim.SetTrigger
    }
    public void SetClimbingState(bool climbing) {

        if (climbing) {
            ChangeState(FirefighterState.Climbing);
        } else {
            //set anim speed
            ChangeState(FirefighterState.Idle);
        }
    }
    private void ChangeState(FirefighterState newState) {

        if (currentState == newState) return;
        currentState = newState;

        //add anims
    }
    private void FlipSprite(float value) {

        if ((value > 0 && !isFacingRight) || (value < 0 && isFacingRight)) {
            isFacingRight = !isFacingRight;
            Vector3 scaler = transform.localScale;
            scaler.x = isFacingRight ? 1f : -1f;
            transform.localScale = scaler;
        }
    }
    public void ResetToIdle() => ChangeState(FirefighterState.Idle);
    private void OnDrawGizmosSelected() {
        
        Gizmos.color = Color.red;
        Vector3 checkPos = transform.position + (isFacingRight ? Vector3.right : Vector3.left) * 1f;
        Gizmos.DrawWireSphere(checkPos, 0.8f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}