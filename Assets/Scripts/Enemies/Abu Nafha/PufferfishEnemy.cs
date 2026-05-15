using Managment;
using Managment.SoundScripts;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PufferfishEnemy : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private float playerDetectionRadius = 2f;
    [SerializeField] private float playerDetectionRadiusInflated = 4f;
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
    
    [Header("Light Settings")]
    [SerializeField] private Light2D pufferLight;

    [SerializeField] private Color inflatedLightColor = Color.red;
    
    [SerializeField] private Color deflationLightColor = Color.white;

    [SerializeField] private float inflatedLightIntensity = 2f;

    [SerializeField] private float inflatedLightRadius = 3f;

    private static Vector3 _normalScale = Vector3.one;
    private Vector3 _inflatedScale;
    
    private bool _isInflated;
    private float _scalingSpeed;
    private Vector3 _targetScale;
    private Color _targetColor;
    private bool _gameplayStarted;

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
        _gameplayStarted = state == GameManager.GameState.Gameplay;

        if (!_gameplayStarted)
        {
            _isInflated = false;

            if (pufferLight != null)
            {
                pufferLight.intensity = 0f;
            }
        }
    }
    
    void Start()
    {
        _originalLocation = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = normalSprite;
    }

    private void Update()
    {
        if (!_gameplayStarted)
        {
            return;
        }
        Collider2D playerCol = ScanForPlayer();
        bool playerDetected = playerCol != null;
        ManageScale();
        ManageLight(playerDetected);
        
        if (_isInflated)
        {
            _spriteRenderer.sprite = inflatedSprite;
        }

        if (transform.localScale == _normalScale)
        {
            _spriteRenderer.sprite = normalSprite;
        }
    }

    private void FixedUpdate()
    {
        if (!_gameplayStarted)
        {
            return;
        }
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

    void ManageScale()
    {
        // 1. Determine the target scale based on detection
        Vector3 targetScale = _isInflated ? _normalScale * inflationMultiplier : _normalScale;

        // 2. Determine which speed to use
        float currentSpeed = _isInflated ? inflateSpeed : deflateSpeed;

        // 3. Move linearly towards the target
        transform.localScale = Vector3.MoveTowards(
            transform.localScale, 
            targetScale, 
            currentSpeed * Time.deltaTime
        );
    }
    
    void ManageLight(bool playerDetected)
    {
        float currentSpeed = _isInflated ? inflateSpeed : deflateSpeed;

        Color targetColor = playerDetected ? inflatedLightColor : deflationLightColor;
        float targetIntensity = _isInflated ? inflatedLightIntensity : 0f;
        float targetRadius = _isInflated ? inflatedLightRadius : 0f;
        
        pufferLight.color = Vector4.MoveTowards(
            pufferLight.color, 
            targetColor, 
            currentSpeed * Time.deltaTime 
        );
        
        pufferLight.intensity = Mathf.MoveTowards(
            pufferLight.intensity, 
            targetIntensity, 
            currentSpeed * Time.deltaTime
        );

        pufferLight.pointLightOuterRadius = Mathf.MoveTowards(
            pufferLight.pointLightOuterRadius, 
            targetRadius, 
            currentSpeed * Time.deltaTime
        );
    }

    Collider2D ScanForPlayer()
    {
        float detectRadius = _isInflated ? playerDetectionRadiusInflated : playerDetectionRadius;
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);
        
        if (playerCol && !_isInflated)
        {
            _isInflated = true;
            
            if(detectionSound != null) SoundManager.Instance.PlaySoundFXClip(detectionSound, transform, soundVolume);
        }
        else if (playerCol == null && _isInflated)
        {
            _isInflated = false;
        }
        return playerCol;
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
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadiusInflated);
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