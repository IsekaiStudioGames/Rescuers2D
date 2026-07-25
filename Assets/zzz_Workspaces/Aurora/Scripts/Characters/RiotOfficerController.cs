using System;
using UnityEngine;

public class RiotOfficerController : MonoBehaviour {
    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Move(Vector2 moveDirection) {

        rb.MovePosition(rb.position + (moveDirection * moveSpeed) * Time.fixedDeltaTime);

        // Face left/right
        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        //walk anim
        if (moveDirection != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);

        }
        else
        {
            animator.SetBool("IsMoving", false);
        }    
    }

    internal void ToggleShield() {
        throw new NotImplementedException();
    }
}