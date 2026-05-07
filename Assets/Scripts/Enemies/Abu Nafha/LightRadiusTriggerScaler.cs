using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightRadiusTriggerScaler : MonoBehaviour
{
    [Header("Light Reference")]
    [SerializeField] private Light2D playerLight;

    [Header("Radius Multiplier")]
    [SerializeField] private float radiusMultiplier = 1f;

    private CircleCollider2D _circleCollider;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (playerLight == null)
        {
            return;
        }

        _circleCollider.radius =
            playerLight.pointLightOuterRadius * radiusMultiplier;
    }
}