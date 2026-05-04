using Enemies;
using Player;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float swimForce = 6f;

    [Header("Damage")]
    [SerializeField] private float airDamageAmount = 10f;

    [Header("Lifetime")]
    [SerializeField] private float lifetimeSeconds = 10f;

    private EnemyPool _pool;
    private Rigidbody2D _rb;
    private Transform _target;
    private float _lifeTimer;
    private bool _hasHitPlayer;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION WITH: " + collision.gameObject.name);

        PlayerAirSupply airSupply =
            collision.gameObject.GetComponent<PlayerAirSupply>();

        if (airSupply == null)
        {
            Debug.Log("PlayerAirSupply is NULL");
            return;
        }

        Debug.Log("FOUND PlayerAirSupply, reducing air");

        airSupply.UseAirSupply(airDamageAmount);
    }

    public void Despawn()
    {
        _pool?.Return(gameObject);
    }

    public void OnTakenFromPool()
    {
        _lifeTimer = 0f;
        _hasHitPlayer = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    public void OnReturnedToPool()
    {
        _target = null;
        _hasHitPlayer = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}