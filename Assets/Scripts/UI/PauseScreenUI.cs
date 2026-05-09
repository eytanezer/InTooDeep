using Managment;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject firstPausePanelButton;

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
        bool isPaused = state == GameManager.GameState.Paused;
        pausePanel.SetActive(isPaused);

        EventSystem.current.SetSelectedGameObject(null);

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(firstPausePanelButton);
        }
        else if (state == GameManager.GameState.Gameplay)
        {
            EventSystem.current.SetSelectedGameObject(pauseButton);
        }
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