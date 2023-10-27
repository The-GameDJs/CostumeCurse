using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    [SerializeField] private AudioSource SummoningSound;
    [SerializeField] private CameraRig CameraRigComponent;
    [SerializeField] private ParticleSystem SummoningCloud;
    [SerializeField] private Animator Animator;

    public void ActivateWitchSummoning()
    {
        Animator.SetBool("IsSummoning", true);
        SummoningCloud.Play();
        SummoningSound.Play();
        CameraRigComponent.SetTargetGO(gameObject);
        CameraRigComponent.MoveCameraRelative(CameraRigComponent.DefaultOffset, CameraRigComponent.DefaultRotation);
    }

    public bool IsSummoning()
    {
        return Animator.GetBool("IsSummoning");
    }
}
