using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDespawned;
    
    public static event Action<float> OnAirSupplyChanged;
    public static event Action<float> OnMaxAirSupplyChanged;

    public static void RaiseEnemySpawned(GameObject enemy)   => OnEnemySpawned?.Invoke(enemy);
    public static void RaiseEnemyDespawned(GameObject enemy) => OnEnemyDespawned?.Invoke(enemy);
    
    public static void RaiseAirSupplyChanged(float newAirSupply) => OnAirSupplyChanged?.Invoke(newAirSupply);
    public static void RaiseMaxAirSupplyChanged(float newMaxAirSupply) => OnMaxAirSupplyChanged?.Invoke(newMaxAirSupply);
}
