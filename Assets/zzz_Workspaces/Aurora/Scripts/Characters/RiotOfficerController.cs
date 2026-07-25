using System;
using UnityEngine;

public class RiotOfficerController : MonoBehaviour {
    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveDirection) {

        rb.MovePosition(rb.position + (moveDirection * moveSpeed) * Time.fixedDeltaTime);
    }

    internal void ToggleShield() {
        throw new NotImplementedException();
    }
}