using System;
using UnityEngine;

public class FirefighterController : MonoBehaviour {

    [Header("Components")]
    //[SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GroundCheck ground;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;

    [Header("Ladder Settings")]
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private float placementDistance = 1.5f;

    private GameObject ladder;
    private Vector2 currentMoveInput;

    private bool isFacingRight = true;

    public enum FirefighterState { Idle, Walking, Climbing, Attacking, Stunned }
    private FirefighterState currentState = FirefighterState.Idle;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        //anim
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveDirection) {

        currentMoveInput = moveDirection;

        rb.MovePosition(rb.position + (currentMoveInput * walkSpeed) * Time.fixedDeltaTime);
    }

    internal void UseAxe() {
        throw new NotImplementedException();
    }

    internal void UseLadder() {
        throw new NotImplementedException();
    }
}