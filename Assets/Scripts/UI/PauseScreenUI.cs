using Managment;
using UnityEngine;

public class PauseScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

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
        pausePanel.SetActive(state == GameManager.GameState.Paused);
    }

    public void OnPauseClicked()
    {
        EventManager.RaisePauseGame();
    }

    public void OnResumeClicked()
    {
        EventManager.RaiseResumeGame();
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }
}