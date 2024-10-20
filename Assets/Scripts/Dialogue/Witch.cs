using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Witch : MonoBehaviour
{
    [SerializeField] private AudioSource SummoningSound;
    [SerializeField] private ParticleSystem SummoningCloud;
    [SerializeField] private Animator Animator;
    [SerializeField] private CinemachineVirtualCamera CinemachineCamera;

    public void ActivateWitchSummoning()
    {
        Animator.SetBool("IsSummoning", true);
        SummoningCloud.Play();
        SummoningSound.Play();
        CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(3.0f);
        CinemachineCameraRig.Instance.SetCinemachineCamera(CinemachineCamera);
    }

    public bool IsSummoning()
    {
        return Animator.GetBool("IsSummoning");
    }
}
