using Unity.Cinemachine;
using UnityEngine;

public class PlayerHitShake : MonoBehaviour
{
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource =
            GetComponent<CinemachineImpulseSource>();
        if (_impulseSource == null) Debug.LogError(gameObject.name + ": No Cinemachine ImpulseSource attached!");
    }

    public void ShakeCamera()
    {
        _impulseSource.GenerateImpulse();
    }
}