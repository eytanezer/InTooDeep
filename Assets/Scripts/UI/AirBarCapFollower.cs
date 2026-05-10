using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AirBarCapFollower : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The Image component that FEEL is filling/draining")]
        [SerializeField] private Image fillImage;
    
        [Tooltip("The glowing cap RectTransform")]
        [SerializeField] private RectTransform capRect;

        [Header("Anchored Y Positions")]
        [Tooltip("The Y position of the cap when the bar is at 100%")]
        [SerializeField] private float fullYPosition;
    
        [Tooltip("The Y position of the cap when the bar is at 0%")]
        [SerializeField] private float emptyYPosition;
    }
}