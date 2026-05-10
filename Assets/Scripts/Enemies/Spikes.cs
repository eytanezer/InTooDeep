using Player;
using UnityEngine;

namespace Enemies
{
    public class Spikes : MonoBehaviour
    {
        private Collider2D _collider;
        [SerializeField] private float damage = 10f;
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }
        
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerAirSupply airSupply =
                    collision.gameObject.GetComponentInParent<PlayerAirSupply>();

                if (!airSupply) return;
                
                airSupply.UseAirSupply(damage);
            }
        }
    }
}