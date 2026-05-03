using UnityEngine;

namespace Player
{
    public class WaterSurface : MonoBehaviour
    {
        public Transform snapPoint;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                PlayerAirSupply playerAirSupply = other.GetComponent<PlayerAirSupply>();
                
                if (playerMovement != null  && playerAirSupply != null){
                    playerMovement.SnapToSurface(snapPoint.position.y);
                    playerAirSupply.SetUnderWater(false);
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