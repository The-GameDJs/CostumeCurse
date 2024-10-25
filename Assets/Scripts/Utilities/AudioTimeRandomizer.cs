using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTimeRandomizer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    void Start()
    {
        _audioSource.time = Random.Range(0.0f, _audioSource.clip.length);
    }
}
