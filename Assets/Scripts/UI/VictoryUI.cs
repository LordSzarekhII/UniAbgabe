using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
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
            panel = transform.Find("Victory")?.gameObject;

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
            panel.SetActive(state == GameManager.GameState.Victory);
        }
    }

    public void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
