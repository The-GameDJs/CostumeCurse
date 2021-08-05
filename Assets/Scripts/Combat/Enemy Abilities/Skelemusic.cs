using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class Skelemusic : Ability
    {
        Timer Timer;
        private GameObject Victim;
        private readonly float SkelemusicDuration = 2.8f;
        private readonly float EndOfTurnDelay = 2.0f;
        private enum Phase { Inactive, Skelemusic }
        private Phase CurrentPhase = Phase.Inactive;

        private readonly float BaseDamage = 40f;
        private float Damage;

        [SerializeField] private AudioSource SkelemusicSound;


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
            Debug.Log("Starting Skelemusic ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[UnityEngine.Random.Range(0, TargetedCombatants.Length)];
            FaceAllyInCombat(Victim);
            StartSkelemusic();
        }

        private void StartSkelemusic()
        {
            CurrentPhase = Phase.Skelemusic;
            Timer.ResetTimer();
            Animator.Play("Base Layer.Skelemusic");
            SkelemusicSound.Play();
            StartCoroutine(DelaySkelemusicDamage());
        }

        protected override void EndAbility()
        {
            StopAllCoroutines();
            Timer.StopTimer();

            Debug.Log($"Skelemusic Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + 10);
        }

        IEnumerator DelaySkelemusicDamage()
        {
            yield return new WaitForSeconds(SkelemusicDuration);
            DealSkelemusicDamage();
        }

        private void DealSkelemusicDamage()
        {
            if (CurrentPhase == Phase.Skelemusic)
            {
                Damage = CalculateDamage();
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