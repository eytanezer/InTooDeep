using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public int KeyCount { get; private set; } = 0;
    
        public void AddKey(Vector3 position)
        {
            KeyCount++;
            Debug.Log("Key collected! Total keys: " + KeyCount);
            EventManager.RaiseKeyCollected(KeyCount,  position);
        }

        public void ResetKeyCount()
        {
            KeyCount = 0;
            EventManager.RaiseKeyCollected(KeyCount, Vector3.zero);
        }
        
        private void OnEnable()
        {
            EventManager.OnResetGame += ResetKeyCount;
        }
        private void OnDisable(){
            EventManager.OnResetGame -= ResetKeyCount;
            
        }
    }
}
