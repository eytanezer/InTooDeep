using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool pool;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxActiveEnemies = 20;
    [Range(0f, 0.45f)]
    [SerializeField] private float spawnPadding = 0.05f;

    private int _activeCount;
    private float _timer;

    private void Start()
    {
        if (pool == null)
        {
            Debug.LogError("EnemySpawner: pool is not assigned.", this);
            return;
        }
        pool.Initialize(enemyPrefab, initialPoolSize);
    }

    private void OnEnable()  => EventManager.OnEnemyDespawned += HandleEnemyDespawned;
    private void OnDisable() => EventManager.OnEnemyDespawned -= HandleEnemyDespawned;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;
        TrySpawn();
    }

    private void TrySpawn()
    {
        if (_activeCount >= maxActiveEnemies) return;

        var enemy = pool.Get();
        enemy.transform.position = GetRandomSpawnPosition();

        if (enemy.TryGetComponent<EnemyController>(out var ctrl))
            ctrl.Init(pool);

        _activeCount++;
        EventManager.RaiseEnemySpawned(enemy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var cam = Camera.main;
        if (cam == null) return Vector3.zero;
        var min = cam.ViewportToWorldPoint(new Vector3(spawnPadding, spawnPadding, cam.nearClipPlane));
        var max = cam.ViewportToWorldPoint(new Vector3(1f - spawnPadding, 1f - spawnPadding, cam.nearClipPlane));
        return new Vector3(UnityEngine.Random.Range(min.x, max.x),
                          UnityEngine.Random.Range(min.y, max.y),
                          0f);
    }

    private void HandleEnemyDespawned(GameObject enemy)
    {
        _activeCount = Mathf.Max(0, _activeCount - 1);
    }
}
