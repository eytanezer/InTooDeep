using Managment.SoundScripts;
using UnityEngine;
using Player;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BigAnglerfish1 : MonoBehaviour
{
    private enum FishState { Sleep, Chasing, Returning }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Detection Settings")] [SerializeField]
    private Vector2 playerDetectionOffset;
    [SerializeField] private float playerDetectionRadius = 2f;
    [SerializeField] private float awakePlayerDetectionRadius = 6f;
    [SerializeField] private float maxChaseRadius = 40f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Audio Settings")] 
    [SerializeField] private AudioClip detectionSound;
    [SerializeField] private float soundVolume = 0.1f;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _playerCol;
    private Vector2 _currentTarget;
    private bool _detectedPlayer;
    
    private Vector2 _startPosition;
    private FishState _state = FishState.Sleep;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
    }

    void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        Collider2D playerCol = LookForPlayer();
        if (playerCol != null)
        {
            if (_state == FishState.Chasing)
            {
                if(detectionSound != null) SoundManager.Instance.PlaySoundFXClip(detectionSound, transform, soundVolume);
            }
            _state = FishState.Chasing;
        }
        
        switch (_state)
        {
            case FishState.Sleep:
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.rotation = 0f;
                break;
            case FishState.Chasing:
                if (playerCol == null)
                {
                    _state = FishState.Returning;
                }
                else
                {
                    ChasePlayer(playerCol.transform);
                }
                break;
            case FishState.Returning:
                if (((Vector2)transform.position - _startPosition).sqrMagnitude <= 0.25f)
                {
                    _state = FishState.Sleep;
                }
                else
                {
                    ReturningToCave();
                }
                break;
        }
    }
    
    Collider2D LookForPlayer()
    {
        float awarenessRadius = _state == FishState.Sleep ? playerDetectionRadius : awakePlayerDetectionRadius; 
        Collider2D playerCol = Physics2D.OverlapCircle((Vector2)transform.position + playerDetectionOffset, awarenessRadius, playerLayer);

        if (((Vector2)transform.position - _startPosition).magnitude > maxChaseRadius)
        {
            return null;
        }
        if (playerCol != null)
        {
            _detectedPlayer = true;
            return playerCol;
        }
        
        _detectedPlayer = false;
        return null;
    }
    
    void ChasePlayer(Transform playerLoc)
    {
        // Get the direction to the player
        Vector2 direction = ((Vector2)playerLoc.position - _rb.position).normalized;

        // 1. Apply a physical push. 
        // AddForce automatically factors in Rigidbody mass and Time.fixedDeltaTime.
        _rb.AddForce(direction * acceleration); 

        // 2. Clamp the velocity so the fish doesn't accelerate infinitely
        if (_rb.linearVelocity.magnitude > speed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * speed;
        }

        // Call the shared rotation logic
        // Note: We pass the actual velocity direction if it's moving, 
        // so it looks where it's actually going (bounces included), not just where it wants to go.
        if (_rb.linearVelocity.magnitude > 0.1f)
        {
            HandleSwimmingRotation(_rb.linearVelocity.normalized); 
        }
        else 
        {
            HandleSwimmingRotation(direction);
        }
    }
    
    private void HandleSwimmingRotation(Vector2 direction)
    {
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerAirSupply airSupply = collision.gameObject.GetComponentInParent<PlayerAirSupply>();
        if (airSupply != null)
        {
            airSupply.UseAirSupply(damage);
            Debug.Log("Piranha hit player, oxygen reduced by: " + damage); 
        }
    }

    void ReturningToCave()
    {
        // 1. Handle Movement (Move towards the start position)
        Vector2 direction = (_startPosition - _rb.position).normalized;
        Vector2 targetVelocity = direction * speed;
    
        _rb.linearVelocity = Vector2.MoveTowards(_rb.linearVelocity, targetVelocity, acceleration * 50f * Time.fixedDeltaTime);

        // 2. Handle Rotation (Smoothly transition back to 0 degrees)
        float smoothedAngle = Mathf.LerpAngle(_rb.rotation, 0f, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(smoothedAngle);

        // 3. Reset the sprite flip so the fish isn't upside down when it goes to sleep
        _spriteRenderer.flipY = false;
    }
    
    private void OnDrawGizmos()
    {
        Vector2 spawnPoint = Application.isPlaying ? _startPosition : (Vector2)transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerDetectionOffset, playerDetectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerDetectionOffset, awakePlayerDetectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, maxChaseRadius);
    }
}
