using System;
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
        
        private bool _isPlayerInRange;
        private bool _isCollected =  false;
        private float _targetLightIntensity = 0f;
        
        // components
        private Transform _playerTransform;
        private Light2D _light2D;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;

        void Start()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
            _light2D = GetComponentInChildren<Light2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            
            _light2D.intensity = 0f;
            _light2D.enabled = true;
        }

        void Update()
        {
            if(_isCollected || !_playerTransform) return;
            
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
                    CollectKey();
                }
            }
        }

        private void CollectKey()
        {
            _isCollected  = true;
            // EventManager.RaiseKeyCollected();
            
            if(_spriteRenderer) _spriteRenderer.enabled = false;
            if(_collider2D) _collider2D.enabled = false;
            if(_light2D) _light2D.enabled = false;
            PlayerInRange(false);
        }

        private void ResetKey()
        {
            _isCollected  = false;
            if(_spriteRenderer) _spriteRenderer.enabled = true;
            if(_collider2D) _collider2D.enabled = true;
            if(_light2D) _light2D.enabled = true;
        }

        private void PlayerInRange(bool isPlayerInRange)
        {
            
            _targetLightIntensity = isPlayerInRange ? MaxLightIntensity : 0f;
            _isPlayerInRange = isPlayerInRange;
        }

        private void OnEnable()
        {
            EventManager.OnResetGame += ResetKey;
        }
        private void OnDisable(){
            EventManager.OnResetGame -= ResetKey;
            
        }
    }
}
