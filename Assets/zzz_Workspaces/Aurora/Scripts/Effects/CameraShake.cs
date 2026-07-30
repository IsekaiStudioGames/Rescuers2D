using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShake : Singleton<CameraShake>{

    [SerializeField] private float globalShakeForce = 1f;

    public void CameraShaking(CinemachineImpulseSource impulseSource) {

        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}