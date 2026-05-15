using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    public float preferredDistance = 15f;
    public float fireInterval = 2f;
    public float moveSpeed = 3.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private float lastFireTime;

    protected override void Awake()
    {
        base.Awake();
        agent.speed = moveSpeed;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > preferredDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        if (distanceToPlayer <= preferredDistance + 5f && Time.time >= lastFireTime + fireInterval)
        {
            Fire();
        }
    }

    private void Fire()
    {
        lastFireTime = Time.time;

        
        Vector3 direction = (player.position + Vector3.up * 0.5f - firePoint.position).normalized;
        // Instantiate the projectile and set its direction
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        EnemyProjectile ep = projectile.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.SetDirection(direction);
        }
    }
}
