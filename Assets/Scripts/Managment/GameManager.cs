using UnityEngine;

namespace Managment
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum GameState
        {
            Title,
            Instructions,
            Gameplay,
            Paused,
            GameOver
        }

        public GameState CurrentState { get; private set; }

        private void Start()
        {
            ChangeState(GameState.Title);
        }

        private void OnEnable()
        {
            EventManager.OnStartGame += StartGame;
            EventManager.OnPauseGame += PauseGame;
            EventManager.OnResumeGame += ResumeGame;
            EventManager.OnGameOver += GameOver;
        }

        private void OnDisable()
        {
            EventManager.OnStartGame -= StartGame;
            EventManager.OnPauseGame -= PauseGame;
            EventManager.OnResumeGame -= ResumeGame;
            EventManager.OnGameOver -= GameOver;
        }

        private void ChangeState(GameState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            EventManager.RaiseGameStateChanged(newState);
        }

        private void StartGame()
        {
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

        private void GameOver()
        {
            ChangeState(GameState.GameOver);
        }
    }
}