using UnityEngine;
using Player;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PiranhaMovement1 : MonoBehaviour
{
    [Header("Piranha Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private Transform nestingGround;
    [SerializeField] private float nestingGroundRadius = 3;
    [SerializeField] private float nestingGroundBuffer = 2;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float playerDetectionRadius = 2;
    private Collider2D _playerCol;
    private bool _detectedPlayer;

    [Header("Wander Settings")]
    [SerializeField] private float stopDistance = 0.05f;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 5f;
    [SerializeField] private float bounceRecoveryTime = 2f;
    [SerializeField] private float bounceRotationDamping = 5f;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _currentTarget;

    // State trackers
    private bool _isBouncing = false;
    private float _bounceTimer = 0f;
    
    private bool _isWaiting = false;
    private float _waitTimer = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (_isBouncing)
        {
            _bounceTimer -= Time.fixedDeltaTime;
            if (_bounceTimer <= 0f)
            {
                _isBouncing = false;
            }
            _rb.angularVelocity = Mathf.Lerp(_rb.angularVelocity, 0f, bounceRotationDamping * Time.fixedDeltaTime);
            return; 
        }

        _playerCol = LookForPlayer();
        float distanceFromCenter = (transform.position - nestingGround.position).magnitude;
        if (_detectedPlayer && distanceFromCenter <= nestingGroundRadius + nestingGroundBuffer)
        {
            ChasePlayer(_playerCol.transform);
            return;
        }
        
        if (_isWaiting)
        {
            _waitTimer -= Time.fixedDeltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                PickNewTarget();
            }
            return; 
        }

        MoveToRandomTarget();
    }

    void MoveToRandomTarget()
    {
        float distance = Vector2.Distance(_rb.position, _currentTarget);

        if (distance > stopDistance)
        {
            Vector2 direction = (_currentTarget - _rb.position).normalized;
            _rb.linearVelocity = direction * speed;
            
            HandleSwimmingRotation(direction); // Call the shared rotation logic
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _isWaiting = true;
            _waitTimer = Random.Range(0.5f, 2f);
        }
    }

   void ChasePlayer(Transform playerLoc)
    {
        float distance = Vector2.Distance(_rb.position, playerLoc.position);

        if (distance > stopDistance)
        {
            Vector2 direction = ((Vector2)playerLoc.position - _rb.position).normalized;
            _rb.linearVelocity = direction * speed;

            HandleSwimmingRotation(direction); // Call the shared rotation logic
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _isWaiting = true;
            _waitTimer = Random.Range(0.5f, 2f);
        }
    }

    private void HandleSwimmingRotation(Vector2 direction)
    {
        //Calculate the base angle pointing towards the direction
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Add 180 degrees because the sprite is drawn facing LEFT
        float targetAngle = baseAngle + 180f;

        float smoothedAngle = Mathf.LerpAngle(_rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(smoothedAngle);

        if (direction.x > 0.05f) 
        {
            _spriteRenderer.flipY = true;
        }
        else if (direction.x < -0.05f)
        {
            _spriteRenderer.flipY = false;
        }
    }

    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * nestingGroundRadius;
        _currentTarget = (Vector2)nestingGround.position + randomOffset;
    }

    // Handle the Collision (Same as before)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
        {
            BounceBack(collision);
        }
        
        if (collision.gameObject.transform.parent != null && 
            collision.gameObject.transform.parent.TryGetComponent(out PlayerAirSupply airSupply))
        {
            airSupply.UseAirSupply(damage);
            // Fix the copy-paste artifact in your log! :)
            Debug.Log("Piranha hit player, oxygen reduced by: " + damage); 
        }
    }

    void BounceBack(Collision2D collision)
    {
        _isBouncing = true;
        _bounceTimer = bounceRecoveryTime;
        _isWaiting = false;

        _rb.linearVelocity = Vector2.zero;
        Vector2 bounceDirection = collision.GetContact(0).normal;
        _rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
    }
    
    Collider2D LookForPlayer()
    {
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
        if (playerCol != null)
        {
            _detectedPlayer = true;
            return playerCol;
        }
        
        _detectedPlayer = false;
        return null;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (nestingGround)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(nestingGround.position, nestingGroundRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(nestingGround.position, nestingGroundRadius + nestingGroundBuffer);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}