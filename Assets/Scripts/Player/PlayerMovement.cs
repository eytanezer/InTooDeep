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

        [Header("Movement Settings")] [SerializeField]
        private float maxSpeed = 5;
        private float swimForce = 10;
    
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