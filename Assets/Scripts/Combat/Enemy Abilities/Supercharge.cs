using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class Supercharge : Ability
    {
        Timer Timer;
        private GameObject Victim;
        private readonly float SuperchargeDuration = 2.0f;
        private readonly float EndOfTurnDelay = 2.0f;
        private readonly float ChargeTargetHeightOffset = 2.0f;
        private enum Phase { Inactive, Supercharge }
        private Phase CurrentPhase = Phase.Inactive;

        private readonly float BaseDamage = 40f;
        private float Damage;

        [SerializeField] private AudioSource SpinSound;

        [Header("Shooting Battle Phase")]
        [SerializeField] GameObject PimpkinCharge;
        [SerializeField] Transform PimpkinFingers;
        private PimpkinCharge CurrentCharge;


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
            FaceAllyInCombat(Victim);
            StartSupercharge();
        }

        private void StartSupercharge()
        {
            CurrentPhase = Phase.Supercharge;
            Timer.ResetTimer();
            Animator.Play("Base Layer.Supercharge");
            SpinSound.Play();
        }

        protected override void EndAbility()
        {
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

        public void SpawnPimpkinFireball()
        {
            GameObject go = Instantiate(PimpkinCharge, PimpkinFingers.transform.position, PimpkinFingers.transform.rotation);
            go.transform.SetParent(PimpkinFingers);
            CurrentCharge = go.GetComponent<PimpkinCharge>();
            CurrentCharge.SetTarget(TargetedCombatants[0]);
            CurrentCharge.SetComponent(this);
        }

        public void ThrowChargeAtTarget()
        {
            PimpkinFingers.SetParent(null);
            Vector3 direction = (TargetedCombatants[0].gameObject.transform.position + new Vector3(0f, ChargeTargetHeightOffset, 0f) - PimpkinFingers.position).normalized;
            CurrentCharge.GetRigidBody().velocity = direction * CurrentCharge.GetSpeed();
        }

        public void DealSuperchargeDamage()
        {
            if (CurrentPhase == Phase.Supercharge)
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
