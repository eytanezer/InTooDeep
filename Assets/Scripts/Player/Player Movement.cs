using Managment;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Controls the movement of the players
    /// 
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _spawnPosition;

        [Header("Movement Settings")] 
        [SerializeField] private float maxSpeed = 5;
        [SerializeField] private float swimForce = 10;
        [SerializeField] public float rotationSpeed = 360f;
        [SerializeField] public float inputSmoothSpeed = 0.1f;
        
        [Header("Dash Settings")]
        [SerializeField] public float dashForce = 20f;
        public float dashAirCost = 10f;
        public float dashCooldown = 1f;
        
        // Surface snapping
        private bool _isAtSurface = false;
        private float _surfaceSnapY;
    
        //movement system
        private Vector2 _targetInput;   // The raw input from the player
        private Vector2 _currentInput;
        private bool _inputChanged = false;
        
        private float _lastDashTime;
        private PlayerAirSupply _airSupply;

        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        // Components
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
    


        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spawnPosition = transform.position;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _airSupply = GetComponent<PlayerAirSupply>();
        
        }

        void OnMove(InputValue value)
        {
            // _moveInput = value.Get<Vector2>();
            Vector2 input = value.Get<Vector2>();
            _inputChanged = input != _targetInput;
            _targetInput = value.Get<Vector2>();
        }

        void OnJump(InputValue value)
        {
            if (value.isPressed && Time.time >= _lastDashTime + dashCooldown)
            {
                Dash();
            }
        }
    

        private void FixedUpdate()
        {
            if(_lastDashTime > 0) {_lastDashTime -= Time.fixedDeltaTime;}


            if (_isAtSurface)
            {
                transform.position = new Vector3(transform.position.x, _surfaceSnapY, transform.position.z);
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);

                if (_currentInput.y < -0.5f)
                {
                    _isAtSurface = false;
                    _rb.AddForce(_currentInput * swimForce, ForceMode2D.Impulse);
                }
            }
            
            _currentInput = Vector2.MoveTowards(_currentInput, _targetInput, Time.fixedDeltaTime * inputSmoothSpeed);

            Vector2 movementForce = _currentInput;
            if (_isAtSurface)
            {
                movementForce.y = 0f; // Disable upward/downward swimming force while snapped
            }
            
            if (_currentInput != Vector2.zero)
            {
                // negate gravity on rotation
                if (_inputChanged)
                {
                    // Debug.Log(_currentInput);
                    Vector2 counterGravityForce = -(Physics2D.gravity * (_rb.gravityScale * _rb.mass));
                    _rb.AddForce(counterGravityForce,  ForceMode2D.Force);
                    _inputChanged = false;
                }
                
                // input movement force
                _rb.AddForce(movementForce * swimForce);
                
                if(_lastDashTime <= 0 && _rb.linearVelocity.magnitude > maxSpeed){
                    _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
                }
                
                if (_currentInput.sqrMagnitude > 0.01f)
                {
                    float targetAngle = Mathf.Atan2(_currentInput.y, _currentInput.x) * Mathf.Rad2Deg;
                    
                    Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                    
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.fixedDeltaTime 
                    );
                    
                    if (Mathf.Abs(_currentInput.x) > 0.1f)
                    {
                        _spriteRenderer.flipY = _currentInput.x < 0;
                    }
                 
                }
            }
        }

        private void Dash()
        {
            if(_airSupply && _airSupply.UseAirSupply(dashAirCost))
            {
                Vector2 dashDirection = _currentInput.normalized;
                
                _rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
                _lastDashTime = dashCooldown;
            }
        }

        public void SnapToSurface(float surfaceSnapY)
        {
            if (!_isAtSurface)
            {
                _isAtSurface = true;
                _surfaceSnapY = surfaceSnapY;
                
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
                transform.position = new Vector3(transform.position.x, _surfaceSnapY, transform.position.z);
            }
        }
        
        
        [Tooltip("Teleports the player back to their original spawn position")]
        public void ResetToSpawn()
        {
            // Reset position
            transform.position = _spawnPosition;

            // If the player has a Rigidbody, kill any leftover movement/momentum
            if (_rb)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = 0;
            }
        }

        void OnEnable()
        {
            Cheats.OnResetPlayersPosition += ResetToSpawn;
        }

        void OnDisable()
        {
            Cheats.OnResetPlayersPosition -= ResetToSpawn;
        }
    }
}