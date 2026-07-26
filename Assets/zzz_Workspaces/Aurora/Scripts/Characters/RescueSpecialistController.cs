using System;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class RescueSpecialistController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private CapsuleCollider2D bodyCollider;


    [Header("Ground Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crawlSpeed = 2.5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Swimming")]
    [SerializeField] private float swimSpeed = 4f;

    private Vector2 currentMoveInput;

    [Header("CrawlingCollider")]
    [SerializeField] private Vector2 crawlingColliderSize = new Vector2(0.75f, 0.65f);
    [SerializeField] private Vector2 crawlingColliderOffset = new Vector2(0f, -.05f);
    private float crawlToggleCooldown = 0.1f;
    private float nextCrawlToggleTime;

    [SerializeField] private Transform visuals;
    [SerializeField] private float crawlingVisualYOffset = 0.08f;

    private Vector3 standingVisualPosition;




    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    private bool isFacingRight = true;
    private bool isCrawling;
    private bool isSwimming;

    public enum SpecialistState
    {
        Idle,
        Walking,
        Jumping,
        Crawling,
        Swimming
    }

    [SerializeField]
    private SpecialistState currentState = SpecialistState.Idle;

    private void Awake()
    {
        if (visuals != null)
        {
            standingVisualPosition = visuals.localPosition;
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (groundCheck == null)
        {
            groundCheck = GetComponentInChildren<GroundCheck>();
        }
        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<CapsuleCollider2D>();
        }
        if (bodyCollider != null)
        {
            standingColliderSize = bodyCollider.size;
            standingColliderOffset = bodyCollider.offset;
        }
    }
    private void FixedUpdate()
    {
        if (isSwimming)
        {
            HandleSwimming();
        }
        else
        {
            HandleGroundMovement();
        }

        UpdateAnimator();
    }

    public void Move(Vector2 moveDirection)
    {
        currentMoveInput = moveDirection;

    }
    private void HandleGroundMovement()
    {
        float activeMoveSpeed = isCrawling
            ? crawlSpeed
            : moveSpeed;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentMoveInput.x * activeMoveSpeed;

        rb.linearVelocity = velocity;

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            FlipSprite(currentMoveInput.x);
        }

        if (isCrawling)
        {
            ChangeState(SpecialistState.Crawling);
            return;
        }

        if (!IsGrounded)
        {
            ChangeState(SpecialistState.Jumping);
            return;
        }

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            ChangeState(SpecialistState.Walking);
        }
        else
        {
            ChangeState(SpecialistState.Idle);
        }
    }
    private void HandleSwimming()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = currentMoveInput * swimSpeed;

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            FlipSprite(currentMoveInput.x);
        }

        ChangeState(SpecialistState.Swimming);
    }

    internal void Jump()
    {
        if (isSwimming)
        {
            return;
        }

        if (isCrawling)
        {
            return;
        }

        if (!IsGrounded)
        {
            return;
        }

        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;

        ChangeState(SpecialistState.Jumping);
        UpdateAnimator();
    }

    public void Crawl()
    {
        if (isSwimming || bodyCollider == null)
        {
            return;
        }

        if (Time.time < nextCrawlToggleTime)
        {
            return;
        }

        if (!isCrawling && !IsGrounded)
        {
            return;
        }

        nextCrawlToggleTime = Time.time + crawlToggleCooldown;
        isCrawling = !isCrawling;

        if (isCrawling)
        {
            float standingBottom =
                standingColliderOffset.y -
                standingColliderSize.y * 0.5f;

            float crawlOffsetY =
                standingBottom +
                crawlingColliderSize.y * 0.5f;

            bodyCollider.size = crawlingColliderSize;
            bodyCollider.offset = new Vector2(
                crawlingColliderOffset.x,
                crawlOffsetY
            );

            if (visuals != null)
            {
                visuals.localPosition =
                    standingVisualPosition +
                    Vector3.up * crawlingVisualYOffset;
            }

            ChangeState(SpecialistState.Crawling);
        }
        else
        {
            bodyCollider.size = standingColliderSize;
            bodyCollider.offset = standingColliderOffset;

            if (visuals != null)
            {
                visuals.localPosition = standingVisualPosition;
            }

            ChangeState(SpecialistState.Idle);
        }

        UpdateAnimator();
    }
    public void SetSwimmingState(bool swimming)
    {
        if (isSwimming == swimming)
        {
            return;
        }

        isSwimming = swimming;
        isCrawling = false;


        if (bodyCollider != null)
        {
            bodyCollider.size = standingColliderSize;
            bodyCollider.offset = standingColliderOffset;
        }

        rb.linearVelocity = Vector2.zero;

        if (isSwimming)
        {
            rb.gravityScale = 0f;
            ChangeState(SpecialistState.Swimming);
        }
        else
        {
            rb.gravityScale = 1f;
            ChangeState(SpecialistState.Idle);
        }

        UpdateAnimator();
    }
    public void Swim()
    {
        SetSwimmingState(!isSwimming);
    }
    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        bool isMoving =
            Mathf.Abs(currentMoveInput.x) > 0.1f;

        animator.SetBool(
            "IsMoving",
            isMoving && !isSwimming
        );

        animator.SetBool(
            "IsJumping",
            currentState == SpecialistState.Jumping
        );

        animator.SetBool(
            "IsCrawling",
            currentState == SpecialistState.Crawling
        );

        animator.SetBool(
            "IsSwimming",
            currentState == SpecialistState.Swimming
        );

        animator.SetFloat(
            "HorizontalInput",
            currentMoveInput.x
        );

        animator.SetFloat(
            "VerticalInput",
            currentMoveInput.y
        );
    }

    private void FlipSprite(float direction)
    {
        if ((direction > 0f && !isFacingRight) ||
            (direction < 0f && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            Vector3 scale = transform.localScale;
            scale.x =
                Mathf.Abs(scale.x) *
                (isFacingRight ? 1f : -1f);

            transform.localScale = scale;
        }
    }


    public bool IsGrounded =>
        groundCheck != null && groundCheck.IsGrounded;


    private void ChangeState(SpecialistState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }


}