using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeAmplitude = 2f;
    [SerializeField] private float shakeFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin _noise;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _noise = GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (_noise == null)
        {
            Debug.LogError("CameraShake: No CinemachineBasicMultiChannelPerlin found on this object");
            return;
        }

        Debug.Log("CameraShake: Noise found");
        StopShake();
    }

    public void Shake()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        _noise.AmplitudeGain = shakeAmplitude;
        _noise.FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(shakeDuration);

        StopShake();
    }

    private void StopShake()
    {
        if (_noise == null)
        {
            return;
        }

        _noise.AmplitudeGain = 0f;
        _noise.FrequencyGain = 0f;
    }
}