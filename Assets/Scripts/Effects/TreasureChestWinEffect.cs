using System.Collections;
using Managment;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TreasureChestWinEffect : MonoBehaviour
{
    [SerializeField] private MMF_Player chestFeelPlayer;
    [SerializeField] private ParticleSystem sparkles;
    [SerializeField] private float effectDuration = 3f;

    private Coroutine _effectCoroutine;

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.WinSequence)
        {
            PlayWinEffect();
        }
    }

    private void PlayWinEffect()
    {
        if (_effectCoroutine != null)
        {
            StopCoroutine(_effectCoroutine);
        }

        _effectCoroutine = StartCoroutine(WinEffectRoutine());
    }

    private IEnumerator WinEffectRoutine()
    {
        if (chestFeelPlayer != null)
        {
            chestFeelPlayer.PlayFeedbacks();
        }

        if (sparkles != null)
        {
            sparkles.Play();
        }

        yield return new WaitForSeconds(effectDuration);

        if (sparkles != null)
        {
            sparkles.Stop();
        }

        _effectCoroutine = null;
    }
}