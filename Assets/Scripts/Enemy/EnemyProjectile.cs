using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 8;
    public float lifetime = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool directionSet;

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
        directionSet = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (directionSet)
        {
            rb.linearVelocity = moveDirection * speed;
        }
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger && other.GetComponentInParent<EnemyBase>() == null)
        {
            Destroy(gameObject);
        }
    }
}
