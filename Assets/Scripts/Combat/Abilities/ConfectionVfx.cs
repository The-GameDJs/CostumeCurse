using UnityEngine;

public class ConfectionVfx : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] MixParticles;

    [SerializeField]
    private ParticleSystem ExplosionParticles;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float TargetVerticalOffset;

    private bool IsMoving;
    private Confection GanielConfection;
    private GameObject Target;
    private int TotalInputs;

    private void Start()
    {
        GanielConfection = GameObject.Find("Ganiel").GetComponentInChildren<Confection>();
        Confection.CastConfectionAction += OnConfectionCasted;
    }

    private void OnConfectionCasted(int totalInputs)
    {
        TotalInputs = Mathf.Clamp(totalInputs / 8, 0, 4);
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

    private void CastConfectionMixVfx()
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
        for (int i = 0; i <= TotalInputs; i++)
        {
            var particleSystem = MixParticles[i];
            
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
        ExplosionParticles.Play();
    }
}
