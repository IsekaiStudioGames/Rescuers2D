using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour {

    private Rigidbody2D rb;

    [SerializeField] private float knockBackTime = 0.2f;

    public bool KnockBackedActive { get; private set; }

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void GetKnockedBack(Transform damageSource, float knockbackThrust) {

        KnockBackedActive = true;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockbackThrust * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(COKnockbackRoutine());
    }

    private IEnumerator COKnockbackRoutine() {
        
        yield return new WaitForSeconds(knockBackTime);

        rb.linearVelocity = Vector2.zero;
        KnockBackedActive = false;
    }
}