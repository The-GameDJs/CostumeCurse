using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineCameraRig : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera DefaultCinemachineVirtualCamera;
    private CinemachineVirtualCamera _currentCinemachineCamera;

    private void Start()
    {
        _currentCinemachineCamera = DefaultCinemachineVirtualCamera;
    }

    public void SetCinemachineCamera(CinemachineVirtualCamera cm)
    {
        if (_currentCinemachineCamera)
        {
            _currentCinemachineCamera.Priority = 10;
            _currentCinemachineCamera = null;
        }
        _currentCinemachineCamera = cm == null ? DefaultCinemachineVirtualCamera : cm;
        _currentCinemachineCamera.Priority = 11;
    }
}
