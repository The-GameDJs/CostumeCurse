using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Combat.Abilities
{
    public class Supercharge : Ability
    {
        Timer Timer;
        private GameObject Victim;
        Animator Animator;
        private readonly float SuperchargeDuration = 2.0f;
        private readonly float EndOfTurnDelay = 2.0f;
        private enum Phase { Inactive, Supercharge }
        private Phase CurrentPhase = Phase.Inactive;

        [SerializeField] private float Damage;


        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            Animator = GetComponentInParent<Animator>();


            TargetSchema = new TargetSchema(
                1,
                CombatantType.Ally,
                SelectorType.Number);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting Supercharge ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[UnityEngine.Random.Range(0, TargetedCombatants.Length)];
            StartSupercharge();
        }

        private void StartSupercharge()
        {
            CurrentPhase = Phase.Supercharge;
            Timer.ResetTimer();
            Animator.Play("Base Layer.Ability");
            StartCoroutine(DelaySuperchargeDamage());
        }

        protected override void EndAbility()
        {
            StopAllCoroutines();
            Timer.StopTimer();

            Debug.Log($"Supercharge Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        IEnumerator DelaySuperchargeDamage()
        {
            yield return new WaitForSeconds(SuperchargeDuration);
            DealSuperchargeDamage();
        }

        private void DealSuperchargeDamage()
        {
            if (CurrentPhase == Phase.Supercharge)
            {
                Attack attack = new Attack((int)Damage);

                Victim.GetComponent<Combatant>().Defend(attack);
                StartCoroutine(DelayEndOfTurn());
            }
        }

        IEnumerator DelayEndOfTurn()
        {
            yield return new WaitForSeconds(EndOfTurnDelay);
            EndAbility();
        }
    }
}
