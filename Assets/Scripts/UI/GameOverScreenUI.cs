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

        Navigation quitNav = new Navigation
        {
            mode = Navigation.Mode.Explicit
        };
        quitButton.navigation = quitNav;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(quitButton.gameObject);
        quitButton.Select();
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
}