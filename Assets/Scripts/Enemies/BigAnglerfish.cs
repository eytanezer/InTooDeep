using System;
using UnityEngine;

public class BigAnglerfish : MonoBehaviour
{
    private enum FishState { Sleep, WakingUp, Awake, Confused, Returning }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float acceleration = 5f;

    [Header("Detection Settings")]
    [SerializeField] private float playerDetectionRadius = 2f;
    [SerializeField] private float awakePlayerDetectionRadius = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Timing Settings")]
    [SerializeField] private float wakeUpWaitTime = 1f;
    [SerializeField] private float confusedWaitTime = 2f;

    private Rigidbody2D _rb;
    private Collider2D _playerCol;

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

        if (_playerCol != null)
        {
            Vector2 direction = ((Vector2)_playerCol.transform.position - (Vector2)transform.position).normalized;
            targetVelocity = direction * speed;
        }
        else if (_currentState == FishState.Returning)
        {
            Vector2 direction = (_startPosition - (Vector2)transform.position).normalized;
            targetVelocity = direction * speed;
        }

        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float visualizationRadius = _currentState != FishState.Sleep ? awakePlayerDetectionRadius : playerDetectionRadius;
        Gizmos.DrawWireSphere(transform.position, visualizationRadius);
    }
}
