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
            GameOver
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
            EventManager.OnResetGame += StartGame;

            EventManager.OnGameOver += LoseGame;
            EventManager.OnLoseGame += LoseGame;
            EventManager.OnWinGame += WinGame;
        }

        private void OnDisable()
        {
            EventManager.OnStartGame -= StartGame;
            EventManager.OnPauseGame -= PauseGame;
            EventManager.OnResumeGame -= ResumeGame;
            EventManager.OnQuitGame -= QuitGame;
            EventManager.OnResetGame -= StartGame;

            EventManager.OnGameOver -= LoseGame;
            EventManager.OnLoseGame -= LoseGame;
            EventManager.OnWinGame -= WinGame;
        }

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;

            Time.timeScale =
                newState == GameState.Gameplay ? 1f : 0f;

            EventManager.RaiseGameStateChanged(newState);
        }

        private void StartGame()
        {
            CurrentResult = GameResult.None;
            ChangeState(GameState.Gameplay);
        }

        private void PauseGame()
        {
            if (CurrentState == GameState.Gameplay)
            {
                ChangeState(GameState.Paused);
            }
        }

        private void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.Gameplay);
            }
        }

        private void WinGame()
        {
            CurrentResult = GameResult.Win;
            ChangeState(GameState.GameOver);
        }

        private void LoseGame()
        {
            CurrentResult = GameResult.Lose;
            ChangeState(GameState.GameOver);
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
    }
}