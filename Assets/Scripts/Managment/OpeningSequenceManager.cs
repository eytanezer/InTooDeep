using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        [SerializeField] private float typeDuration = 1.0f; // How long it takes to type out the sentence
        [SerializeField] private float textFadeDuration = 0.5f;
        
        [Header("Movement Settings")]
        [SerializeField] private float travelTime = 1.5f;
        [SerializeField] private float lockTime = 0.5f;

        private void OnEnable() => EventManager.OnGameStateChanged += HandleGameState;
        private void OnDisable() => EventManager.OnGameStateChanged -= HandleGameState;
        
        
        private void HandleGameState(GameManager.GameState state)
        {
            if (state == GameManager.GameState.OpeningSequence)
            {
                PlayOpeningMovie();
            }
        }
        
        private Vector3 GetPos(Transform target, float zPos)
        {
            return new Vector3(target.position.x, target.position.y, zPos);
        }

        private void PlayOpeningMovie()
        {
            Debug.Log("Playing Opening Movie");
            if (wayPoints == null || wayPoints.Count <= 0)
            {
                Debug.LogWarning("Sequence Manager has no waypoints! Skipping sequence.");
                EventManager.RaiseSequenceComplete();
                return;
            }
            
            if(cinemachineBrain) cinemachineBrain.enabled = false;
            
            float camZ = mainCamera.transform.position.z;
            
            mainCamera.transform.position = GetPos(wayPoints[0].target, camZ);
            
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(UpdateType.Late);
            
            // light on
            if(globalLight)
            {
                // globalLight.enabled = true;
                sequence.Join(DOTween.To(() =>
                    globalLight.intensity, x => globalLight.intensity = x, lightIntensity, 1));
            }

            for (int i = 1; i < wayPoints.Count; i++)
            {
                if(!wayPoints[i].target) continue;
                
                // CRITICAL FIX: Cache the value of 'i' so the DOTween callback remembers it!
                int currentIndex = i; 
                
                sequence.Append(mainCamera.transform.DOMove(GetPos(wayPoints[currentIndex].target, camZ), travelTime, false).SetEase(Ease.InOutSine));

                if (sequenceText && !string.IsNullOrWhiteSpace(wayPoints[currentIndex].textToType))
                {
                    // reset the text and hide
                    sequence.AppendCallback(() => 
                    {
                        // Use currentIndex here instead of i!
                        sequenceText.text = wayPoints[currentIndex].textToType; 
                        sequenceText.maxVisibleCharacters = 0; 
                        sequenceText.alpha = 1f; 
                    });
    
                    // typewriter effect
                    sequence.Append(DOTween.To(
                        () => sequenceText.maxVisibleCharacters, 
                        x => sequenceText.maxVisibleCharacters = x, 
                        wayPoints[currentIndex].textToType.Length, // Use currentIndex here!
                        typeDuration).SetEase(Ease.Linear));
    
                    //wait
                    sequence.AppendInterval(lockTime*2);
    
                    // fadeout
                    sequence.Append(DOTween.To(
                        () => sequenceText.alpha, 
                        x => sequenceText.alpha = x, 
                        0f, 
                        textFadeDuration));
                }
                
                sequence.AppendInterval(lockTime);
            }
            
            // for (int i = 1; i < wayPoints.Count; i++)
            // {
            //     if(!wayPoints[i].target) continue;
            //     
            //     sequence.Append(mainCamera.transform.DOMove(GetPos(wayPoints[i].target, camZ), travelTime, false).SetEase(Ease.InOutSine));
            //
            //     if (sequenceText && !string.IsNullOrWhiteSpace(wayPoints[i].textToType))
            //     {
            //         // reset the text and hide
            //         sequence.AppendCallback(() => 
            //         {
            //             sequenceText.text = wayPoints[i].textToType; 
            //             sequenceText.maxVisibleCharacters = 0; // Hides everything
            //             sequenceText.alpha = 1f; 
            //         });
            //
            //         // typewriter effect
            //         sequence.Append(DOTween.To(
            //             () => sequenceText.maxVisibleCharacters, 
            //             x => sequenceText.maxVisibleCharacters = x, 
            //             wayPoints[i].textToType.Length, 
            //             typeDuration).SetEase(Ease.Linear));
            //
            //         //wait
            //         sequence.AppendInterval(lockTime);
            //
            //         // fadeout
            //         sequence.Append(DOTween.To(
            //             () => sequenceText.alpha, 
            //             x => sequenceText.alpha = x, 
            //             0f, 
            //             textFadeDuration));
            //     }
            //     
            //     sequence.AppendInterval(lockTime);
            // }
            
            
            
            
            //return to player
            if (wayPoints.Count > 1) sequence.Append(mainCamera.transform.DOMove(GetPos(wayPoints[0].target, camZ), travelTime).SetEase(Ease.InOutSine));
            
            //light off
            if(globalLight)
            {
                sequence.Join(DOTween.To(() =>
                    globalLight.intensity, x => globalLight.intensity = x, normalIntensity, 1));
                // globalLight.enabled = false;
            }
            
            sequence.OnComplete(() =>
            {
                if(cinemachineBrain) cinemachineBrain.enabled = true;
                EventManager.RaiseSequenceComplete();
            });
        }
    }
}