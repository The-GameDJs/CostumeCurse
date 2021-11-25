using Combat.Abilities;
using UnityEngine;

public class MusicalNotes : MonoBehaviour
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
    private Skelemusic SkeletonMusicAbility;
    private GameObject Target;
    
    private void Update()
    {
        if (IsMoving && Target != null)
        {
            CastMusicalNotesVfx();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Target))
        {
            IsMoving = false;
            SwitchMusicalNotesParticleSystemsState(false);
            SkeletonMusicAbility.DealDamage();
        }
    }
    
    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    private void CastMusicalNotesVfx()
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
    
    public void SwitchMusicalNotesParticleSystemsState(bool activate = true)
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
    
    public void ExplodeCandies()
    {
        explosionParticles.Play();
    }
}
