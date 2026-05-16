using Managment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LowOxygenPostProcessFlash : MonoBehaviour
{
    [SerializeField] private Volume volume;

    [Header("Low Oxygen")]
    [SerializeField] private float warningThreshold = 0.3f;

    [Header("Flash")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField] private float flashSpeed = 4f;
    [SerializeField] private float maxFlashStrength = 0.6f;

    private ColorAdjustments _colorAdjustments;
    private float _airPercent = 1f;

    private void Awake()
    {
        if (volume == null)
        {
            volume = GetComponent<Volume>();
        }

        if (volume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            _colorAdjustments = colorAdjustments;
        }
    }

    private void OnEnable()
    {
        EventManager.OnAirSupplyChanged += HandleAirSupplyChanged;
        EventManager.OnStartNewRun += ResetFlash;
    }

    private void OnDisable()
    {
        EventManager.OnAirSupplyChanged -= HandleAirSupplyChanged;
        EventManager.OnStartNewRun -= ResetFlash;
    }

    private void Update()
    {
        if (_colorAdjustments == null)
        {
            return;
        }

        if (_airPercent > warningThreshold)
        {
            _colorAdjustments.colorFilter.value = normalColor;
            return;
        }

        float danger = 1f - (_airPercent / warningThreshold);
        float pulse = (Mathf.Sin(Time.unscaledTime * flashSpeed) + 1f) / 2f;
        float strength = danger * pulse * maxFlashStrength;

        _colorAdjustments.colorFilter.value =
            Color.Lerp(normalColor, warningColor, strength);
    }

    private void HandleAirSupplyChanged(float airPercent)
    {
        _airPercent = Mathf.Clamp01(airPercent);
    }

    private void ResetFlash()
    {
        _airPercent = 1f;

        if (_colorAdjustments != null)
        {
            _colorAdjustments.colorFilter.value = normalColor;
        }
    }
}