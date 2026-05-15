using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Health")]
    public Slider healthBar;

    [Header("Wave Info")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesRemainingText;

    [Header("Crosshair")]
    public Image crosshair;

    [Header("Ammo")]
    public TextMeshProUGUI ammoText;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealth;
        WaveManager.OnWaveStarted += UpdateWaveText;
        WaveManager.OnEnemyCountChanged += UpdateEnemyCount;
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        Weapon.OnAmmoChanged += UpdateAmmo;
        Weapon.OnReloadStarted += ShowReloading;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealth;
        WaveManager.OnWaveStarted -= UpdateWaveText;
        WaveManager.OnEnemyCountChanged -= UpdateEnemyCount;
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        Weapon.OnAmmoChanged -= UpdateAmmo;
        Weapon.OnReloadStarted -= ShowReloading;
    }

    private void Awake()
    {
        if (healthBar == null)
        {
            Transform hp = transform.Find("HP");
            if (hp != null) healthBar = hp.GetComponent<Slider>();
        }
        if (waveText == null)
        {
            Transform wt = transform.Find("WaveCount");
            if (wt != null) waveText = wt.GetComponent<TextMeshProUGUI>();
        }
        if (enemiesRemainingText == null)
        {
            Transform et = transform.Find("EnemyCount");
            if (et != null) enemiesRemainingText = et.GetComponent<TextMeshProUGUI>();
        }
        if (crosshair == null)
        {
            Transform ch = transform.Find("Crosshair");
            if (ch != null) crosshair = ch.GetComponent<Image>();
        }
        if (ammoText == null)
        {
            Transform at = transform.Find("AmmoCount");
            if (at != null) ammoText = at.GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        if (waveText != null)
            waveText.text = "Wave 1/3";
        if (enemiesRemainingText != null)
            enemiesRemainingText.text = "Enemies: 0";
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
    }

    private void UpdateWaveText(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {wave}/3";
        }
    }

    private void UpdateEnemyCount(int count)
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = $"Enemies: {count}";
        }
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (crosshair != null)
        {
            crosshair.enabled = state == GameManager.GameState.Playing;
        }
    }

    private void UpdateAmmo(int current, int magazineSize)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{current}/{magazineSize}";
        }
    }

    private void ShowReloading()
    {
        if (ammoText != null)
        {
            ammoText.text = "Reloading...";
        }
    }
}
