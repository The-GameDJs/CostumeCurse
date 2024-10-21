using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryVfx : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _lightning;

    public void ActivateVfx(AllyCombatant ally)
    {
        gameObject.SetActive(true);
        transform.position = ally.ParryVfxPlacement.position;
        transform.rotation = ally.ParryVfxPlacement.rotation;
        _animator.Play("Play");
    }

    public void ActivateLightning()
    {
        _lightning.Play();
    }

    public void DeactivateVfx()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }
}
