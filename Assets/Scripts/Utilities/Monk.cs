using System;
using UnityEngine;

public class Monk : MonoBehaviour
{
    [SerializeField] private ParticleSystem SummoningCloud;

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
        if (!SummoningCloud) return;

        SummoningCloud.transform.parent = null;
        var main = SummoningCloud.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        SummoningCloud.Play();
    }
}
