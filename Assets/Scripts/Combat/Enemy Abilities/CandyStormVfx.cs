using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Enemy_Abilities;
using UnityEngine;

public class CandyStormVfx : MonoBehaviour
{
    [Header("Particle Systems")] 
    [SerializeField] private GameObject CloudObject;
    [SerializeField] private GameObject CandyObject;
    
    [SerializeField] private ParticleSystem ThunderCloudParticles;
    [SerializeField] private ParticleSystem ThunderCandyClusterParticles;
    [SerializeField] private ParticleSystem ThunderSprinkerClusterParticles;
    [SerializeField] private ParticleSystem ExplosionParticles;

    [Header("Movement")]
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float TargetVerticalOffset;
    private CandyStorm CandyStormAbility;
    private bool IsMoving;
    private GameObject Target;

    private void Update()
    {
        if (IsMoving && Target)
        {
            StrikeCandyStormVfx();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            IsMoving = false;
            SwitchCloudStormParticleSystemsState(false);
            //StartCoroutine(CandyStormAbility.DealCandyStormDamage());
        }
    }

    public void SwitchVfxActivation(bool activate = true)
    {
        CloudObject.SetActive(activate);
        CandyObject.SetActive(activate);
    }
    
    public void SetTarget(GameObject target)
    {
        Target = target;
    }
    
    public void StartMoving()
    {
        IsMoving = true;
    }
    
    public void ResetVfx()
    {
        gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void StrikeCandyStormVfx()
    {
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + new Vector3(0.0f, TargetVerticalOffset, 0.0f), MovementSpeed * Time.deltaTime);
    }
    
    public void SwitchCloudStormParticleSystemsState(bool activate = true)
    {
        if (activate)
        {
            ThunderCloudParticles.Play();
            ThunderCandyClusterParticles.Play();
            ThunderSprinkerClusterParticles.Play();
        }
        else
        {
            ThunderCloudParticles.Stop();
            ThunderCandyClusterParticles.Stop();
            ThunderSprinkerClusterParticles.Stop();
        }
    }

    public void ExplodeConfectionMix()
    {
        ExplosionParticles.Play();
    }
}
