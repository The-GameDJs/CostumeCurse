using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArea : MonoBehaviour
{
    [SerializeField]
    private Vector3 Offset;
    [SerializeField]
    private Vector3 Rotation;
    [SerializeField]
    private float TransitionInSmoothness;
    [SerializeField]
    private float TransitionOutSmoothness;

    GameObject MainCamera;
    CameraRig CameraRig;

    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CameraRig = MainCamera.GetComponent<CameraRig>();        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            CameraRig.SetTransitionSmoothness(TransitionInSmoothness);
            CameraRig.MoveCameraRelative(Offset, Quaternion.Euler(Rotation));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraRig.SetTransitionSmoothness(TransitionOutSmoothness);
            CameraRig.MoveCameraRelative(CameraRig.DefaultOffset,
                CameraRig.DefaultRotation);
        }
    }

    private void OnDestroy()
    {
        CameraRig.SetTransitionSmoothness(TransitionOutSmoothness);
        CameraRig.MoveCameraRelative(CameraRig.DefaultOffset,
            CameraRig.DefaultRotation);
    }
}
