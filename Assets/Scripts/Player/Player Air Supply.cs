using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAirSupply : MonoBehaviour
    {
        [Header("Air supply parameters")]
        [SerializeField] private float airSupplyMax;
        [SerializeField] private float airLossSpeed;
        [SerializeField] private float airLossAmount;
        [SerializeField] private float airGainSpeed;
        
        [Header("Lights")]
        [SerializeField] private List<PlayerLightControler> lights;
        
    
        private float _currentAirSupply;
        private float _timePassed;
        private bool _underWater;
    
        public float CurrentAirSupply => _currentAirSupply;
        
        
        void Start()
        {
            ResetAirSupply();  
            _underWater = true;
        }

        void Update()
        {
            if (!_underWater)
            {
                IncreaseAirSupply();
            }
            else
            {
                _timePassed += Time.deltaTime;
                if (_timePassed >= airLossSpeed)
                {
                    DecreaseAirSupply();
                    _timePassed = 0;
                }
            }
            
        }

        private void ResetAirSupply()
        {
            _currentAirSupply = airSupplyMax;
            EventManager.RaiseAirSupplyChanged(_currentAirSupply);
        }

        private void DecreaseAirSupply()
        {
            if (_currentAirSupply <= 0){return;}
            
            _currentAirSupply -= airLossAmount;
            // Debug.Log("Decreasing air supply: " + _currentAirSupply);
            
            EventManager.RaiseAirSupplyChanged(_currentAirSupply);
            UpdateLights();
        }

        public bool UseAirSupply(float amount)
        {
            if (_currentAirSupply >= amount)
            {
                _currentAirSupply -= amount;
                EventManager.RaiseAirSupplyChanged(_currentAirSupply);
                UpdateLights();
                return true;
            }
            return false;
        }

        private void IncreaseAirSupply()
        {
            if (_currentAirSupply >= airSupplyMax) {return;}
            
            _currentAirSupply += (airLossAmount * airGainSpeed);
            // Debug.Log("Increasing air supply: " + _currentAirSupply);
            
            EventManager.RaiseAirSupplyChanged(_currentAirSupply);
            UpdateLights();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateLights()
        {
            float airSupplyPercentage = _currentAirSupply / airSupplyMax;
            foreach (PlayerLightControler lightControler in lights)
            {
                lightControler.UpdateLight(airSupplyPercentage);
            }
        }

        
    }
}
