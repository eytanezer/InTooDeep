using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class DashBubbleEffect_2 : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dashBubbleEffect;
        [SerializeField] private float duration = 1f;

        void OnEnable()
        {
            EventManager.OnDash += PlayDashBubbles;
        }

        void OnDisable()
        {
            EventManager.OnDash -= PlayDashBubbles;
        }

        private void PlayDashBubbles()
        {
            StartCoroutine(PlayDashBubblesCoroutine());
        }
        
        private IEnumerator PlayDashBubblesCoroutine()
        {
            dashBubbleEffect.Play();
            yield return new WaitForSeconds(duration);
            dashBubbleEffect.Stop();
        }
    }
}