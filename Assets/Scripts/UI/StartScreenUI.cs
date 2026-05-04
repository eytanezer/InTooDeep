using Managment;
using UnityEngine;

public class StartScreenUI : MonoBehaviour
{
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
        gameObject.SetActive(state == GameManager.GameState.Title);
    }

    public void OnStartClicked()
    {
        EventManager.RaiseStartGame();
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }
}