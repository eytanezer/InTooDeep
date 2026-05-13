using Managment.SoundScripts;
using Player;
using UnityEngine;

public class PufferfishEnemy : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private float playerDetectionRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Inflation")]
    [SerializeField] private float inflationMultiplier = 1.8f;
    [SerializeField] private float inflateSpeed = 5f;
    [SerializeField] private float deflateSpeed = 3f;

    [Header("sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite inflatedSprite;
    
    [Header("Damage")]
    [SerializeField] private float damage = 15f;
    
    [Header("Audio Settings")] 
    [SerializeField] private AudioClip detectionSound;
    [SerializeField] private float soundVolume = 0.1f;

    private static Vector3 _normalScale = Vector3.one;
    private Vector3 _inflatedScale;
    
    private Vector3 _targetScale;
    private float _scalingSpeed;
    private bool _isInflated;

    //returning to original location variables
    [SerializeField] private float swimSpeed = 1;
    private Vector3 _originalLocation;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private float _waitTimer;
    private bool _isWaiting;
    private bool _isReturning;
    
    private void OnEnable()
    {
        EventManager.OnResetGame += ResetEnemy;
    }

    private void OnDisable()
    {
        EventManager.OnResetGame -= ResetEnemy;
    }
    
    void Start()
    {
        _inflatedScale = _normalScale * inflationMultiplier;
        
        _originalLocation = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = normalSprite;
    }

    private void Update()
    {
        ScanForPlayer();
        UpdateAttributes();
        
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            _targetScale,
            _scalingSpeed * Time.deltaTime
        );
    }

    private void FixedUpdate()
    {
        if (_isWaiting)
        {
            _waitTimer -= Time.fixedDeltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                _isReturning = true;
            }
        }
        else if (_isReturning)
        {
            Vector2 direction = (_originalLocation - transform.position).normalized;
            _rb.linearVelocity = direction * swimSpeed;

            // Stop moving once it gets close enough to the original location
            if (Vector2.Distance(transform.position, _originalLocation) < 0.1f)
            {
                _isReturning = false;
                _rb.linearVelocity = Vector2.zero;
                transform.position = _originalLocation;
            }
        }
    }

    void UpdateAttributes()
    {
        if (_isInflated)
        {
            _targetScale = _inflatedScale;
            _scalingSpeed = inflateSpeed;
            _spriteRenderer.sprite = inflatedSprite;
        }
        else
        {
            _targetScale = _normalScale;
            _scalingSpeed = deflateSpeed;
            _spriteRenderer.sprite = normalSprite;
        }
    }

    void ScanForPlayer()
    {
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
        if (playerCol && !_isInflated)
        {
            _isInflated = true;
            
            if(detectionSound != null) SoundManager.Instance.PlaySoundFXClip(detectionSound, transform, soundVolume);
        }
        else if (playerCol == null && _isInflated)
        {
            _isInflated = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isWaiting = true;
        _waitTimer = Random.Range(0.5f, 2f);
        
        PlayerAirSupply airSupply =
            collision.gameObject.GetComponentInParent<PlayerAirSupply>();

        if (airSupply == null)
        {
            return;
        }
        airSupply.UseAirSupply(damage);

        Debug.Log("Pufferfish hit player, oxygen reduced by: " + damage);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
    
    private void ResetEnemy()
    {
        transform.position = _originalLocation;
        transform.localScale = _normalScale;

        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _isInflated = false;
        _isWaiting = false;
        _isReturning = false;
        _waitTimer = 0f;

        _targetScale = _normalScale;
        _scalingSpeed = deflateSpeed;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = normalSprite;
        }
    }
}