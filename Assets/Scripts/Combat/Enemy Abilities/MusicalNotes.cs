using System;
using UnityEngine;

public class MusicalNotes : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] MixParticles;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private Light emission;
    
    [SerializeField]
    private float Speed;

    [SerializeField]
    private float TargetVerticalOffset;

    private bool IsMoving;
    private Skelemusic SkeletonMusicAbility;
    private GameObject Target;
    private AllyCombatant _allyCombatant;

    private void Update()
    {
        if (IsMoving && Target != null && _allyCombatant != null)
        {
            CastMusicalNotesVfx();

            if (Input.GetButtonDown("Action Command"))
            {
                Debug.Log("Parry Button Pressed");
                _allyCombatant.HasParried = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            var allyCombatant = Target.GetComponent<AllyCombatant>();
            if (allyCombatant.HurtCollider == other)
            {
                IsMoving = false;
                SwitchMusicalNotesParticleSystemsState(false);
                SkeletonMusicAbility.DealSkelemusicDamage();
                ExplodeCandies();
                SetLight(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            if (_allyCombatant.ParryCollider == other && 
                !_allyCombatant.HasParried && 
                Input.GetButtonDown("Action Command"))
            {
                Debug.Log("Parried!");
                _allyCombatant.HasParriedCorrectly = true;
            }
            else if(_allyCombatant.HasParried || !_allyCombatant.HasParriedCorrectly)
            {
                Debug.Log($"Couldn't parry due because player pressed either missed or already pressed the parry button. Has Pressed Parry: {_allyCombatant.HasParried}");
            }
        }
    }

    public void SetAbility(Skelemusic ability)
    {
        SkeletonMusicAbility = ability;
    }

    public void SetLight(bool isActive = true)
    {
        emission.gameObject.SetActive(isActive);
    }
    
    public void SetTarget(GameObject target)
    {
        Target = target;
        _allyCombatant = target.GetComponent<AllyCombatant>();
    }

    private void CastMusicalNotesVfx()
    {
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + new Vector3(0.0f, TargetVerticalOffset, 0.0f), Speed * Time.deltaTime);
    }
    
    public void StartMoving()
    {
        IsMoving = true;
    }
    
    public void ResetVfx()
    {
        gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        emission.gameObject.SetActive(false);
    }
    
    public void SwitchMusicalNotesParticleSystemsState(bool activate = true)
    {
        foreach (var particleSystem in MixParticles)
        {
            if (activate)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Stop();
            }
        }
    }

    private void ExplodeCandies()
    {
        explosionParticles.Play();
    }
}
