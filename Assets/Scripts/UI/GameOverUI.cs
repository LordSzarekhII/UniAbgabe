using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public Button restartButton;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        if (panel == null)
            panel = transform.Find("GameOver")?.gameObject;
        
        if (restartButton == null)
            restartButton = GetComponentInChildren<Button>(true);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        
        if (panel != null)
            panel.SetActive(false);
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (panel != null)
        {
            panel.SetActive(state == GameManager.GameState.GameOver);
        }
    }

    public void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
