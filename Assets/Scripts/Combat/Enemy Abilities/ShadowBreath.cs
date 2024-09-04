using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

public class ShadowBreath : Ability
{
    private GameObject Victim;
    [SerializeField] private int Damage = 50;
    [SerializeField] private float VictimForceFieldOffset = 2.5f;
    [SerializeField] private ParticleSystem BreathingParticles;
    [SerializeField] private ParticleSystemForceField VictimForceField;
    [SerializeField] private AudioSource ExhaleSound;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        
        TargetSchema = new TargetSchema(
            1,
            CombatantType.Ally,
            SelectorType.Number);
    }
    
    public new void StartAbility(bool userTargeting = false)
    {
        Debug.Log("Starting Shadow Breath ability");

        base.StartAbility();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
        FaceAllyInCombat(Victim);
        Animator.Play("Base Layer.Exhale");
    }

    protected override void EndAbility()
    {
        VictimForceField.transform.localPosition = Vector3.zero;
        VictimForceField.gameObject.SetActive(false);
        CombatSystem.EndTurn(GetComponentInParent<Combatant>().gameObject);
    }

    public void ActivateShadowBreath()
    {
        BreathingParticles.transform.LookAt(Victim.transform);
        BreathingParticles.Play();
        VictimForceField.transform.position = Victim.transform.position + new Vector3(0.0f, VictimForceFieldOffset, 0.0f);
        VictimForceField.gameObject.SetActive(true);
        ExhaleSound.Play();
    }
    
    public void DeactivateShadowBreath()
    {
        var attack = new Attack(Damage, Element, Style);
        Victim.GetComponent<Combatant>().Defend(attack);
        
        BreathingParticles.Stop();
        StartCoroutine(DelayEndOfTurn());
    }
    
    IEnumerator DelayEndOfTurn()
    {
        yield return new WaitForSeconds(1.0f);
        EndAbility();
    }
}
