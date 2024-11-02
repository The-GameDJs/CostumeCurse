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
    private AllyCombatant _allyCombatant;


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
        _allyCombatant = target.GetComponent<AllyCombatant>();
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
        
        if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand && 
            Vector3.Distance(Target.transform.position, transform.position) >= 0.2f
            && Vector3.Distance(Target.transform.position, transform.position) <= 6.4f)
        {
            Debug.Log("Parried correctly!");
            _allyCombatant.ParrySound.Play();
            _allyCombatant.HasParriedCorrectly = true;
        }

        if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand)
        {
            Debug.Log("Parry Button Pressed");
            _allyCombatant.HasParried = true;
        }
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
