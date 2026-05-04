using System;
using System.Collections.Generic;
using Collectables;
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
        
        private List<Image> _keyUIImages =  new List<Image>();

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

        private void OnEnable()
        {
            EventManager.OnKeyCollected += UpdateKeyDisplay;
        }

        private void OnDisable()
        {
            EventManager.OnKeyCollected -= UpdateKeyDisplay;
        }
    }
}