using UnityEngine;

public class LightRadiusTriggerScaler : MonoBehaviour
{
    [Header("Collider Radius")]
    [SerializeField] private float maxRadius = 3f;
    [SerializeField] private float minRadius = 0.5f;

    private CircleCollider2D _circleCollider;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        EventManager.OnAirSupplyChanged += UpdateRadius;
    }

    private void OnDisable()
    {
        EventManager.OnAirSupplyChanged -= UpdateRadius;
    }

    private void UpdateRadius(float airPercent)
    {
        float newRadius = Mathf.Lerp(minRadius, maxRadius, airPercent);
        _circleCollider.radius = newRadius;
    }
}