using System;
using UnityEngine;
using Managment;

public class EventManager : MonoBehaviour
{
    #region Actions

    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDespawned;

    public static event Action<float> OnAirSupplyChanged;
    public static event Action<float> OnMaxAirSupplyChanged;
    public static event Action<int> OnKeyCollected;

    public static event Action OnStartGame;
    public static event Action OnPauseGame;
    public static event Action OnResumeGame;
    public static event Action OnQuitGame;
    public static event Action OnResetGame;

    public static event Action OnGameOver;
    public static event Action OnWinGame;
    public static event Action OnLoseGame;

    public static event Action<GameManager.GameState> OnGameStateChanged;

    #endregion

    #region Invoking Events

    public static void RaiseEnemySpawned(GameObject enemy) =>
        OnEnemySpawned?.Invoke(enemy);

    public static void RaiseEnemyDespawned(GameObject enemy) =>
        OnEnemyDespawned?.Invoke(enemy);

    public static void RaiseAirSupplyChanged(float newAirSupply) =>
        OnAirSupplyChanged?.Invoke(newAirSupply);

    public static void RaiseMaxAirSupplyChanged(float newMaxAirSupply) =>
        OnMaxAirSupplyChanged?.Invoke(newMaxAirSupply);
    
    public static void RaiseKeyCollected(int totalKeys) =>
        OnKeyCollected?.Invoke(totalKeys);
    #endregion
    
    
    
    
    #region GameState Events
    public static void RaiseStartGame() =>
        OnStartGame?.Invoke();

    public static void RaisePauseGame() =>
        OnPauseGame?.Invoke();

    public static void RaiseResumeGame() =>
        OnResumeGame?.Invoke();

    public static void RaiseQuitGame() =>
        OnQuitGame?.Invoke();

    public static void RaiseResetGame() =>
        OnResetGame?.Invoke();

    public static void RaiseGameOver() =>
        OnGameOver?.Invoke();

    public static void RaiseWinGame() =>
        OnWinGame?.Invoke();

    public static void RaiseLoseGame() =>
        OnLoseGame?.Invoke();

    public static void RaiseGameStateChanged(GameManager.GameState state) =>
        OnGameStateChanged?.Invoke(state);

    #endregion
}