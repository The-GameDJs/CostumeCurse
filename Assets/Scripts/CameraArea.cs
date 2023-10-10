using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArea : MonoBehaviour
{
    [SerializeField] public Vector3 Offset;
    [SerializeField] public Vector3 Rotation;
    [SerializeField] private float TransitionInSmoothness;
    [SerializeField] private float TransitionOutSmoothness;

    GameObject MainCamera;
    CameraRig CameraRig;
    private CombatSystem CombatSystem;

    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CameraRig = MainCamera.GetComponent<CameraRig>();
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Sield")
        {
            SetCameraMovement(TransitionInSmoothness, Offset, Quaternion.Euler(Rotation));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Sield")
        {
            SetCameraMovement(TransitionOutSmoothness, CameraRig.DefaultOffset, CameraRig.DefaultRotation);
        }
    }

    private void SetCameraMovement(float transitionSmoothness, Vector3 offset, Quaternion rotation)
    {
        CameraRig.SetTransitionSmoothness(transitionSmoothness);
        CameraRig.MoveCameraRelative(offset, rotation);
    }

    public void OnDestroy()
    {
        if (CameraRig == null)
            return;

        if (GetComponent<CombatZone>() == null)
        {
            CameraRig.SetTransitionSmoothness(TransitionOutSmoothness);
            CameraRig.MoveCameraRelative(CameraRig.DefaultOffset,
                CameraRig.DefaultRotation);
        }

    }
}
