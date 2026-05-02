using UnityEngine;
using Player;

namespace Collectables
{
    public class TreasureChest : MonoBehaviour
    {
        [Header("Chest Settings")]
        [SerializeField] private int requiredKeys = 3;
        
        private bool _isOpen = false;
        

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_isOpen){return;}

            if (collision.CompareTag("Player"))
            {
                PlayerInventory playerInventory = collision.GetComponent<PlayerInventory>();
                if (playerInventory != null && playerInventory.KeyCount >= requiredKeys)
                {
                    OpenChest();
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
