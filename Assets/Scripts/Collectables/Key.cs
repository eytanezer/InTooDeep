using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Collectables
{
    public class Key : MonoBehaviour
    {
        [Header("Proximity Settings")]
        [SerializeField] private float PlayerDistanceLimit;
        
        
        [Header("Light Settings")]
        [SerializeField] private float MaxLightIntensity;
        [SerializeField] private float GlowSpeed;
        
        
        private Transform _playerTransform;
        private bool _isPlayerInRange;
        
        private Light2D _light2D;
        private float _targetLightIntensity = 0f;

        void Start()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
            _light2D = GetComponentInChildren<Light2D>();
            
            _light2D.intensity = 0f;
            _light2D.enabled = true;
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            if (_isPlayerInRange && distance > PlayerDistanceLimit)
            {
                PlayerInRange(false);
            }
            if(!_isPlayerInRange && distance <= PlayerDistanceLimit)
            {
                PlayerInRange(true);
            }

            if (_light2D.intensity != _targetLightIntensity)
            {
                _light2D.intensity = Mathf.MoveTowards(_light2D.intensity, _targetLightIntensity, GlowSpeed * Time.deltaTime);
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
            
            _targetLightIntensity = isPlayerInRange ? MaxLightIntensity : 0f;
            _isPlayerInRange = isPlayerInRange;
        }
    }
}
