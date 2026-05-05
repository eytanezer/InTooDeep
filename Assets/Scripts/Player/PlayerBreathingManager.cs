using Management.SoundScripts;
using UnityEngine;
using Managment;
using Managment.SoundScripts;

namespace Player
{
    public class PlayerBreathingManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioClip breathingClip;
        // [SerializeField] private AudioClip fillingAirClip;
        [SerializeField] private float breathingVolume = 1f;

        private AudioSourcePoolable _breathingSource;
        // private AudioSourcePoolable _fillingAirSource;
        private bool _isUnderwater = true; 

        private void Start()
        {
            _breathingSource = SoundManager.Instance.PlayLoopingSoundFX(breathingClip, transform, breathingVolume);
            // _fillingAirSource = SoundManager.Instance.PlayLoopingSoundFX(fillingAirClip, transform, breathingVolume);
            
            if (_breathingSource)  _breathingSource.Source.Pause();
            // if (_fillingAirSource)  _breathingSource.Source.Pause();
            
        }

        private void OnEnable()
        {
            EventManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= HandleGameStateChanged;

            if (_breathingSource && _breathingSource.Source)
            {
                _breathingSource.Source.Stop();
            }
        }


        private void HandleGameStateChanged(GameManager.GameState state)
        {
            if (!_breathingSource) return;

            if (state == GameManager.GameState.Gameplay)
            {
                if (_isUnderwater && !_breathingSource.Source.isPlaying)
                {
                    _breathingSource.Source.Play();
                }
            }
            else 
            {
                if (_breathingSource.Source.isPlaying)
                {
                    _breathingSource.Source.Pause();
                }
            }
        }


        public void SetUnderwaterStatus(bool isUnderwater)
        {
            _isUnderwater = isUnderwater;
            if (!_breathingSource) return;

            if (_isUnderwater && GameManager.Instance.CurrentState == GameManager.GameState.Gameplay)
            {
                Debug.Log("Underwater");
                // _fillingAirSource.Source.Pause();
                _breathingSource.Source.Play();
            }
            else
            {
                Debug.Log("Not underwater");
                _breathingSource.Source.Pause();
                // _fillingAirSource.Source.Play();
            }
        }
    }
}