using Managment;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameManager.GameState state)
    {
        startPanel.SetActive(state == GameManager.GameState.Title);
        instructionsPanel.SetActive(state == GameManager.GameState.Instructions);
        gameplayPanel.SetActive(state == GameManager.GameState.Gameplay);
        pausePanel.SetActive(state == GameManager.GameState.Paused);
        gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
    }

    // ===== BUTTONS =====

    public void OnStartClicked()
    {
        EventManager.RaiseStartGame();
    }

    public void OnPauseClicked()
    {
        EventManager.RaisePauseGame();
    }

    public void OnResumeClicked()
    {
        EventManager.RaiseResumeGame();
    }

    public void OnRestartClicked()
    {
        EventManager.RaiseResetGame();
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }
}