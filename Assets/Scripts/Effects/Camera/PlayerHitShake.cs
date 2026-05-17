using Unity.Cinemachine;
using UnityEngine;

public class PlayerHitShake : MonoBehaviour
{
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource =
            GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera()
    {
        _impulseSource.GenerateImpulse();
    }
}