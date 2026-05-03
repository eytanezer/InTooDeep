using System;
using UnityEngine;
using Player;

namespace Collectables
{
    public class TreasureChest : MonoBehaviour
    {
        [Header("Chest Settings")]
        [SerializeField] private int requiredKeys = 3;
        
        private bool _isOpen = false;

        private void Start()
        {
            _isOpen = false;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.unityLogger.Log("T1");
            if(_isOpen){return;}
            Debug.unityLogger.Log("T2");
            if (other.CompareTag("Player"))
            {
                Debug.unityLogger.Log("T3");
                PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
                if (playerInventory != null && playerInventory.KeyCount >= requiredKeys)
                {
                    // OpenChest();
                    Debug.unityLogger.Log("Treasure chest collected");
                    _isOpen = true;
                }
                else
                {
                    Debug.unityLogger.Log("Treasure still locked");
                }
            }
        }

        private void OpenChest()
        {
            // TODO: add chest opening animation
        }
    }
}
