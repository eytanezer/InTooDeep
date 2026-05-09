using System.Collections;
using Managment;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject firstSelectedButton;

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
            StartCoroutine(SelectButtonNextFrame());
        }
    }

    private IEnumerator SelectButtonNextFrame()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        Debug.Log("Selected button: " + EventSystem.current.currentSelectedGameObject);
    }

    private void UpdateResultText()
    {
        if (resultText == null)
        {
            return;
        }

        resultText.text =
            GameManager.Instance.CurrentResult == GameManager.GameResult.Win
                ? "You Win!"
                : "Game Over";
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }
}