using Managment;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        Time.timeScale = state == GameManager.GameState.Gameplay ? 1f : 0f;
    }
}