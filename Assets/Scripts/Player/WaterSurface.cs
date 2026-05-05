using Managment.SoundScripts;
using UnityEngine;

namespace Player
{
    public class WaterSurface : MonoBehaviour
    {
        public Transform snapPoint;
        
        [SerializeField] private AudioClip fillingAirClip;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                PlayerAirSupply playerAirSupply = other.GetComponent<PlayerAirSupply>();
                
                if (playerMovement != null  && playerAirSupply != null){
                    playerMovement.SnapToSurface(snapPoint.position.y);
                    playerAirSupply.SetUnderWater(false);
                    SoundManager.Instance.PlaySoundFXClip(fillingAirClip, transform, 0.8f, 5f);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerAirSupply  playerAirSupply = other.GetComponent<PlayerAirSupply>();
                if (playerAirSupply != null) { 
                    playerAirSupply.SetUnderWater(true);
                }
                
            }
        }
    }
}