using System;
using UnityEngine;

public class ConfectionVfx : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] MixParticles;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float TargetVerticalOffset;

    [SerializeField]
    private AudioSource ConfectionSound;

    private bool IsMoving;
    private Confection GanielConfection;
    private GameObject Target;
    private int TotalInputs;

    private void Start()
    {
        GanielConfection = GameObject.Find("Ganiel").GetComponentInChildren<Confection>();
        Confection.ShowConfectionParticle += OnPerfectClick;
    }

    public void ActivateVfx()
    {
        var particleSystem = MixParticles[0];
        particleSystem.Play();
        ConfectionSound.Play();
    }
    
    private void OnPerfectClick(int perfectClicks)
    {
        var particleSystem = MixParticles[perfectClicks - 1];
        particleSystem.Play();
    }

    private void OnDestroy()
    {
        Confection.ShowConfectionParticle -= OnPerfectClick;
    }

    private void Update()
    {
        if (IsMoving && Target != null)
        {
            CastConfectionMixVfx();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            ConfectionSound.Stop();
            IsMoving = false;
            TurnOffConfectionParticles();
            StartCoroutine(GanielConfection.DealConfectionDamage());
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    private void CastConfectionMixVfx()
    {
        var victimCombatant = Target.GetComponent<Combatant>();
        float offset;
        if (victimCombatant.isBoss)
        {
            offset = TargetVerticalOffset * 2;
        }
        else if (victimCombatant is WoodLogsCombatant)
        {
            offset = 0.0f;
        }
        else
        {
            offset = TargetVerticalOffset;
        }
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + new Vector3(0.0f, offset, 0.0f), Speed * Time.deltaTime);
    }

    public void StartMoving()
    {
        IsMoving = true;
    }

    public void ResetVfx()
    {
        gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void TurnOffConfectionParticles()
    {
        for (int i = 0; i < MixParticles.Length; i++)
        {
            var particleSystem = MixParticles[i];
            
            particleSystem.Stop();
        }
    }
}
