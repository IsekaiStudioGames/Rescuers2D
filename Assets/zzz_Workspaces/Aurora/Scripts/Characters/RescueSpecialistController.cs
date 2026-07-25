using System;
using UnityEngine;

public class RescueSpecialistController : MonoBehaviour {

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Move(Vector2 moveDirection) {

        rb.MovePosition(rb.position + (moveDirection * moveSpeed) * Time.fixedDeltaTime);
    }
    internal void Crawl() { throw new NotImplementedException(); }
    internal void Jump() {
        throw new NotImplementedException();
    }
    internal void Swim() { throw new NotImplementedException(); }
}