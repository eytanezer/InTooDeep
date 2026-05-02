using UnityEngine;

namespace Collectables
{
    public class Key : MonoBehaviour
    {
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
    }
}
