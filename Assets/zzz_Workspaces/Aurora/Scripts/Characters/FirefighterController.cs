using System;
using UnityEngine;

public class FirefighterController : MonoBehaviour {

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveDirection) {

        rb.MovePosition(rb.position + (moveDirection * moveSpeed) * Time.fixedDeltaTime);
    }

    internal void UseAxe() {
        throw new NotImplementedException();
    }

    internal void UseLadder() {
        throw new NotImplementedException();
    }
}