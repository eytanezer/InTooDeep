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
            if (controlAngles) {_lightSource.pointLightOuterAngle = Mathf.Lerp(minAngles, maxAngles, airSupplyPercentage);}
            if (controlIntensity) {_lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, airSupplyPercentage);}
            if(controlSize) {_lightSource.pointLightOuterRadius = Mathf.Lerp(minSize, maxSize, airSupplyPercentage);}
        }
    }
}
