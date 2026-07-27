using UnityEngine;
using System.Collections;
using UnityEditor.U2D.Aseprite;


public class DoggoScript : MonoBehaviour
{
    bool isPartolling = true, knockedOut, iswalking, iswaiting;
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningStartSpreed;
    [SerializeField] private float runningMulty;
    [SerializeField] float runningMaxSpeed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LayerMask player;
    [SerializeField] private float seeDis;
    private GameObject target;

    [Header("Aurora's~ Script :3")]
    [SerializeField] private GroundCheck groundCheck; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        walkingSpeed += runningMulty * Time.fixedDeltaTime;
        walkingSpeed = Mathf.Min(walkingSpeed, runningMaxSpeed);

        Vector2 direction = (target.transform.position - transform.position).normalized;

        rb.linearVelocity = direction * walkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        if (isPartolling)
        {
            if (iswalking)
            {
                if (transform.localScale.x == 1)
                {
                    rb.linearVelocity = (Vector2)(-transform.right * walkingSpeed);
                }
                else
                {
                    rb.linearVelocity = (Vector2)(transform.right * walkingSpeed);
                }
            }
            else if (rb.linearVelocity != Vector2.zero && !iswalking)
            {
                rb.linearVelocity = Vector2.zero;
            }
            if (!iswaiting) StartCoroutine("WaitBeforeMoving");
        }
        else
        {

        }
        if (knockedOut)
        {
            if (!IsGrounded)
            {

            }
            else
            {

            }
        }
    }
    public bool IsGrounded =>
        groundCheck != null && groundCheck.IsGrounded;

    IEnumerator WaitBeforeMoving()
    {
        iswaiting = true;
        yield return new WaitForSeconds(3f);
        iswalking = true;
        StartCoroutine("WalkingTimer");
    }
    IEnumerator WalkingTimer()
    {
        yield return new WaitForSeconds(2.5f);
        iswalking = false;
        iswaiting = false;
        //if (transform.localScale.x == 1) transform..x = -1;
        if (transform.localScale.x == 1) transform.localScale = new Vector3(-1,1,1);
        else transform.localScale = new Vector3(1, 1, 1);

    }
    private void FindPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, seeDis, player);
        if (hit != null && hit.tag == "Player")
        {
            target = hit.gameObject;
            isPartolling = false;
            StopAllCoroutines();
            iswalking = false;
            iswaiting = false;
        }
    }
}
