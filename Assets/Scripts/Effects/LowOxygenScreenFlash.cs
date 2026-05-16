using Managment;
using UnityEngine;
using UnityEngine.UI;

public class LowOxygenScreenFlash : MonoBehaviour
{
    [SerializeField] private Image redPanel;

    [Header("Low Oxygen")]
    [SerializeField] private float warningThreshold = 0.3f;

    [Header("Flash")]
    [SerializeField] private float flashSpeed = 5f;
    [SerializeField] private float maxAlpha = 0.35f;

    private float _airPercent = 1f;
    private bool _isGameplay;

    private void OnEnable()
    {
        EventManager.OnAirSupplyChanged += HandleAirSupplyChanged;
        EventManager.OnStartNewRun += ResetFlash;
        EventManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        EventManager.OnAirSupplyChanged -= HandleAirSupplyChanged;
        EventManager.OnStartNewRun -= ResetFlash;
        EventManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (redPanel == null)
        {
            return;
        }

        Color color = redPanel.color;

        if (!_isGameplay || _airPercent > warningThreshold)
        {
            color.a = 0f;
            redPanel.color = color;
            return;
        }

        float danger = 1f - (_airPercent / warningThreshold);
        float pulse = (Mathf.Sin(Time.unscaledTime * flashSpeed) + 1f) / 2f;

        color.a = danger * pulse * maxAlpha;
        redPanel.color = color;
    }

    private void HandleAirSupplyChanged(float airPercent)
    {
        _airPercent = Mathf.Clamp01(airPercent);
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        _isGameplay = state == GameManager.GameState.Gameplay;

        if (!_isGameplay)
        {
            ResetFlash();
        }
    }

    private void ResetFlash()
    {
        _airPercent = 1f;

        if (redPanel != null)
        {
            Color color = redPanel.color;
            color.a = 0f;
            redPanel.color = color;
        }
    }
}