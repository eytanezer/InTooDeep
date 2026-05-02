using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;


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

        
        
        private Light2D  _lightSource;

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
            _lightSource.intensity = maxIntensity;
            _lightSource.pointLightOuterRadius = maxSize;
        }

        public void UpdateLight(float airSupplyPercentage)
        {
            if (controlAngles) {_lightSource.pointLightInnerAngle = Mathf.Lerp(minAngles, maxAngles, airSupplyPercentage);}
            if (controlIntensity) {_lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, airSupplyPercentage);}
            if(controlSize) {_lightSource.pointLightOuterRadius = Mathf.Lerp(minSize, maxSize, airSupplyPercentage);}
            
            
            
            // Debug.Log("Updating Lights: intensity:" +_lightSource.intensity + " radius:" +_lightSource.pointLightOuterRadius);
        }
        
        // public void DecreaseLight()
        // {
        //     if (_lightSource.intensity > minIntensity)
        //     {
        //         _lightSource.intensity = Mathf.Max(minIntensity, _lightSource.intensity - intensityIntervals);
        //     }
        //     if (_lightSource.pointLightOuterRadius > minSize)
        //     {
        //         _lightSource.pointLightOuterRadius = Mathf.Max(minSize, _lightSource.pointLightOuterRadius  - sizeIntervals);
        //     }
        //     
        //     Debug.Log("Decreasing Lights: intensity:" +_lightSource.intensity + " radius:" +_lightSource.pointLightOuterRadius);
        // }
        //
        // public void IncreaseLight()
        // {
        //     if (_lightSource.intensity < maxIntensity)
        //     {
        //         _lightSource.intensity = Mathf.Min(maxIntensity, _lightSource.intensity + intensityIntervals);
        //     }
        //     if (_lightSource.pointLightOuterRadius < maxSize)
        //     {
        //         _lightSource.pointLightOuterRadius = Mathf.Min(minSize, _lightSource.pointLightOuterRadius + sizeIntervals);
        //     }
        //     
        //     Debug.Log("Increasing Lights: intensity:" +_lightSource.intensity + " radius:" +_lightSource.pointLightOuterRadius);
        //
        // }
    }
}
