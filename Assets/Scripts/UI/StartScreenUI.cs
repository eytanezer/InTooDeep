using System.Collections;
using Managment;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject controlsPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Controls Menu Buttons")]
    [SerializeField] private Button backButton;
    

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
        
        if (shouldShow)
        {
            startPanel.SetActive(true);
            controlsPanel.SetActive(false);
            StartCoroutine(SelectButtonNextFrame(startButton));
            SetUpMainMenuNavigation();
        }
        else
        {
            startPanel.SetActive(false);
            controlsPanel.SetActive(false);
        }
    }

    private IEnumerator SelectButtonNextFrame(Button buttonToSelect)
    {
        yield return new WaitForSecondsRealtime(0.1f);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
    }

    private void SetVerticalNavigation(Button topButton, Button bottomButton)
    {
        Navigation topNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnDown = bottomButton};
        topButton.navigation = topNav;

        Navigation bottomNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnUp = topButton};
        bottomButton.navigation = bottomNav;
    }

    public void OpenControls()
    {
        startPanel.SetActive(false);
        controlsPanel.SetActive(true);
        StartCoroutine(SelectButtonNextFrame(backButton));
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        startPanel.SetActive(true);
        StartCoroutine(SelectButtonNextFrame(controlsButton));
    }
    
    public void SetUpMainMenuNavigation()
    {
        Navigation startNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnUp = quitButton, selectOnDown = controlsButton};
        startButton.navigation = startNav;
        
        Navigation controlsNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnUp = startButton, selectOnDown = quitButton};
        controlsButton.navigation = controlsNav;
        
        Navigation quitNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnUp = controlsButton, selectOnDown = startButton};
        quitButton.navigation = quitNav;
    }
    
    
    public void OnStartClicked() => EventManager.RaiseStartGame();
    public void OnQuitClicked() => EventManager.RaiseQuitGame();
    
}