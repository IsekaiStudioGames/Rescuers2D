using Unity.Cinemachine;
using UnityEngine;

public class Ground : MonoBehaviour {

    private CinemachineImpulseSource impulseSource;

    private void Start() {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void OnTriggerEnter2D(Collider2D other) {

        CameraShake.Instance.CameraShaking(impulseSource);
    }
}