using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Effects.Camera
{
    public class CameraFeedbackManager : MonoBehaviour
    {
        [SerializeField] private MMF_Player damageFeedbackPlayer;

        private void OnEnable()
        {
            EventManager.OnPlayerHit += TriggerShake;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerHit -= TriggerShake;
        }

        private void TriggerShake()
        {
            Debug.Log("<color=orange>PLAYING FEEL FEEDBACK NOW!</color>");
            if (damageFeedbackPlayer)
            {
                damageFeedbackPlayer.PlayFeedbacks();
            }
        }
    }
}