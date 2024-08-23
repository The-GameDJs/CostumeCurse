using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Combat.Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Enemy_Abilities
{
    public class Firecharge : Ability
    {
        private Timer Timer;
        private GameObject Victim;
        private const float EndOfTurnDelay = 2.0f;
        private const float ChargeTargetHeightOffset = 2.0f;
    
        private enum Phase { Inactive, Firecharge }
        private Phase CurrentPhase = Phase.Inactive;
        private Vector3 MaxScaleSize;
        
        [SerializeField] float BaseDamage = 40f;
        private float Damage;

        [SerializeField] private AudioSource ChargeSound;
        [SerializeField] private AudioSource ThrowSound;
    
        [Header("Shooting Battle Phase")]
        [SerializeField] private GameObject FlameChargeObject;
        [SerializeField] private Transform ThrowPoint;
        private FlameCharge FlameCharge;
        private bool isReleasingFirecharge;

        // Start is called before the first frame update
        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            // Ensure target selector and combat system are found!
            if (!TargetSelector)
            {
                TargetSelector = FindObjectOfType<TargetSelector>();
            }

            if (!CombatSystem)
            {
                CombatSystem = FindObjectOfType<CombatSystem>();
            }
        
            TargetSchema = new TargetSchema(
                1,
                CombatantType.Ally,
                SelectorType.Number);
            
            BossAnimationHelper.ActivateChargeUpReleaseAction += OnChargeUpRelease;
        }

        private void OnChargeUpRelease(bool isFireChargeReleased)
        {
            isReleasingFirecharge = isFireChargeReleased;
            FlameCharge.SetAttached(!isReleasingFirecharge);
            
            var direction = (Victim.gameObject.transform.position + new Vector3(0f, ChargeTargetHeightOffset, 0f) - ThrowPoint.position).normalized;
            FlameCharge.GetRigidBody().velocity = direction * FlameCharge.GetSpeed();
            
            CameraRigSystem.MoveCameraToSelectedTarget(Victim);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting Firecharge ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
            FaceAllyInCombat(Victim);
            StartFireCharge();
        }
    
        private void StartFireCharge()
        {
            CurrentPhase = Phase.Firecharge;
            Timer.ResetTimer();
            SetUpFireCharge();
            // Fix animation issue for models with rotated animations, specifically the Boss
            if (Combatant is EnemyCombatant enemy)
            {
                enemy.RotateModel();
            }
            Animator.Play($"Base Layer.Thrust");
            ChargeSound.Play();
        }

        protected override void EndAbility()
        {
            Victim.GetComponent<Combatant>().SetFire(false, Combatant.FireType.ePurpleFire);
            StopAllCoroutines();
            Timer.StopTimer();
            
            Debug.Log($"Firecharge Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        private void SetUpFireCharge()
        {
            FlameCharge = FlameChargeObject.GetComponent<FlameCharge>();
            FlameCharge.SetAttached(true);
            FlameCharge.SetTarget(Victim);
            FlameCharge.SetComponent(this);
            FlameCharge.transform.localScale = Vector3.zero;
            FlameCharge.EnableEffect();
        }

        public Transform GetAttachPoint()
        {
            return ThrowPoint;
        }

        private void OnDestroy()
        {
            BossAnimationHelper.ActivateChargeUpReleaseAction -= OnChargeUpRelease;
        }
        
        public void DealSuperchargeDamage()
        {
            if (CurrentPhase != Phase.Firecharge)
            {
                return;
            }
            Damage = CalculateDamage();
            Attack attack = new Attack((int)Damage, Element);

            Victim.GetComponent<Combatant>().Defend(attack);
            StartCoroutine(DelayEndOfTurn());
        }
        
        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + 10);
        }

        IEnumerator DelayEndOfTurn()
        {
            yield return new WaitForSeconds(EndOfTurnDelay);
            EndAbility();
        }

        public void EmptyCurrentCharge()
        {
            FlameCharge = null;
        }
    }
}
