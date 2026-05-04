using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyPool pool;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxActiveEnemies = 20;

    private int _activeCount;
    private float _timer;

    private void Start()
    {
        if (pool == null || enemyPrefab == null ||
            spawnPoint == null || player == null)
        {
            Debug.LogError("EnemySpawner: Missing reference.", this);
            enabled = false;
            return;
        }

        pool.Initialize(enemyPrefab, initialPoolSize);
    }

    private void OnEnable()
    {
        EventManager.OnEnemyDespawned += HandleEnemyDespawned;
    }

    private void OnDisable()
    {
        EventManager.OnEnemyDespawned -= HandleEnemyDespawned;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < spawnInterval)
        {
            return;
        }

        _timer = 0f;
        TrySpawn();
    }

    private void TrySpawn()
    {
        if (_activeCount >= maxActiveEnemies)
        {
            return;
        }

        GameObject enemy = pool.Get();
        enemy.transform.position = spawnPoint.position;

        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.Init(pool, player);
        }

        _activeCount++;
        EventManager.RaiseEnemySpawned(enemy);
    }

    private void HandleEnemyDespawned(GameObject enemy)
    {
        _activeCount = Mathf.Max(0, _activeCount - 1);
    }
}