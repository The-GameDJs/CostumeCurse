using Assets.Scripts.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class Bonk : Ability
    {
        private enum Phase { Inactive, Approaching, Bonking, Disengaging }
        private Phase CurrentPhase = Phase.Inactive;

        private Timer Timer;
        [SerializeField]
        private const float ApproachingDuration = 1.25f;
        [SerializeField]
        private const float DisengagingDuration = 0.75f;

        private const float Damage = 25;

        private Vector3 InitialPosition;
        private GameObject Victim;
        private Vector3 AttackingPosition;
        private readonly float SpaceBetweenBonk = 2.5f;

        private bool BelongsToAlly;


        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();

            BelongsToAlly = GetComponentsInParent<AllyCombatant>().Length == 1;

            TargetSchema = new TargetSchema(
                1,
                BelongsToAlly ? CombatantType.Enemy : CombatantType.Ally,
                SelectorType.Number);
        }

        public void Update()
        {
            if (CurrentPhase == Phase.Inactive)
                return;

            if (CurrentPhase == Phase.Approaching)
                ApproachingUpdate();

            if (CurrentPhase == Phase.Bonking)
                BonkingUpdate();

            if (CurrentPhase == Phase.Disengaging)
                DisengagingUpdate();
        }

        private void ApproachingUpdate()
        {
            if (Timer.IsInProgress())
            {
                float t = Timer.GetProgress() / ApproachingDuration;
                transform.parent.gameObject.transform.position = Vector3.Lerp(
                    InitialPosition,
                    AttackingPosition,
                    t);
            }

            else if (Timer.IsFinished())
            {
                StartBonkPhase();
            }
        }

        private void DisengagingUpdate()
        {
            if (Timer.IsInProgress())
            {
                float t = Timer.GetProgress() / DisengagingDuration;
                transform.parent.gameObject.transform.position = Vector3.Lerp(
                    AttackingPosition,
                    InitialPosition,
                    t);
            }

            else if (Timer.IsFinished())
            {
                EndAbility();
            }
        }

        public void DealBonkDamage()
        {
            if (CurrentPhase == Phase.Bonking)
            {
                Attack attack = new Attack((int)EvaluateBonkDamage());

                Victim.GetComponent<Combatant>().Defend(attack);
            }
        }

        private void BonkingUpdate()
        {
            if (Timer.IsInProgress())
            {
                // If we need more bonking update logic, 
            }

            else if (Timer.IsFinished())
            {
                StartDisengagingPhase();
            }
        }

        private void StartBonkPhase()
        {
            CurrentPhase = Phase.Bonking;

            var animator = GetComponentInParent<Animator>();

            animator.Play("Base Layer.Bonk");

            Timer.StartTimer(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private void StartDisengagingPhase()
        {
            CurrentPhase = Phase.Disengaging;

            Timer.StartTimer(DisengagingDuration);
        }

        private float EvaluateBonkDamage()
        {
            return Damage;
        }

        protected override void EndAbility()
        {
            Debug.Log($"Bonk Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting BONK ability");

            base.StartAbility();
        }
        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];

            InitialPosition = transform.position;

            Vector3 selfToVictimVector = Victim.gameObject.transform.position - InitialPosition;
            AttackingPosition = InitialPosition
                + selfToVictimVector.normalized * (selfToVictimVector.magnitude - SpaceBetweenBonk);

            StartApproachingPhase();
        }

        private void StartApproachingPhase()
        {
            this.CurrentPhase = Phase.Approaching;

            Timer.StartTimer(ApproachingDuration);
        }
    }
}
