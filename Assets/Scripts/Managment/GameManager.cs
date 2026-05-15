using System.Collections;
using UnityEngine;

namespace Managment
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum GameState
        {
            Title,
            Gameplay,
            Paused,
            GameOver,
            OpeningSequence,
            WinSequence
        }

        public enum GameResult
        {
            None,
            Win,
            Lose
        }

        public GameState CurrentState { get; private set; }
        public GameResult CurrentResult { get; private set; }

        private void Start()
        {
            CurrentResult = GameResult.None;
            ChangeState(GameState.Title);
        }

        private void OnEnable()
        {
            EventManager.OnStartGame += StartGame;
            EventManager.OnPauseGame += PauseGame;
            EventManager.OnResumeGame += ResumeGame;
            EventManager.OnQuitGame += QuitGame;
            
            EventManager.OnResetGame += ResetGame;

            EventManager.OnGameOver += LoseGame;
            EventManager.OnLoseGame += LoseGame;
            EventManager.OnWinGame += WinGame;
            
            EventManager.OnReturnToMenu += ReturnToMenu;
        }

        private void OnDisable()
        {
            EventManager.OnStartGame -= StartGame;
            EventManager.OnPauseGame -= PauseGame;
            EventManager.OnResumeGame -= ResumeGame;
            EventManager.OnQuitGame -= QuitGame;
            EventManager.OnResetGame -= ResetGame;

            EventManager.OnGameOver -= LoseGame;
            EventManager.OnLoseGame -= LoseGame;
            EventManager.OnWinGame -= WinGame;
            
            EventManager.OnReturnToMenu -= ReturnToMenu;
        }

        private void ChangeState(GameState newState)
        {
            Debug.Log("STATE CHANGED TO: " + newState);
            CurrentState = newState;

            Time.timeScale = 
                (newState == GameState.Gameplay || 
                 newState == GameState.OpeningSequence || 
                 newState == GameState.WinSequence) ? 1f : 0f;

            EventManager.RaiseGameStateChanged(newState);
        }

        private void StartGame()
        {
            CurrentResult = GameResult.None;
            ChangeState(GameState.OpeningSequence);
            StartCoroutine(PlayOpeningSequence());
        }

        private void ResetGame()
        {
            CurrentResult = GameResult.None;
            ChangeState(GameState.Gameplay);
        }

        private void PauseGame()
        {
            if (CurrentState == GameState.Gameplay) ChangeState(GameState.Paused);
            
        }

        private void ResumeGame()
        {
            if (CurrentState == GameState.Paused) ChangeState(GameState.Gameplay);
            
        }

        private void WinGame()
        {
            CurrentResult = GameResult.Win;
            ChangeState(GameState.WinSequence);
            StartCoroutine(PlayWinSequence());
        }

        private void LoseGame()
        {
            CurrentResult = GameResult.Lose;
            ChangeState(GameState.GameOver);
        }

        private void ReturnToMenu()
        {
            CurrentResult = GameResult.None;
            ChangeState(GameState.Title);
        }

        private void QuitGame()
        {
            Time.timeScale = 1f;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator PlayOpeningSequence()
        {
            //TODO:ADD SEQUENCE
            Debug.Log("Playing Opening Sequence...");
            yield return new WaitForSeconds(3f);
            ChangeState(GameState.Gameplay);
        }
        
        private IEnumerator PlayWinSequence()
        {
            //TODO:ADD SEQUENCE
            Debug.Log("Playing Winning Sequence...");
            yield return new WaitForSeconds(3f);
            ChangeState(GameState.GameOver);
        }
    }
}