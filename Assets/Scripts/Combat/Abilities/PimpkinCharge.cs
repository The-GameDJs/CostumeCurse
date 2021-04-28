using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    public void SetComponent(Supercharge charge)
    {
        PimpkinSupercharge = charge;
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
        PimpkinSupercharge.DealSuperchargeDamage();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ChargeRigidbody = null;
        PimpkinSupercharge = null;
        Target = null;
    }

}
