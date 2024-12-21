using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineCameraRig : MonoBehaviour
{
    private static CinemachineCameraRig _instance;
    public static CinemachineCameraRig Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CinemachineCameraRig>();
            }

            return _instance;
        }
    }

    [SerializeField] private CinemachineBrain CinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera DefaultCinemachineVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera _currentCinemachineCamera;
    [SerializeField] private CinemachineVirtualCamera _currentWorldCinemachineCamera;
    public CinemachineVirtualCamera CurrentCinemachineCamera => _currentCinemachineCamera;
    public CinemachineVirtualCamera CurrentWorldCinemachineCamera => _currentWorldCinemachineCamera;
    [SerializeField] private float CameraInitialBlendTime;

    private void Start()
    {
        _currentCinemachineCamera = DefaultCinemachineVirtualCamera;
        ChangeCinemachineBrainBlendTime(CameraInitialBlendTime);
    }

    public void SetCinemachineCamera(CinemachineVirtualCamera cm)
    {
        if (_currentCinemachineCamera.LookAt.parent != null && 
            _currentCinemachineCamera.LookAt.parent.name == "The Witch")
            return;
        
        if (_currentCinemachineCamera)
        {
            _currentCinemachineCamera.Priority = 10;
            _currentCinemachineCamera = null;
        }
        _currentCinemachineCamera = cm == null ? DefaultCinemachineVirtualCamera : cm;
        _currentCinemachineCamera.Priority = 11;
    }

    public void SetCinemachineCameraTarget(Transform target)
    {
        _currentCinemachineCamera.Follow = target;
        _currentCinemachineCamera.LookAt = target;
        
        /*var composer = _currentCinemachineCamera.GetCinemachineComponent<CinemachineComposer>();

        if (composer && composer.m_TrackedObjectOffset.y != offset)
        {
            var followOffset = composer.m_TrackedObjectOffset;
            followOffset.y = offset;
            composer.m_TrackedObjectOffset = followOffset;
        }*/
    }

    public void ChangeCinemachineBrainBlendTime(float blendTime)
    {
        CinemachineBrain.m_DefaultBlend.m_Time = blendTime;
    }

    public void SetCinemachineWorldCamera(CinemachineVirtualCamera currentCinemachineCamera)
    {
        _currentWorldCinemachineCamera = currentCinemachineCamera;
    }
}
