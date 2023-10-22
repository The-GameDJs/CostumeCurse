using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHelper : MonoBehaviour
{
    [SerializeField] private float startTime = 2.0f; // Adjust the start time as needed
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float ScrollSpeed;

    private void Start()
    {
        particleSystem.Simulate(startTime, true, false);
        particleSystem.Play();
    }
}
