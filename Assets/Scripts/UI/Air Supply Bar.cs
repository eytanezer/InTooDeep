using System;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class AirSupplyBar : EnemyController
    {
        
        [SerializeField] private Slider airSupplySlider;
        
        public void SetMaxAirSupply(float maxAirSupply)
        {
            airSupplySlider.maxValue = maxAirSupply;
        }
        
        public void SetAirSupply(float airSupply)
        {
            airSupplySlider.value = airSupply;
        }

        private void OnEnable()
        {
            EventManager.OnAirSupplyChanged += SetAirSupply;
            EventManager.OnMaxAirSupplyChanged += SetMaxAirSupply;
        }

        private void OnDisable()
        {
            EventManager.OnAirSupplyChanged -= SetAirSupply;
            EventManager.OnMaxAirSupplyChanged -= SetMaxAirSupply;
        }
    }
}