using System;
using Combat.Enemy_Abilities;
using UnityEngine;

namespace Combat.Abilities
{
    public class PimpkinCharge : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody ChargeRigidbody;

        [SerializeField]
        private float ChargeSpeed;

        [SerializeField]
        private GameObject GunshotImpact;

        private Supercharge PimpkinSupercharge;
        private GameObject Target;

        public void SetComponents(Supercharge charge, GameObject target)
        {
            PimpkinSupercharge = charge;
            Target = target;
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
            if (other.gameObject.Equals(Target) && !transform.parent)
            {
                DestroyCharge();
            }
        }

        private void DestroyCharge()
        {
            // Set Player on fire depending on supercharge's pimpkin type
            if (PimpkinSupercharge.GetPimpkinType() == Supercharge.PimpkinType.Pimpkin)
            {
                Target.GetComponent<Combatant>().SetFire(true, Combatant.FireType.eOrangeFire);
            }
            else if (PimpkinSupercharge.GetPimpkinType() == Supercharge.PimpkinType.DarkPimpkin)
            {
                Target.GetComponent<Combatant>().SetFire(true, Combatant.FireType.ePurpleFire);
            }
            PimpkinSupercharge.DealSuperchargeDamage();
            
            PimpkinSupercharge.EmptyCurrentCharge();
            ChargeRigidbody = null;
            Target = null;
            Destroy(gameObject);
        }
    }
}
