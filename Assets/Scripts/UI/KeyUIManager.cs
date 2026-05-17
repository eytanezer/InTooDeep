using System;
using System.Collections.Generic;
using Collectables;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class KeyUIManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private GameObject KeyUIPrefab;
        [SerializeField] private Transform KeyUIParent;
        
        [Header("Sprites")]
        [SerializeField] private Sprite EmptyKeySprites;
        [SerializeField] private Sprite LitKeySprite;
        
        [Header("Flying Effect")]
        [SerializeField] private GameObject flyingKeyPrefab;
        [SerializeField] private float flyDuration = 0.6f;
        
        private Camera _mainCamera;
        
        private List<Image> _keyUIImages =  new List<Image>();

        void Awake()
        {
            _mainCamera = Camera.main;
        }
        
        void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            int totalKeys = FindObjectsByType<Key>(FindObjectsSortMode.None).Length;

            foreach (Transform child in KeyUIParent)
            {
                Destroy(child.gameObject);
            }
            _keyUIImages.Clear();
            for (int i = 0; i < totalKeys; i++)
            {
                GameObject keyUIInstance = Instantiate(KeyUIPrefab, KeyUIParent);
                Image keyUIImage = keyUIInstance.GetComponent<Image>();
                if (keyUIImage)
                {
                    keyUIImage.sprite = EmptyKeySprites;
                    _keyUIImages.Add(keyUIImage);
                }
            }
        }

        public void UpdateKeyDisplay(int currentKeysCollected)
        {
            for (int i = 0; i < _keyUIImages.Count; i++)
            {
                if (i < currentKeysCollected)
                {
                    _keyUIImages[i].sprite = LitKeySprite;
                }
                else
                {
                    _keyUIImages[i].sprite = EmptyKeySprites;
                }
            }
        }
        
        private void StartKeyFlyEffect(int currentKeysCollected, Vector3 worldSpawnPosition)
        {
            if (worldSpawnPosition == Vector3.zero) {
                UpdateKeyDisplay(currentKeysCollected);
                return;
            }
            
            Vector2 screenPosition = _mainCamera.WorldToScreenPoint(worldSpawnPosition);
            
            Transform rootCanvas = GetComponentInParent<Canvas>().transform;
            
            GameObject flyingKey = Instantiate(flyingKeyPrefab, rootCanvas);
            
            Image image = flyingKey.GetComponent<Image>();
            if(image){
                image.sprite = LitKeySprite;
            }
            flyingKey.transform.SetAsLastSibling();
            flyingKey.transform.position = screenPosition;
            
            
            int targetKeysCollected = currentKeysCollected - 1;
            if (targetKeysCollected >= 0 && targetKeysCollected < _keyUIImages.Count)
            {
                Transform targetUITransform = _keyUIImages[targetKeysCollected].transform;
                
                flyingKey.transform.DOJump(targetUITransform.position, jumpPower: 50f, numJumps: 1, flyDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                {
                    Destroy(flyingKey);
                    UpdateKeyDisplay(currentKeysCollected);
                });
            }        
        }
        

        private void OnEnable()
        {
            EventManager.OnKeyCollected += StartKeyFlyEffect;
        }

        private void OnDisable()
        {
            EventManager.OnKeyCollected -= StartKeyFlyEffect;
        }
    }
}