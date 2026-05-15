using System.Collections;
using Managment;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resumeButton;

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
        bool isGameplay = state == GameManager.GameState.Gameplay;

        pausePanel.SetActive(isPaused);
        
        if(pauseButton) pauseButton.gameObject.SetActive(isGameplay);

        if (isPaused)
        {
            StartCoroutine(SelectResumeNextFrame());
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    public void OnPause()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Gameplay)
        {
            EventManager.RaisePauseGame();
        }
        else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
        {
            EventManager.RaiseResumeGame();
        }
    }

    private IEnumerator SelectResumeNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        
        SetVerticalNavigation(resumeButton, menuButton);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    private void SetVerticalNavigation(Button topButton, Button bottomButton)
    {
        Navigation topNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = bottomButton
        };
        topButton.navigation = topNav;

        Navigation bottomNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnUp = topButton
        };
        bottomButton.navigation = bottomNav;
    }

    public void OnPauseClicked()
    {
        Debug.Log("PAUSE CLICKED FROM UI BUTTON");
        EventManager.RaisePauseGame();
    }

    public void OnResumeClicked()
    {
        EventManager.RaiseResumeGame();
    }

    public void OnMenuClicked()
    {
        EventManager.RaiseReturnToMenu();
    }
}