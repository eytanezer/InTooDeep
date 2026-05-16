using System;
using Managment.SoundScripts;
using UnityEngine;

namespace Effects
{
    public class Geyser : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private float volume;

        private void Start()
        {
            SoundManager.Instance.PlayLoopingSoundFX(clip, gameObject.transform, volume);
        }
    }
}