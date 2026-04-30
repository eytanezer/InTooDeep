using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDespawned;

    public static void RaiseEnemySpawned(GameObject enemy)   => OnEnemySpawned?.Invoke(enemy);
    public static void RaiseEnemyDespawned(GameObject enemy) => OnEnemyDespawned?.Invoke(enemy);
}
