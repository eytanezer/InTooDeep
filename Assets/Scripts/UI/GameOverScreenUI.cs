using Managment;
using TMPro;
using UnityEngine;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

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
        gameObject.SetActive(state == GameManager.GameState.GameOver);

        if (state == GameManager.GameState.GameOver)
        {
            UpdateResultText();
        }
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