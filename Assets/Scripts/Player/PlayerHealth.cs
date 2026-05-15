using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int CurrentHP { get; private set; }

    public static event Action<int, int> OnHealthChanged; // current, max
    public static event Action OnPlayerDied;

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHP <= 0) return;

        CurrentHP -= damage;
        CurrentHP = Mathf.Max(CurrentHP, 0);
        OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnPlayerDied?.Invoke();
    }
}
