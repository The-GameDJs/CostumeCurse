using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Combat.Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Enemy_Abilities
{
    public class TankShield : Ability
    {
        private const float EndOfTurnDelay = 2.0f;
        
        [SerializeField] private int TankShieldHealth = 50;

        [SerializeField] private AudioSource SlashSound;
        [SerializeField] private AudioSource ShieldUpSound;
        [SerializeField] private AudioSource ShieldSound;

        // Start is called before the first frame update
        public new void Start()
        {
            base.Start();
            TargetSchema = new TargetSchema(
                1,
                CombatantType.Enemy,
                SelectorType.Self);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting TankShield ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            StartCoroutine(StartTankShielding());
        }
    
        private IEnumerator StartTankShielding()
        {
            yield return new WaitForSeconds(1.0f);
            
            Animator.Play($"Base Layer.Shield");
            Debug.Log($"APPLYING SHIELD.");
            
            yield return new WaitForSeconds(1.0f);
            
            var shieldObject = TargetedCombatants[0].GetComponent<Combatant>().ApplyShield(TankShieldHealth, Element);
            shieldObject.transform.localScale = Vector3.one * 15f;
            ShieldSound.Play();
            
            yield return new WaitForSeconds(1.0f);

            EndAbility();
        }

        public void ActivateTankShield()
        {
            SlashSound.Play();
            ShieldUpSound.Play();
        }

        protected override void EndAbility()
        {
            StopAllCoroutines();
            Debug.Log($"Pimpkin Bulker shielded self with {TankShieldHealth} shield.");
            CombatSystem.EndTurn(GetComponentInParent<Combatant>().gameObject);
        }
    }
}
