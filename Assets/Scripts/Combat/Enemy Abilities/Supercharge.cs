using System.Collections;
using Assets.Scripts.Combat;
using Combat.Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Enemy_Abilities
{
    public class Supercharge : Ability
    {
        Timer Timer;
        private GameObject Victim;
        private const float EndOfTurnDelay = 2.0f;
        private const float ChargeTargetHeightOffset = 2.0f;

        private enum Phase { Inactive, Supercharge }
        private Phase CurrentPhase = Phase.Inactive;

        private readonly float BaseDamage = 40f;
        private float Damage;

        [SerializeField] private AudioSource SpinSound;

        [Header("Shooting Battle Phase")]
        [SerializeField] private GameObject PimpkinCharge;
        [SerializeField] private Transform PimpkinFingers;
        private PimpkinCharge CurrentCharge = null;


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
            Victim = TargetedCombatants[0];
            FaceAllyInCombat(Victim);
            StartSupercharge();
        }

        private void StartSupercharge()
        {
            CurrentPhase = Phase.Supercharge;
            Timer.ResetTimer();
            SpawnPimpkinFireball();
            Animator.Play($"Base Layer.Supercharge");
            SpinSound.Play();
        }

        protected override void EndAbility()
        {
            Victim.GetComponent<Player>().SetFire(false);
            StopAllCoroutines();
            Timer.StopTimer();
            
            Debug.Log($"Supercharge Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + 10);
        }

        private void SpawnPimpkinFireball()
        {
            var go = Instantiate(PimpkinCharge, PimpkinFingers.position, PimpkinFingers.rotation);
            go.transform.SetParent(PimpkinFingers);
            CurrentCharge = go.gameObject.GetComponent<PimpkinCharge>();
            CurrentCharge.SetTarget(TargetedCombatants[0]);
            CurrentCharge.SetComponent(this);
        }

        public void ThrowChargeAtTarget()
        {
            PimpkinFingers.SetParent(null);
            var direction = (TargetedCombatants[0].gameObject.transform.position + new Vector3(0f, ChargeTargetHeightOffset, 0f) - PimpkinFingers.position).normalized;
            CurrentCharge.GetRigidBody().velocity = direction * CurrentCharge.GetSpeed();
        }

        public void DealSuperchargeDamage()
        {
            if (CurrentPhase != Phase.Supercharge)
            {
                return;
            }
            Damage = CalculateDamage();
            Attack attack = new Attack((int)Damage);

            Victim.GetComponent<Combatant>().Defend(attack);
            StartCoroutine(DelayEndOfTurn());
        }

        IEnumerator DelayEndOfTurn()
        {
            yield return new WaitForSeconds(EndOfTurnDelay);
            EndAbility();
        }

        public void EmptyCurrentCharge()
        {
            CurrentCharge = null;
        }
    }
}
