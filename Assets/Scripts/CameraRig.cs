using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    private enum CameraPhase { InTransition, NoTransition };
    private CameraPhase CurrentPhase;
    private Camera MainCamera;
    
    private Timer Timer;

    // Current
    private bool IsRelativeToTargetGO;
    public float Smoothness;

    // Target (end of transition)
    private Vector3 TargetPosition;
    private Vector3 TargetOffset;
    private GameObject TargetGO;
    private Quaternion TargetRotation;

    // Initial (before transition)
    private Vector3 InitialPosition;
    private Quaternion InitialRotation;


    // Defaults
    [SerializeField]
    public Vector3 DefaultOffset;
    public Quaternion DefaultRotation =
        Quaternion.identity;
    [SerializeField]
    private readonly float DefaultSmoothness = 5f;

    private void Start()
    {
        IsRelativeToTargetGO = true;
        TargetGO = GameObject.Find("Sield");
        TargetOffset = DefaultOffset;
        InitialPosition = TargetPosition = TargetOffset + TargetGO.transform.position;
        InitialRotation = TargetRotation = DefaultRotation;
        this.transform.position = InitialPosition;
        this.transform.rotation = InitialRotation;
        Smoothness = DefaultSmoothness;

        Timer = GetComponent<Timer>();
        MainCamera = GetComponent<Camera>();
        CurrentPhase = CameraPhase.NoTransition;
    }

    // Moves independent to the CurrentGameObject
    public void MoveCameraAbsolute(Vector3 targetPosition, Quaternion targetRotation)
    {
        IsRelativeToTargetGO = false;

        InitialPosition = this.transform.position;
        InitialRotation = this.transform.rotation;

        TargetPosition = targetPosition;
        TargetRotation = targetRotation;

        CurrentPhase = CameraPhase.InTransition;
        Timer.StartTimer(Smoothness);
    }

    // Moves relative to the CurrentGameObject
    public void MoveCameraRelative(Vector3 targetOffset, Quaternion targetRotation)
    {
        IsRelativeToTargetGO = true;
        
        InitialPosition = this.transform.position;
        InitialRotation = this.transform.rotation;

        TargetOffset = targetOffset;
        TargetRotation = targetRotation;

        CurrentPhase = CameraPhase.InTransition;
        Timer.StartTimer(Smoothness);
    }

    public void SetTargetGO(GameObject targetObject)
    {
        TargetGO = targetObject;
    }

    public void SetTransitionSmoothness(float transitionSmoothness)
    {
        Smoothness = transitionSmoothness;
    }

    void LateUpdate()
    {
        switch(CurrentPhase)
        {
            case CameraPhase.NoTransition:
                NoTransitionUpdate();
                break;

            case CameraPhase.InTransition:
                InTransitionUpdate();
                break;
        }

    }

    private void InTransitionUpdate()
    {
        if (Timer.IsInProgress())
        {
            TargetPosition = IsRelativeToTargetGO ?
                TargetGO.transform.position + TargetOffset :
                TargetPosition;

            this.transform.position = Vector3.Lerp(InitialPosition,
                TargetPosition,
                Timer.GetProgress() / Smoothness);
            this.transform.rotation = Quaternion.Lerp(InitialRotation,
                TargetRotation,
                Timer.GetProgress() / Smoothness);
        }
        else
        {
            Debug.Log($"Done! {Timer.GetProgress()}");

            Timer.ResetTimer();

            CurrentPhase = CameraPhase.NoTransition;

            Smoothness = DefaultSmoothness; // revert transitional smoothness to the default
        }
    }

    private void NoTransitionUpdate()
    {
        TargetPosition = IsRelativeToTargetGO ?
                        TargetGO.transform.position + TargetOffset :
                        TargetPosition;

        this.transform.position = TargetPosition;
        this.transform.rotation = TargetRotation;
    }
}
