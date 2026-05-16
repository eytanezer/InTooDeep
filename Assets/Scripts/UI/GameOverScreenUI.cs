using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managment;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;
    
    [Header("Visuals")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite winBackgroundSprite;
    [SerializeField] private Sprite loseBackgroundSprite;

    [SerializeField] private List<string> gameOverPrompts;
    
    [Header("Buttons")]
    [SerializeField] private Button menuButton;
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

        gameOverPanel.SetActive(shouldShow);

        if (shouldShow)
        {
            UpdateVisuals();
            StartCoroutine(SelectQuitButtonNextFrame());
        }
    }

    private IEnumerator SelectQuitButtonNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        
        SetVerticalNavigation(restartButton, menuButton);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        // restartButton.Select();
    }
    
    private void SetVerticalNavigation(Button topButton, Button bottomButton)
    {
        Navigation topNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnDown = bottomButton};
        topButton.navigation = topNav;

        Navigation bottomNav = new Navigation{mode = Navigation.Mode.Explicit, selectOnUp = topButton};
        bottomButton.navigation = bottomNav;
    }

    private void UpdateVisuals()
    {
        var result = GameManager.Instance.CurrentResult;
        if (resultText)
        {
            string randomLosePrompt = "Game Over";
            
            if (gameOverPrompts != null && gameOverPrompts.Count > 0)
            {
                // Random.Range for integers is EXCLUSIVE on the top end, 
                // so passing gameOverPrompts.Count is perfectly safe!
                int randomIndex = UnityEngine.Random.Range(0, gameOverPrompts.Count);
                randomLosePrompt = gameOverPrompts[randomIndex];
            }
            
            resultText.text =
                result == GameManager.GameResult.Win
                    ? "winner"
                    : randomLosePrompt;;
        }

        if (backgroundImage)
        {
            backgroundImage.sprite = result == GameManager.GameResult.Win
                ? winBackgroundSprite
                : loseBackgroundSprite;
        }
        
    }

    public void OnMenuClicked() => EventManager.RaiseReturnToMenu();
    

    public void OnRestartClicked() => EventManager.RaiseResetGame();
    
}