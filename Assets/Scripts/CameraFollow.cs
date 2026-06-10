using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    public void SetTarget(Transform target)
    {
        cinemachineCamera.Target.TrackingTarget = target;
        cinemachineCamera.Target.LookAtTarget = target;
    }
}