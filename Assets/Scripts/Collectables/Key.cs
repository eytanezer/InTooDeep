using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Collectables
{
    public class Key : MonoBehaviour
    {
        
        [SerializeField] private float PlayerDistanceLimit;
        
        private Transform _playerTransform;
        private bool _isPlayerInRange;
        
        private Light2D _light2D;

        void Start()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
            _light2D = GetComponentInChildren<Light2D>();
            _light2D.enabled = false;
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            if (_isPlayerInRange && distance > PlayerDistanceLimit)
            {
                PlayerInRange(false);
            }
            if(!_isPlayerInRange && distance < PlayerDistanceLimit)
            {
                PlayerInRange(true);
            }
        }
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.PlayerInventory playerInventory = other.GetComponent<Player.PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.AddKey();
                    
                    // TODO: add sound and light effects
                    Destroy(gameObject);
                }
            }
        }

        private void PlayerInRange(bool isPlayerInRange)
        {
            _light2D.enabled = isPlayerInRange;
            _isPlayerInRange = isPlayerInRange;
        }
    }
}
