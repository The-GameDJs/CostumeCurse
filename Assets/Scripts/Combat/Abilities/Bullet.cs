using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody BulletRigidbody;

    [SerializeField]
    private float BulletSpeed;

    private Revolver GanielRevolver;
    private GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        GanielRevolver = GameObject.Find("Ganiel").GetComponentInChildren<Revolver>();
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    public Rigidbody GetRigidBody()
    {
        return BulletRigidbody;
    }

    public float GetSpeed()
    {
        return BulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        GanielRevolver.DealRevolverDamage();
        Destroy(gameObject);
    }
}
