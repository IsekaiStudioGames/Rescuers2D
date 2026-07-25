using System;
using UnityEngine;

public class FirefighterController : MonoBehaviour {

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

    private GameObject ladder;
    private Vector2 currentMoveInput;

    private float gravityScale = 3f;

    public float SlopeAngle => groundCheck != null ? groundCheck.SlopeAngle : 0f;
    public bool IsGrounded => groundCheck != null && groundCheck.IsGrounded;
    private bool isFacingRight = true;

    public enum FirefighterState { Idle, Walking, Climbing, Attacking, Stunned }
    private FirefighterState currentState = FirefighterState.Idle;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
    }
    private void FixedUpdate() {
        ApplyGravity();
    }
    public void Move(Vector2 moveDirection) {

        currentMoveInput = moveDirection;

        if (currentState == FirefighterState.Attacking || currentState == FirefighterState.Stunned) {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (currentState == FirefighterState.Climbing) {
            HandleClimbing();
        } else {
            HandleGroundMovement();
        }

        rb.MovePosition(rb.position + (currentMoveInput * walkSpeed) * Time.fixedDeltaTime);
    }

    private void HandleGroundMovement() {

        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentMoveInput.x * walkSpeed;
        if (groundCheck.IsGrounded && rb.linearVelocity.y > 0f) {
            velocity.y = 0f;
        }
        rb.linearVelocity = velocity;
    }
    private void ApplyGravity() {

        bool grounded = groundCheck.IsGrounded;

        if (!grounded && currentState != FirefighterState.Climbing) {
            Vector2 velocity = rb.linearVelocity;
            velocity.y += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
    }

    private void HandleClimbing() {
        throw new NotImplementedException();
    }

    internal void UseAxe() {
        throw new NotImplementedException();
    }

    internal void UseLadder() {
        throw new NotImplementedException();
    }
}