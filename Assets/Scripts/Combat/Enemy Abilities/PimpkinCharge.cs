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
        private AllyCombatant _allyCombatant;


        private void Update()
        {
            if (Target != null && _allyCombatant != null)
            {
                if (!_allyCombatant.HasParried && InputManager.HasPressedActionCommand && 
                    Vector3.Distance(Target.transform.position, transform.position) >= 0.2f
                    && Vector3.Distance(Target.transform.position, transform.position) <= 4.2f)
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
        }

        public void SetComponents(Supercharge charge, GameObject target)
        {
            PimpkinSupercharge = charge;
            Target = target;
            _allyCombatant = target.GetComponent<AllyCombatant>();
        }

        public Rigidbody GetRigidBody()
        {
            return ChargeRigidbody;
        }

        public float GetSpeed()
        {
            return ChargeSpeed;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.Equals(Target) && !transform.parent)
            {
                if (_allyCombatant.HurtCollider == other)
                {
                    DestroyCharge();
                }
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
