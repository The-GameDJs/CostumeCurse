using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineCameraArea : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private Camera MainCamera;
    private CinemachineCameraRig CameraRigComponent;
    private void Start()
    {
        MainCamera = Camera.main;
        CameraRigComponent = MainCamera.GetComponent<CinemachineCameraRig>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Sield")
        {
            CameraRigComponent.SetCinemachineCamera(_cinemachineVirtualCamera);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Sield")
        {
            CameraRigComponent.SetCinemachineCamera(null);
        }
    }
}
