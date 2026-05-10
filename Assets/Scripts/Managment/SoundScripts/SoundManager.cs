using System.Collections;
using DG.Tweening;
using Management.SoundScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managment.SoundScripts
{
    /// <summary>
    /// singleton class to manage all sound effects and background music
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager >
    {
        [SerializeField] private AudioSource soundFXObject;
        // [SerializeField] private AudioClip backgroundMusic;
        [Header("Background Music")]
        [SerializeField] private AudioClip titleBGM;
        [SerializeField] private AudioClip playingBGM;
        [SerializeField] private AudioClip gameOverBGM;
        
        [SerializeField] private float fadeDuration = 1f;

        private AudioPool _audioPool;
        private AudioSourcePoolable _currentBgmSource;

        private void Awake()
        {
            _audioPool = AudioPool.Instance;
        }

        private void Start()
        {
            PlayBGM(titleBGM);
        }
        
        /// <summary>
        /// begin playing background music
        /// </summary>
        private void PlayBGM(AudioClip newClip)
        {
            if (!newClip) return;
            // it's already playing this exact clip, do nothing
            if (_currentBgmSource && _currentBgmSource.Source.clip == newClip && _currentBgmSource.Source.isPlaying) return;

            // save old track
            AudioSourcePoolable oldBGM = _currentBgmSource;
            
            _currentBgmSource = _audioPool.Get();
            _currentBgmSource.transform.SetParent(transform);
            
            _currentBgmSource.Source.clip = newClip;
            _currentBgmSource.Source.loop = true;
            _currentBgmSource.Source.spatialBlend = 0;
            
            _currentBgmSource.Source.volume = 0f;
            _currentBgmSource.Source.Play();
            _currentBgmSource.Source.DOFade(0.3f, fadeDuration); // fade in new track
            
            if(oldBGM) // fade out old track and return to pool
            {
                oldBGM.Source.DOFade(0f, fadeDuration/2).OnComplete(() =>
                {
                    oldBGM.Source.Stop();
                    _audioPool.Return(oldBGM);
                }); 
            }
        }
        
        /// <summary>
        /// Plays a looping sound attached to a specific object (Great for Cars)
        /// </summary>
        public AudioSourcePoolable PlayLoopingSoundFX(AudioClip clip, Transform targetTransform, float volume)
        {
            if (!clip) return null;

            AudioSourcePoolable audioSourcePoolable = _audioPool.Get();
            AudioSource source = audioSourcePoolable.Source;
            
            // Attach the audio object to the car so it moves with it
            audioSourcePoolable.transform.SetParent(targetTransform);
            audioSourcePoolable.transform.localPosition = Vector3.zero;
            
            source.clip = clip;
            source.volume = volume;
            source.loop = true;
            source.spatialBlend = 1; // 1 = 3D (Pan left/right as car moves)
            source.pitch = Random.Range(0.9f, 1.1f); // Slight variation so cars don't sound identical
            
            source.Play();
            
            return audioSourcePoolable; // Return this so the car can give it back to the pool later!
        }
        
        /// <summary>
        /// play a sound effect clip at a given transform location with specified volume
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="spawnTransform"></param>
        /// <param name="volume"></param>
        public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, float customLength = -1)
        {
            // spawn in gameObject
            AudioSourcePoolable audioSourcePoolable = _audioPool.Get();
            AudioSource source = audioSourcePoolable.Source;
            
            // set position
            audioSourcePoolable.transform.position = spawnTransform.position;
            
            // assign audioClip
            source.clip = clip;
            // assign volume
            source.volume = volume;
            // source.pitch = Random.Range(0.8f, 1.2f);
            source.pitch = 1;
            source.spatialBlend = 0;
            
            source.Play();
            
            //get length of clip
            float clipLength = (customLength > 0) ? customLength : source.clip.length;
            // return to pool after clip finished playing
            StartCoroutine(ReturnToPool(audioSourcePoolable, clipLength));
        }

        private IEnumerator ReturnToPool(AudioSourcePoolable audioSourcePoolable, float delay)
        {
            yield return new WaitForSeconds(delay);
            _audioPool.Return(audioSourcePoolable);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StopAllSounds()
        {
            AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource source in allAudioSources)
            {
                source.Stop();
            }
        }
        
        private void OnEnable()
        {
            // EventManager.OnStartGame += TriggerTitleMusic;
            EventManager.OnStartGame += TriggerGameplayMusic;
            // EventManager.OnPauseGame += ;
            // EventManager.OnResumeGame += ;
            EventManager.OnResetGame += TriggerGameplayMusic;
            EventManager.OnGameOver += TriggerGameOverMusic;
        }

        private void OnDisable()
        {
            // EventManager.OnStartGame -= TriggerTitleMusic;
            EventManager.OnStartGame -= TriggerGameplayMusic;
            // EventManager.OnPauseGame -= ;
            // EventManager.OnResumeGame -= ;
            EventManager.OnGameOver -= TriggerGameOverMusic;
        }
        
        // Small helper methods for the events to trigger
        private void TriggerGameplayMusic() => PlayBGM(playingBGM);
        private void TriggerGameOverMusic() => PlayBGM(gameOverBGM);
        private void TriggerTitleMusic() => PlayBGM(titleBGM);
        
    }
}