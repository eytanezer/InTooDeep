using System.Collections;
using Managment;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
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
        bool shouldShow = state == GameManager.GameState.Title;
        gameObject.SetActive(shouldShow);

        if (shouldShow)
        {
            StartCoroutine(SelectButtonNextFrame());
        }
    }

    private IEnumerator SelectButtonNextFrame()
    {
        yield return null;

        SetVerticalNavigation(startButton, quitButton);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        startButton.Select();
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

    public void OnStartClicked()
    {
        EventManager.RaiseStartGame();
    }

    public void OnQuitClicked()
    {
        EventManager.RaiseQuitGame();
    }
}