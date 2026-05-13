using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Managment;


namespace Player
{
    public class PlayerLightControler : MonoBehaviour
    {

        [Header("Light Settings")] 
        [Header("Size")] 
        [SerializeField] private bool controlSize;
        [SerializeField] private float maxSize;
        [SerializeField] private float minSize;
        
        [Header("Intensity")]
        [SerializeField] private bool controlIntensity;
        [SerializeField] private float maxIntensity;
        [SerializeField] private float minIntensity;
        
        [Header("Angles")]
        [SerializeField] private bool controlAngles;
        [SerializeField] private float maxAngles;
        [SerializeField] private float minAngles;
    
        [Header("Breathing Pulse")]
        [SerializeField] private bool breathingPulse;
        [SerializeField] private float normalPulseInterval = 3f;
        [SerializeField] private float lowOxygenPulseInterval = 0.7f;
        [SerializeField] private float pulseAmount = 0.15f;

        private float _airSupplyPercentage = 1f;
        private float _baseRadius;
        private float _timer;
        
        
        private Light2D  _lightSource;
        
        private void OnEnable()
        {
            EventManager.OnResetGame += ResetLight;
        }

        private void OnDisable()
        {
            EventManager.OnResetGame -= ResetLight;
        }

        private void Awake()
        {
            _lightSource = GetComponent<Light2D>();
        }

        void Start()
        {
            ResetLight();
        }

        public void ResetLight()
        {
            _airSupplyPercentage = 1f;
            _timer = 0f;

            if (controlIntensity)
            {
                _lightSource.intensity = maxIntensity;
            }

            if (controlSize)
            {
                _baseRadius = maxSize;
                _lightSource.pointLightOuterRadius = maxSize;
            }

            if (controlAngles)
            {
                _lightSource.pointLightOuterAngle = maxAngles;
            }
        }

        public void UpdateLight(float airSupplyPercentage)
        {
            if (controlAngles) {_lightSource.pointLightOuterAngle = Mathf.Lerp(minAngles, maxAngles, airSupplyPercentage);}
            if (controlIntensity) {_lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, airSupplyPercentage);}
            if (controlSize)
            {
                _airSupplyPercentage = Mathf.Clamp01(airSupplyPercentage);
                _baseRadius = Mathf.Lerp(minSize, maxSize, _airSupplyPercentage);
                _lightSource.pointLightOuterRadius = _baseRadius;
            }
        }
        
        private void Update()
        {
            if (!breathingPulse || !controlSize)
            {
                return;
            }

            float interval = Mathf.Lerp(
                lowOxygenPulseInterval,
                normalPulseInterval,
                _airSupplyPercentage
            );

            _timer += Time.deltaTime;

            float wave = Mathf.Sin((_timer / interval) * Mathf.PI * 2f);
            float pulse = ((wave + 1f) / 2f) * pulseAmount;

            _lightSource.pointLightOuterRadius = _baseRadius + pulse;
        }
    }
}
