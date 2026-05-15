using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;

    public int CurrentHP { get; private set; }

    public static event Action OnEnemyDied;

    private Renderer enemyRenderer;
    private Color originalColor;

    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        CurrentHP = maxHP;
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHP <= 0) return;

        CurrentHP -= damage;
        StartCoroutine(gotHit());

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator gotHit()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = originalColor;
        }
    }

    protected virtual void Die()
    {
        OnEnemyDied?.Invoke();
        Destroy(gameObject);
    }
}
