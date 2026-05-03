using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float swimForce = 6f;

    [Header("Lifetime")]
    [SerializeField] private float lifetimeSeconds = 10f;

    private EnemyPool _pool;
    private Rigidbody2D _rb;
    private Transform _target;
    private float _lifeTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= lifetimeSeconds)
        {
            Despawn();
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null || _target == null)
        {
            return;
        }

        Vector2 direction =
            ((Vector2)_target.position - _rb.position).normalized;

        _rb.AddForce(direction * swimForce, ForceMode2D.Force);

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void Init(EnemyPool pool, Transform target)
    {
        _pool = pool;
        _target = target;
    }

    public void Despawn()
    {
        _pool?.Return(gameObject);
    }

    public void OnTakenFromPool()
    {
        _lifeTimer = 0f;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    public void OnReturnedToPool()
    {
        _target = null;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}