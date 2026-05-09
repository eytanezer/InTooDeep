using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PiranhaMovement : MonoBehaviour
{
    [Header("Piranha Settings")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float playerDetectionRadius = 1f;
    [SerializeField] private Transform nestingGround;
    [SerializeField] private float nestingGroundRadius = 5f;
    [SerializeField] private float nestingGroundBuffer = 2f;
    [SerializeField] private float minChooseTargetTime = 1f;
    [SerializeField] private float maxChooseTargetTime = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [Header("Rotation Settings")]
    [SerializeField] private float rotationLerpSpeed = 6f;  // Higher is snappier, lower is smoother

    private Rigidbody2D _rb;
    private Vector2 _targetPosition;
    private bool _chasingPlayer = false;
    private Transform _playerTransform;
    private Coroutine _randomMoveCoroutine;
    private Coroutine _chasePlayerCoroutine;
    private Vector2 _lastDirection = Vector2.right; // Default start facing right

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _randomMoveCoroutine = StartCoroutine(RandomSwimRoutine());
    }

    void Update()
    {
        // Detect the player
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
        if (playerCol && !_chasingPlayer)
        {
            _playerTransform = playerCol.transform;
            _chasingPlayer = true;
            StopAndChasePlayer();
        }
        else if (_chasingPlayer)
        {
            float distanceFromNesting = Vector2.Distance(transform.position, nestingGround.position);
            if (distanceFromNesting > nestingGroundRadius + nestingGroundBuffer)
            {
                _chasingPlayer = false;
                _playerTransform = null;
                ResumeRandomMovement();
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = Vector2.zero;
        if (!_chasingPlayer)
        {
            moveDirection = (_targetPosition - (Vector2)transform.position).normalized;
            _rb.AddForce(moveDirection * speed, ForceMode2D.Force);

            if (Vector2.Distance(transform.position, _targetPosition) < 0.5f)
                PickNewTarget();
        }
        // Else, chasing player logic handled in ChasePlayer()

        // Choose facing direction (either chase direction or random movement direction)
        if (_chasingPlayer && _playerTransform != null)
        {
            Vector2 chaseDir = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
            if (chaseDir.sqrMagnitude > 0.0001f)
                _lastDirection = chaseDir;
        }
        else if (moveDirection.sqrMagnitude > 0.0001f)
        {
            _lastDirection = moveDirection;
        }

        UpdateRotation();
    }

    // This will smoothy rotate using lerp towards the _lastDirection vector
    private void UpdateRotation()
    {
        if (_lastDirection.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(_lastDirection.y, _lastDirection.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.fixedDeltaTime * rotationLerpSpeed);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    IEnumerator RandomSwimRoutine()
    {
        while (!_chasingPlayer)
        {
            PickNewTarget();
            float waitTime = Random.Range(minChooseTargetTime, maxChooseTargetTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void PickNewTarget()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomRadius = Random.Range(0f, nestingGroundRadius);
        Vector2 randomTarget = (Vector2)nestingGround.position + randomDir * randomRadius;
        _targetPosition = randomTarget;
    }

    void StopAndChasePlayer()
    {
        if (_randomMoveCoroutine != null)
        {
            StopCoroutine(_randomMoveCoroutine);
            _randomMoveCoroutine = null;
        }
        _chasePlayerCoroutine = StartCoroutine(ChasePlayerCoroutine());
    }

    void ResumeRandomMovement()
    {
        if (_chasePlayerCoroutine != null)
        {
            StopCoroutine(_chasePlayerCoroutine);
            _chasePlayerCoroutine = null;
        }
        _randomMoveCoroutine = StartCoroutine(RandomSwimRoutine());
    }

    IEnumerator ChasePlayerCoroutine()
    {
        while (_chasingPlayer)
        {
            Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
            _rb.AddForce(direction * speed, ForceMode2D.Force);
            // Rotation will be handled in FixedUpdate for consistency (using _lastDirection)
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            if (!_chasingPlayer)
                PickNewTarget();
        }
    }

    // Optional: For debug visualization
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