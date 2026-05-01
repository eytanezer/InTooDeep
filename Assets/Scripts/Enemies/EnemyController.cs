using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float swimForce = 6f;
    [SerializeField] private float directionChangeInterval = 1.5f;

    [Header("Lifetime")]
    [SerializeField] private float lifetimeSeconds = 10f;

    private EnemyPool _pool;
    private Rigidbody2D _rb;
    private Vector2 _swimDirection;
    private float _directionTimer;
    private float _lifeTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;
        _directionTimer += Time.deltaTime;

        if (_directionTimer >= directionChangeInterval)
        {
            _directionTimer = 0f;
            PickRandomDirection();
        }

        if (_lifeTimer >= lifetimeSeconds)
        {
            Despawn();
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null) return;

        _rb.AddForce(_swimDirection * swimForce, ForceMode2D.Force);

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void Init(EnemyPool pool)
    {
        _pool = pool;
    }

    public void Despawn()
    {
        _pool?.Return(gameObject);
    }

    public void OnTakenFromPool()
    {
        _lifeTimer = 0f;
        _directionTimer = 0f;
        PickRandomDirection();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    public void OnReturnedToPool()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    private void PickRandomDirection()
    {
        _swimDirection = UnityEngine.Random.insideUnitCircle.normalized;

        if (_swimDirection == Vector2.zero)
        {
            _swimDirection = Vector2.right;
        }
    }
}