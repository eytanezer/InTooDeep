using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public int KeyCount { get; private set; } = 0;
    
        public void AddKey()
        {
            KeyCount++;
            Debug.Log("Key collected! Total keys: " + KeyCount);
        }

        public void ResetKeyCount()
        {
            KeyCount = 0;
        }
    }
}
