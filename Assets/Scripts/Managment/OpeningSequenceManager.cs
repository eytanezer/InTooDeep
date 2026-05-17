using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

namespace Managment
{
    [Serializable] 
    public struct SequencePoint
    {
        public Transform target;
        [TextArea(2, 4)] // This makes the text box bigger in the Unity Inspector
        public string textToType; 
    }
    
    public class OpeningSequenceManager : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineBrain cinemachineBrain;
        
        [Header("Light Settings")]
        [SerializeField] private Light2D globalLight;
        [SerializeField] private float lightIntensity = 0.6f;
        [SerializeField] private float normalIntensity = 0f;
        
        [Header("WayPoints")]
        [Tooltip("Element 0 should be the Player. The camera will tour the rest, then return to Element 0.")]
        [SerializeField] private List<SequencePoint> wayPoints;
        
        [Header("UI Settings")] // NEW
        [SerializeField] private TMP_Text sequenceText; // Drag your UI Text here
        [SerializeField] private TMP_Text skipIntroHint;
        [SerializeField] private InputActionReference Shift;
        [SerializeField] private float typeDuration = 1.0f; // How long it takes to type out the sentence
        [SerializeField] private float textFadeDuration = 0.5f;
        
        [Header("Movement Settings")]
        [SerializeField] private float travelTime = 1.5f;
        [SerializeField] private float lockTime = 0.5f;
        
        private Sequence _openingSequence;

        private void OnEnable()
        {
            EventManager.OnGameStateChanged += HandleGameState;
            Shift.action.Enable();
        }

        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= HandleGameState;
            Shift.action.Disable();
            _openingSequence?.Kill();
        }

        private void HandleGameState(GameManager.GameState state)
        {
            if (state == GameManager.GameState.OpeningSequence)
            {
                PlayOpeningMovie();
                CheckSkipIntro();
            }
        }
        
        private Vector3 GetPos(Transform target, float zPos)
        {
            return new Vector3(target.position.x, target.position.y, zPos);
        }

        private void PlayOpeningMovie()
        {
            skipIntroHint.gameObject.SetActive(true);
            Debug.Log("Playing Opening Movie");
            if (wayPoints == null || wayPoints.Count <= 0)
            {
                Debug.LogWarning("Sequence Manager has no waypoints! Skipping sequence.");
                skipIntroHint.gameObject.SetActive(false);
                EventManager.RaiseSequenceComplete();
                return;
            }
            
            if(cinemachineBrain) cinemachineBrain.enabled = false;
            
            float camZ = mainCamera.transform.position.z;
            mainCamera.transform.position = GetPos(wayPoints[1].target, camZ);
            
            _openingSequence?.Kill();
            
            _openingSequence = DOTween.Sequence();
            _openingSequence.SetUpdate(UpdateType.Late);
            
            // light on
            if(globalLight)
            {
                // globalLight.enabled = true;
                _openingSequence.Join(DOTween.To(() =>
                    globalLight.intensity, x => globalLight.intensity = x, lightIntensity, 1));
            }

            for (int i = 2; i < wayPoints.Count; i++)
            {
                if(!wayPoints[i].target) continue;
                
                int currentIndex = i; 
                
                _openingSequence.Append(mainCamera.transform.DOMove(GetPos(wayPoints[currentIndex].target, camZ), travelTime, false).SetEase(Ease.InOutSine));

                if (sequenceText && !string.IsNullOrWhiteSpace(wayPoints[currentIndex].textToType))
                {
                    // reset the text and hide
                    _openingSequence.AppendCallback(() => 
                    {
                        // Use currentIndex here instead of i!
                        sequenceText.text = wayPoints[currentIndex].textToType; 
                        sequenceText.maxVisibleCharacters = 0; 
                        sequenceText.alpha = 1f; 
                    });
     
                    // typewriter effect
                    _openingSequence.Append(DOTween.To(
                        () => sequenceText.maxVisibleCharacters, 
                        x => sequenceText.maxVisibleCharacters = x, 
                        wayPoints[currentIndex].textToType.Length, // Use currentIndex here!
                        typeDuration).SetEase(Ease.Linear));
    
                    //wait
                    _openingSequence.AppendInterval(lockTime*2);
    
                    // fadeout
                    _openingSequence.Append(DOTween.To(
                        () => sequenceText.alpha, 
                        x => sequenceText.alpha = x, 
                        0f, 
                        textFadeDuration));
                }
                
                _openingSequence.AppendInterval(lockTime*0.75f);
            }
            
            //return to player
            if (wayPoints.Count > 1) _openingSequence.Append(mainCamera.transform.DOMove(GetPos(wayPoints[0].target, camZ), travelTime).SetEase(Ease.InOutSine));
            
            //light off
            if(globalLight)
            {
                _openingSequence.Join(DOTween.To(() =>
                    globalLight.intensity, x => globalLight.intensity = x, normalIntensity, 1));
                // globalLight.enabled = false;
            }
            
            _openingSequence.OnComplete(() =>
            {
                if(cinemachineBrain) cinemachineBrain.enabled = true;
                EventManager.RaiseSequenceComplete();
                skipIntroHint.gameObject.SetActive(false);
            });
        }
        
        void CheckSkipIntro()
        {
            if (Shift.action.WasPressedThisFrame())
            {
                Debug.Log("pressed shift to skip intro");
                wayPoints.Clear();
                skipIntroHint.gameObject.SetActive(false);
            }
        }
    }
}