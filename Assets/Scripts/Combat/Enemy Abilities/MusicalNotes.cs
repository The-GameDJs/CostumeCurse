using System;
using UnityEngine;
using Assets.Scripts.Combat;

public class MusicalNotes : MonoBehaviour
{
    
    [SerializeField] private ParticleSystem[] MixParticles;
    [SerializeField] private ParticleSystem SprinklesParticles;
    [SerializeField] private Material ParticlesMaterial;
    
    [SerializeField] private Light emission;
    [SerializeField] private float Speed;
    [SerializeField] private float TargetVerticalOffset;

    private bool IsMoving;
    private Skelemusic SkeletonMusicAbility;
    private GameObject Target;
    private AllyCombatant _allyCombatant;
    private ElementType MusicType;

    private void Update()
    {
        if (IsMoving && Target != null && _allyCombatant != null)
        {
            CastMusicalNotesVfx();

            if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand)
            {
                Debug.Log($"Couldn't parry due because player pressed either missed or already pressed the parry button. Has Pressed Parry: {_allyCombatant.HasParried}");
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
                //xplodeCandies();
                SetLight(false);
            }
        }
    }

    public void SetType(ElementType type)
    {
        MusicType = type;
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

        CheckIfParried();
    }

    public void CheckIfParried()
    {
        if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand && 
            Vector3.Distance(Target.transform.position, transform.position) >= 0.0001f
            && Vector3.Distance(Target.transform.position, transform.position) <= 4.2f)
        {
            Debug.Log("Parried correctly!");
            _allyCombatant.HasParriedCorrectly = true;
        }
    }
    
    public void CheckIfParriedSecondTime()
    {
        if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand)
        {
            Debug.Log("Parried correctly!");
            _allyCombatant.HasParriedCorrectly = true;
        }
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
        for (var index = 0; index < MixParticles.Length; index++)
        {
            var particleSystem = MixParticles[index];
            var trails = particleSystem.trails;
            if (activate)
            {
                particleSystem.Play();
                if (particleSystem.TryGetComponent<ParticleSystemRenderer>(out var psr))
                {
                    psr.trailMaterial = ParticlesMaterial;
                }
            }
            else
            {
                particleSystem.Stop();
            }

            trails.enabled = activate;
        }

        if (activate)
        {
            SprinklesParticles.Play();
        }
        else
        {
            SprinklesParticles.Stop();
        }
    }

    private void ExplodeCandies()
    {
        //explosionParticles.Play();
    }
}
