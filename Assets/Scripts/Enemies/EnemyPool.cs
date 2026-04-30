using UnityEngine;

public class EnemyPool : ObjectPool<EnemyController>
{
    public void Initialize(GameObject enemyPrefab, int initialSize)
    {
        EnemyController enemyController = enemyPrefab.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError("Enemy prefab must have EnemyController component.");
            return;
        }

        InitializePool(enemyController, initialSize);
    }

    public GameObject Get()
    {
        EnemyController enemy = GetFromPool();
        enemy.Init(this);
        return enemy.gameObject;
    }

    public void Return(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            ReturnToPool(enemyController);
            EventManager.RaiseEnemyDespawned(enemy);
        }
    }
}