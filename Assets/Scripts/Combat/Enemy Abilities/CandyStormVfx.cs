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
    [SerializeField] private CandyStorm CandyStormAbility;
    [SerializeField] private AudioSource StormExplosionSound;
    private bool IsMoving;
    private GameObject Target;


    private void Start()
    {
        // Make sure it is assigned on the inspector!
        if (!CandyStormAbility)
        {
            CandyStormAbility = FindObjectOfType<CandyStorm>();
        }
    }

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
            StartCoroutine(CandyStormAbility.DealCandyStormDamage());
            StormExplosionSound.Play();
        }
    }

    public void SwitchVfxActivation(bool activate = true)
    {
        CloudObject.SetActive(activate);
        CandyObject.SetActive(activate);
    }

    public void SetComponents(GameObject target)
    {
        Target = target;
    }
    
    public void StartMoving()
    {
        IsMoving = true;
    }
    
    public void ResetVfx()
    {
        gameObject.transform.position = Vector3.zero;
        SwitchCloudStormParticleSystemsState(false);
        gameObject.SetActive(false);
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

    public void ExplodeCandyStormMix()
    {
        ExplosionParticles.Play();
    }
}
