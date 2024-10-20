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
        public enum PimpkinType { Pimpkin, DarkPimpkin}
        [SerializeField] private PimpkinType PimpkinEnemyType;

        [SerializeField] private float BaseDamage = 40f;
        [SerializeField] private float OffsetDamage = 10f;
        private float Damage;

        [SerializeField] private AudioSource SpinSound;

        [Header("Shooting Battle Phase")]
        [SerializeField] private GameObject PimpkinCharge;
        [SerializeField] private Transform PimpkinFingers;
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

        public PimpkinType GetPimpkinType()
        {
            return PimpkinEnemyType;
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting Supercharge ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
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
            // Set Player on fire depending on supercharge's pimpkin type
            if (GetPimpkinType() == Supercharge.PimpkinType.Pimpkin)
            {
                Victim.GetComponent<Combatant>().SetFire(false, Combatant.FireType.eOrangeFire);
            }
            else if (GetPimpkinType() == Supercharge.PimpkinType.DarkPimpkin)
            {
                Victim.GetComponent<Combatant>().SetFire(false, Combatant.FireType.ePurpleFire);
            }
            StopAllCoroutines();
            Timer.StopTimer();
            
            Debug.Log($"Supercharge Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn();
        }

        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + OffsetDamage);
        }

        private void SpawnPimpkinFireball()
        {
            CurrentCharge = Instantiate(PimpkinCharge,
                                        PimpkinFingers.position, 
                                        PimpkinFingers.rotation, PimpkinFingers)
                                        .GetComponent<PimpkinCharge>();
            CurrentCharge.transform.localScale = Vector3.one / 100f; // CHECK FOR ORANGE PIMPKIN!
            CurrentCharge.SetComponents(this, Victim);
        }

        public void ThrowChargeAtTarget()
        {
            // Juuuuust in case above statement has failed fails
            if (!CurrentCharge) CurrentCharge = FindObjectOfType<PimpkinCharge>();
                
            CurrentCharge.transform.SetParent(null);
            var direction = (Victim.gameObject.transform.position + new Vector3(0f, ChargeTargetHeightOffset, 0f) - PimpkinFingers.position).normalized;
            CurrentCharge.GetRigidBody().velocity = direction * CurrentCharge.GetSpeed();
            
            CinemachineCameraRig.Instance.SetCinemachineCameraTarget(Victim.transform);
        }

        public void DealSuperchargeDamage()
        {
            if (CurrentPhase != Phase.Supercharge)
            {
                return;
            }
            Damage = CalculateDamage();
            Attack attack = new Attack((int)Damage, Element, Style);

            var victimCombatant = Victim.GetComponent<AllyCombatant>();
            PlayExplosionParticles(victimCombatant.HasParriedCorrectly, victimCombatant);
            victimCombatant.Defend(attack);
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
