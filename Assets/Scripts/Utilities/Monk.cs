using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AbilitySummoningInfo
{
    public List<GameObject> visualFX;
    public AudioSource audioFX;
    public string heroName;
}

public class Monk : MonoBehaviour
{
    [SerializeField] private ParticleSystem SummoningCloud;
    [SerializeField] private AbilitySummoningInfo GrantingAbility;

    public AbilitySummoningInfo GrantingPlayerAbility => GrantingAbility;

    public void PlayParticlesSummoningCloud()
    {
        if (!SummoningCloud) return;

        if (SummoningCloud.transform.position != gameObject.transform.position)
        {
            SummoningCloud.transform.position = gameObject.transform.position;
        }
        SummoningCloud.Play();
    }

    public void PlayParticlesDisappearingCloud()
    {
        if (!SummoningCloud || !gameObject.activeSelf) return;
        
        SummoningCloud.transform.parent = null;
        SummoningCloud.Play();
    }
}
