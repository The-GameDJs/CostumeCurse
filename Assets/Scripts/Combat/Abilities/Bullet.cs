using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer BulletTrail;

    [SerializeField]
    private Rigidbody BulletRigidbody;

    [SerializeField]
    private float BulletSpeed;

    [SerializeField]
    private GameObject GunshotImpact;

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
        Instantiate(GunshotImpact, gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
        GanielRevolver.DealRevolverDamage();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BulletTrail.transform.parent = null;
        BulletTrail.autodestruct = true;
        BulletTrail = null;
        BulletRigidbody = null;
        GanielRevolver = null;
        Target = null;
    }
}
