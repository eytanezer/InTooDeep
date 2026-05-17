using Managment;
using UnityEngine;
using Player;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PiranhaMovement1 : MonoBehaviour
{
    [Header("Piranha Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private Transform nestingGround;
    [SerializeField] private float nestingGroundRadius = 3;
    [SerializeField] private float nestingGroundBuffer = 2;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float playerDetectionRadius = 2;
    private Collider2D _playerCol;
    private bool _detectedPlayer;

    [Tooltip("How close to the target before picking a new one or pausing")]
    [SerializeField] private float waypointTolerance = 0.5f;
    [Tooltip("Chance to pause when reaching a waypoint (0 to 1)")]
    [SerializeField] private float pauseChance = 0.5f;
    
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 5f;
    [SerializeField] private float bounceRecoveryTime = 2f;
    [SerializeField] private float bounceRotationDamping = 5f;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _currentTarget;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    // State trackers
    private bool _isBouncing = false;
    private float _bounceTimer = 0f;
    
    private bool _isWaiting = false;
    private float _waitTimer = 0f;
    
    private bool _canMove = false;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        PickNewTarget();
    }
    
    private void OnEnable()
    {
        EventManager.OnStartNewRun += ResetEnemy;
        EventManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        EventManager.OnStartNewRun -= ResetEnemy;
        EventManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        _canMove = state == GameManager.GameState.Gameplay;
    }

    void FixedUpdate()
    {
        if(!_canMove) return;
        
        if (_isBouncing)
        {
            _bounceTimer -= Time.fixedDeltaTime;
            if (_bounceTimer <= 0f)
            {
                _isBouncing = false;
                PickNewTarget();
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

        if (distance > waypointTolerance)
        {
            Vector2 direction = (_currentTarget - _rb.position).normalized;
            Vector2 desiredVelocity = direction * speed;
            
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
            
            HandleSwimmingRotation(direction); // Call the shared rotation logic
        }
        else if (Random.value < pauseChance)
        {
            _isWaiting = true;
            _waitTimer = Random.Range(0.5f, 1.5f);
        }
        else
        {
            PickNewTarget();
        }
    }

   void ChasePlayer(Transform playerLoc)
    {
        float distance = Vector2.Distance(_rb.position, playerLoc.position);

        if (distance > waypointTolerance)
        {
            Vector2 direction = ((Vector2)playerLoc.position - _rb.position).normalized;
            Vector2 desiredVelocity = direction * speed;
            
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
            HandleSwimmingRotation(direction); // Call the shared rotation logic
        }
        else
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, acceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleSwimmingRotation(Vector2 direction)
    {
        //Calculate the base angle pointing towards the direction
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float smoothedAngle = Mathf.LerpAngle(_rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(smoothedAngle);
        
        Vector3 currentScale = transform.localScale;

        if (direction.x > 0.05f) 
        {
            // _spriteRenderer.flipY = false
            currentScale.y = Mathf.Abs(currentScale.y);
        }
        
        else if (direction.x < -0.05f)
        {
            // _spriteRenderer.flipY = true;
            currentScale.y = -Mathf.Abs(currentScale.y);
        }
        
        transform.localScale = currentScale;
    }

    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * nestingGroundRadius;
        _currentTarget = (Vector2)nestingGround.position + randomOffset;
    }

    // Handle the Collision (Same as before)
    void OnCollisionEnter2D(Collision2D collision)
    {
        BounceBack(collision);
            
        PlayerAirSupply airSupply = collision.gameObject.GetComponentInParent<PlayerAirSupply>();
        if (airSupply != null)
        {
            airSupply.UseAirSupply(damage);
            Debug.Log("Piranha hit player, oxygen reduced by: " + damage); 
            PlayerHitShake hitShake =
                collision.gameObject.GetComponentInParent<PlayerHitShake>();

            hitShake?.ShakeCamera();
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
    
    private void ResetEnemy()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;

        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _isBouncing = false;
        _bounceTimer = 0f;

        _isWaiting = false;
        _waitTimer = 0f;

        _detectedPlayer = false;
        _playerCol = null;

        _spriteRenderer.flipY = false;

        PickNewTarget();
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