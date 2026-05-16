using System.Collections;
using UnityEngine;
using Managment;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeIntensity = 0.12f;

    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        EventManager.OnPlayerHit += Shake;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerHit -= Shake;
    }

    private void Shake()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(
            ShakeRoutine()
        );
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector2 randomOffset =
                Random.insideUnitCircle * shakeIntensity;

            transform.localPosition =
                _originalPosition +
                new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originalPosition;
    }
}