using System.Collections.Generic;
using Managment;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;


namespace Player
{
    public class PlayerAirSupply : MonoBehaviour
    {
        [Header("Air supply parameters")] [SerializeField]
        private float airSupplyMax;

        [SerializeField] private float airLossSpeed;
        [SerializeField] private float airLossAmount;
        [SerializeField] private float airGainSpeed;
        

        [Header("Lights")] [SerializeField] private List<PlayerLightControler> lights;

        [Header("MMFeedbacks")] [SerializeField]
        private MMProgressBar airProgressBar;

        [SerializeField] private MMF_Player dashDrainFeedback; // 2. Add your Feedback slot
        [SerializeField] private MMF_Player lowAirWarningFeedback;


        private float _currentAirSupply;
        private float _timePassed;
        private bool _underWater;
        private PlayerBreathingManager _breathingManager;
        
        private bool _breathingEnabled;

        public float CurrentAirSupply => _currentAirSupply;

        void Awake()
        {
            _breathingManager = GetComponent<PlayerBreathingManager>();
        }

        void Start()
        {
            ResetAirSupply();
            _underWater = true;
        }

        void OnEnable()
        {
            EventManager.OnStartNewRun += ResetAirSupply;
            EventManager.OnGameStateChanged += HandleGameStateChanged;
        }

        void OnDisable()
        {
            EventManager.OnStartNewRun -= ResetAirSupply;
            EventManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        
        private void HandleGameStateChanged(GameManager.GameState state)
        {
            _breathingEnabled = state == GameManager.GameState.Gameplay;
        }

        void Update()
        {
            if (!_breathingEnabled) return;
            if (!_underWater)
            {
                IncreaseAirSupply();
                _timePassed = 0;
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
            EventManager.RaiseAirSupplyChanged(1);
        }

        private void DecreaseAirSupply()
        {
            if (_currentAirSupply <= 0)
            {
                return;
            }

            _currentAirSupply = Mathf.Max(0,  _currentAirSupply - airLossAmount) ;
            // Debug.Log("Decreasing air supply: " + _currentAirSupply);

            EventManager.RaiseAirSupplyChanged(_currentAirSupply / airSupplyMax);
            UpdateVisuals();

            if (_currentAirSupply < (airSupplyMax * 0.25f))
            {
                // lowAirWarningFeedback?.PlayFeedbacks();
            }

            if (_currentAirSupply <= 0)
            {
                Debug.Log("NO AIR!!!");
                EventManager.RaiseLoseGame();
            }
        }

        public bool UseAirSupply(float amount)
        {
            if (_currentAirSupply >= amount)
            {
                _currentAirSupply -= amount;
                EventManager.RaiseAirSupplyChanged(_currentAirSupply / airSupplyMax);
                UpdateVisuals();

                if (_currentAirSupply <= 0)
                {
                    Debug.Log("NO AIR!!!");
                    EventManager.RaiseLoseGame();
                }
                
                return true;
            }
            
            return false;
        }

        private void IncreaseAirSupply()
        {
            if (_currentAirSupply >= airSupplyMax)
            {
                return;
            }

            _currentAirSupply = Mathf.Min((airLossAmount * airGainSpeed * Time.deltaTime) + _currentAirSupply,  airSupplyMax) ;
            
            // Debug.Log("Increasing air supply: " + _currentAirSupply);

            EventManager.RaiseAirSupplyChanged(_currentAirSupply / airSupplyMax);
            UpdateVisuals();

        }

        private void UpdateVisuals()
        {
            UpdateLights();
            UpdateAirUI();
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

        private void UpdateAirUI()
        {
            if (airProgressBar != null)
            {
                airProgressBar.UpdateBar(_currentAirSupply, 0f, airSupplyMax);
            }
        }
        
        public void SetUnderWater(bool underWater)
        {
            _underWater = underWater;
            _currentAirSupply = Mathf.Round(_currentAirSupply);
             EventManager.RaiseAirSupplyChanged(_currentAirSupply / airSupplyMax);
             UpdateVisuals();
             
             if (_breathingManager) _breathingManager.SetUnderwaterStatus(underWater);
        }
    }
}
