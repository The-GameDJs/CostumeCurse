using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ConfectionVfx : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] MixParticles;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float TargetVerticalOffset;

    private bool IsMoving;
    private Confection GanielConfection;
    private GameObject Target;

    private void Start()
    {
        GanielConfection = GameObject.Find("Ganiel").GetComponentInChildren<Confection>();
    }

    private void Update()
    {
        if (IsMoving && Target != null)
        {
            CastConfectionMixVfx();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            IsMoving = false;
            SwitchConfectionMixParticleSystemsState(false);
            StartCoroutine(GanielConfection.DealConfectionDamage());
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    public void CastConfectionMixVfx()
    {
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + new Vector3(0.0f, TargetVerticalOffset, 0.0f), Speed * Time.deltaTime);
    }

    public void StartMoving()
    {
        IsMoving = true;
    }

    public void ResetVfx()
    {
        gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void SwitchConfectionMixParticleSystemsState(bool activate = true)
    {
        foreach (var particleSystem in MixParticles)
        {
            if (activate)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Stop();
            }
        }
    }

    public void ExplodeConfectionMix()
    {
        explosionParticles.Play();
    }
}
