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
        private bool isAttached;

        private void Update()
        {
            if (isAttached && ChargeAbility)
            {
                transform.position = ChargeAbility.GetAttachPoint().position;
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
            Target.GetComponent<Player>().SetFire(true);
            ChargeAbility.DealSuperchargeDamage();
            FlameParticleSystem.Stop();
            transform.position = Vector3.zero;
            ChargeAbility.EmptyCurrentCharge();
            Target = null;
        }
    }
}
