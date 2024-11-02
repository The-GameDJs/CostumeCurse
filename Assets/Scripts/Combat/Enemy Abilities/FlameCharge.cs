using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Enemy_Abilities;
using UnityEngine;

namespace Combat.Abilities
{
    public class FlameCharge : MonoBehaviour
    {
        [SerializeField] private Rigidbody ChargeRigidbody;
        [SerializeField] private ParticleSystem FlameParticleSystem;
        [SerializeField] private Animation ChargeUpAnimation;
        [SerializeField] private float ChargeSpeed;
    
        [SerializeField]
        private GameObject GunshotImpact;

        private Firecharge ChargeAbility;
        private GameObject Target;
        private AllyCombatant _allyCombatant;
        private bool isAttached;

        private void Update()
        {
            if (isAttached && ChargeAbility)
            {
                transform.position = ChargeAbility.GetAttachPoint().position;
            }
            if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand && 
                Vector3.Distance(Target.transform.position, transform.position) >= 0.2f
                && Vector3.Distance(Target.transform.position, transform.position) <= 6.4f)
            {
                Debug.Log("Parried correctly!");
                _allyCombatant.ParrySound.Play();
                _allyCombatant.HasParriedCorrectly = true;
            }

            if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand)
            {
                Debug.Log("Parry Button Pressed");
                _allyCombatant.HasParried = true;
            }
            
        }

        public void EnableEffect()
        {
            FlameParticleSystem.Play();
            ChargeUpAnimation.Play("FlameChargeUp");
        }
        
        public void SetTarget(GameObject target)
        {
            Target = target;
            _allyCombatant = Target.GetComponent<AllyCombatant>();
        }

        public void SetAttached(bool isAttached)
        {
            this.isAttached = isAttached;
        }

        public void SetComponent(Firecharge charge)
        {
            ChargeAbility = charge;
        }

        public Rigidbody GetRigidBody()
        {
            return ChargeRigidbody;
        }

        public float GetSpeed()
        {
            return ChargeSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.Equals(Target))
            {
                DestroyCharge();
            }
        }

        private void DestroyCharge()
        {
            Target.GetComponent<Combatant>().SetFire(true, Combatant.FireType.ePurpleFire);
            ChargeAbility.DealSuperchargeDamage();
            FlameParticleSystem.Stop();
            transform.position = Vector3.zero;
            ChargeAbility.EmptyCurrentCharge();
            Target = null;
        }
    }
}
