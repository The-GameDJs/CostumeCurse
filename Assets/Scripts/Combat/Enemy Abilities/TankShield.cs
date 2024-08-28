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
        private Timer Timer;
        private const float EndOfTurnDelay = 2.0f;
        
        [SerializeField] private int TankShieldHealth = 50;

        [SerializeField] private AudioSource SlashSound;
        [SerializeField] private AudioSource ShieldSound;
    
        [Header("Shooting Battle Phase")]
        [SerializeField] private GameObject[] Shields;

        // Start is called before the first frame update
        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            TargetSchema = new TargetSchema(
                0,
                CombatantType.Enemy,
                SelectorType.All);
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
            
            yield return new WaitForSeconds(1.0f);
            foreach (var target in TargetedCombatants)
            {
                if (target.TryGetComponent<Combatant>(out var enemy))
                {
                    //enemy.ApplyShield(TankShieldHealth , Element);
                }
            }
            
            yield return new WaitForSeconds(1.0f);

            EndAbility();
        }

        protected override void EndAbility()
        {
            Timer.StopTimer();
            
            Debug.Log($"Pimpkin Bulker shielded {TargetedCombatants.Length} allies with {TankShieldHealth} shield each.");
            
            CombatSystem.EndTurn(GetComponentInParent<Combatant>().gameObject);
        }
    }
}
