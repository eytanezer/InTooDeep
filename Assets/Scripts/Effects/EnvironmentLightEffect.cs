using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Effects
{
    [RequireComponent(typeof(Light2D))]
    public class EnvironmentLightEffect : MonoBehaviour
    {
        private Light2D _light;
        private float _initialIntensity;
        private float _initialRadius;
        private float _randomTimeOffset;
        
        [Header("Flicker Effect")]
        [SerializeField] bool enableFlicker = false;
        [SerializeField] float flickerSpeed = 5f;
        [SerializeField] float flickerIntensityVariation = 0.5f;
       
        
        [Header("Pulse Effect")]
        [SerializeField] bool enablePulse = false;
        [SerializeField] float pulseSpeed = 2f;
        [SerializeField] float pulseIntensityVariation = 1;
        [SerializeField] float pulseSizeVariation = 0.2f;
        
        // [Tooltip("Defines the pulse shape. X-axis is time (0 to 1), Y-axis is multiplier (-1 to 1)")]
        // [SerializeField] AnimationCurve pulseCurve = new AnimationCurve(new Keyframe(0f, 0f),       // Start at 0
        //     new Keyframe(0.25f, 1f),    // Peak positive at 25%
        //     new Keyframe(0.75f, -1f),   // Peak negative at 75%
        //     new Keyframe(1f, 0f));

        
        void  Awake()
        {
            _light = GetComponent<Light2D>();
            _initialIntensity = _light.intensity;
            _initialRadius = _light.pointLightOuterRadius;
            _randomTimeOffset = Random.Range(0f, 100f);
        }

        private void Update()
        {
            float targetIntensity = _initialIntensity;
            float targetRadius = _initialRadius;

            if (enableFlicker)
            {
                float noise = Mathf.PerlinNoise(_randomTimeOffset * Time.time * flickerSpeed, 0f);
                
                targetIntensity += (noise - 0.5f) * flickerIntensityVariation * 2f;
            }

            if (enablePulse)
            {
                // float looptime = Mathf.Repeat((Time.time + _randomTimeOffset) * pulseSpeed, 1f);
                //
                // float curveValue = pulseCurve.Evaluate(looptime);
                //
                float curveValue = Mathf.Sin((Time.time + _randomTimeOffset) * pulseSpeed);
                targetRadius += curveValue * pulseSizeVariation;
                targetIntensity += curveValue * pulseIntensityVariation;
            }
            
            _light.intensity = Mathf.Max(0f, targetIntensity);
            _light.pointLightOuterRadius = Mathf.Max(0.1f, targetRadius);
        }
    }
}
