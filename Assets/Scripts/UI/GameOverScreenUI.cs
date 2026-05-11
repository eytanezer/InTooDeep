using System.Collections;
using Managment;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;

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
        bool shouldShow = state == GameManager.GameState.GameOver;
        bool isGameplay = state == GameManager.GameState.Gameplay;

        gameOverPanel.SetActive(shouldShow);

        if (shouldShow)
        {
            UpdateResultText();
            StartCoroutine(SelectQuitButtonNextFrame());
        }
    }

    private IEnumerator SelectQuitButtonNextFrame()
    {
        yield return null;

        // Navigation quitNav = new Navigation
        // {
        //     mode = Navigation.Mode.Explicit
        // };
        // quitButton.navigation = quitNav;
        
        SetVerticalNavigation(restartButton, quitButton);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        restartButton.Select();
    }
    
    private void SetVerticalNavigation(Button topButton, Button bottomButton)
    {
        Navigation topNav = new Navigation();
        topNav.mode = Navigation.Mode.Explicit;
        topNav.selectOnDown = bottomButton;
        topButton.navigation = topNav;

        Navigation bottomNav = new Navigation();
        bottomNav.mode = Navigation.Mode.Explicit;
        bottomNav.selectOnUp = topButton;
        bottomButton.navigation = bottomNav;
    }

    private void UpdateResultText()
    {
        if (resultText == null)
        {
            return;
        }

        resultText.text =
            GameManager.Instance.CurrentResult == GameManager.GameResult.Win
                ? "winner"
                : "Game Over";
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }

    public void OnRestartClicked()
    {
        EventManager.RaiseResetGame();
    }
}