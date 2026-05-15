using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Enemy Prefabs")]
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float delayBetweenWaves = 3f;
    public float spawnInterval = 0.5f;

    public int CurrentWave { get; private set; }
    public int TotalWaves => waveDefinitions.Length;
    public int EnemiesAlive { get; private set; }
    
    public static event Action<int> OnWaveStarted; // wave number
    public static event Action OnAllWavesCleared;
    public static event Action<int> OnEnemyCountChanged;
    
    private WaveDefinition[] waveDefinitions = new WaveDefinition[]
    {
        new WaveDefinition(5, 0),
        new WaveDefinition(5, 3),
        new WaveDefinition(8, 5)
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EnemyBase.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        CurrentWave = 0;
        StartCoroutine(StartNextWave());
    }

    private void HandleEnemyDied()
    {
        EnemiesAlive--;
        OnEnemyCountChanged?.Invoke(EnemiesAlive);

        if (EnemiesAlive <= 0)
        {
            if (CurrentWave >= TotalWaves)
            {
                OnAllWavesCleared?.Invoke();
            }
            else
            {
                // Start next wave after a short delay
                StartCoroutine(StartNextWave());
            }
        }
    }
    
    private IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(delayBetweenWaves);

        CurrentWave++;
        WaveDefinition wave = waveDefinitions[CurrentWave - 1];
        EnemiesAlive = wave.meleeCount + wave.rangedCount;

        OnWaveStarted?.Invoke(CurrentWave);
        OnEnemyCountChanged?.Invoke(EnemiesAlive);

        
        yield return StartCoroutine(SpawnEnemies(meleeEnemyPrefab, wave.meleeCount));
        yield return StartCoroutine(SpawnEnemies(rangedEnemyPrefab, wave.rangedCount));
    }
    
    // Spawns enemies with a small random offset to prevent stacking
    private IEnumerator SpawnEnemies(GameObject prefab, int count)
    {
        if (prefab == null) yield break;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
            Instantiate(prefab, spawnPoint.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Simple struct to define the number of melee and ranged enemies for each wave
    [System.Serializable]
    private struct WaveDefinition
    {
        public int meleeCount;
        public int rangedCount;

        public WaveDefinition(int melee, int ranged)
        {
            meleeCount = melee;
            rangedCount = ranged;
        }
    }
}
