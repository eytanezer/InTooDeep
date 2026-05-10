using System;
using System.Collections.Generic;
using UnityEngine;
using Player;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BigAnglerfish : MonoBehaviour
{
    private enum FishState { Sleep, WakingUp, Awake, Confused, Returning }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float acceleration = 5f;
	[SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float wanderRadius = 1f;
    
    [Header("Detection Settings")]
    [SerializeField] private float playerDetectionRadius = 2f;
    [SerializeField] private float awakePlayerDetectionRadius = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Timing Settings")]
    [SerializeField] private float wakeUpWaitTime = 1f;
    [SerializeField] private float confusedWaitTime = 2f;
    
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
    private Collider2D _playerCol;
    private Vector2 _currentTarget;

    private FishState _currentState = FishState.Sleep;
    private Vector2 _startPosition;
    private float _stateTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    void Update()
    {

        LookForPlayer(_currentState == FishState.Awake || _currentState == FishState.Confused || _currentState == FishState.Returning);
        HandleStateMachine();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LookForPlayer(bool awake)
    {
        float checkRadius = awake ? awakePlayerDetectionRadius : playerDetectionRadius;
        _playerCol = Physics2D.OverlapCircle(transform.position, checkRadius, playerLayer);
    }

    private void HandleStateMachine()
    {
        switch (_currentState)
        {
            case FishState.Sleep:
                _stateTimer = wakeUpWaitTime;
                if (_playerCol != null)
                    _currentState = FishState.WakingUp;
                break;

            case FishState.WakingUp:
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0)
                    _currentState = FishState.Awake;
                break;

            case FishState.Awake:
                if (_playerCol == null)
                {
                    _currentState = FishState.Confused;
                    _stateTimer = confusedWaitTime;
                }
                break;

            case FishState.Confused:
                _stateTimer -= Time.deltaTime;
                if (_playerCol != null)
                    _currentState = FishState.Awake;
                else if (_stateTimer <= 0)
                    _currentState = FishState.Returning;
                break;

            case FishState.Returning:
                if (_playerCol != null)
                {
                    _currentState = FishState.Awake;
                }
                else if (Vector2.Distance(transform.position, _startPosition) < 0.1f)
                {
                    _currentState = FishState.Sleep;
                }
                break;
        }
    }

    private void HandleMovement()
    {
        Vector2 targetVelocity = Vector2.zero;
        Vector2 direction =  Vector2.left;
        
        if (_playerCol != null)
        {
            direction = ((Vector2)_playerCol.transform.position - (Vector2)transform.position).normalized;
            targetVelocity = direction * speed;
        }
        else if (_currentState == FishState.Returning)
        {
            direction = (_startPosition - (Vector2)transform.position).normalized;
            targetVelocity = direction * speed;
        }

        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        HandleSwimmingRotation(direction);
    }

    void PickNewTarget()
    {
        //Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        //_currentTarget = (Vector2)transform.position + randomOffset;
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
        //else if (Random.value < pauseChance)
        {
            //_isWaiting = true;
            //_waitTimer = Random.Range(0.5f, 1.5f);
        }
        //else
        {
            PickNewTarget();
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float visualizationRadius = _currentState != FishState.Sleep ? awakePlayerDetectionRadius : playerDetectionRadius;
        Gizmos.DrawWireSphere(transform.position, visualizationRadius);
    }
}
