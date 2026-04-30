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
    
        private Vector2 _moveInput;

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
        
        }

        void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
    

        private void FixedUpdate()
        {
            _rb.AddForce(_moveInput * swimForce, ForceMode2D.Force);
            if(_rb.linearVelocity.magnitude > maxSpeed){
                _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;}

            if (_moveInput != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
                
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.fixedDeltaTime 
                );
                // if (Mathf.Abs(_rb.linearVelocity.x) > 0.1f)
                // {
                //     _spriteRenderer.flipY = _rb.linearVelocity.x < 0;
                // }
                if(_moveInput.x > 0.1f){
                    _spriteRenderer.flipY = false;
                } else if(_moveInput.x < -0.1f){
                    _spriteRenderer.flipY = true;
                }
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