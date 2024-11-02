using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private AudioSource ExplorationMusicSource;
    [SerializeField] private AudioSource CombatMusicSource;

    public void StopAllMusic()
    {
        if (ExplorationMusicSource.isPlaying)
        {
            ExplorationMusicSource.Stop();
        }

        if (CombatMusicSource.isPlaying)
        {
            CombatMusicSource.Stop();
        }
    }
    
    public void EnterCombatMode()
    {
        ExplorationMusicSource.Stop();
        CombatMusicSource.Play();
    }

    public void ExitCombatMode()
    {
        CombatMusicSource.Stop();
        ExplorationMusicSource.Play();
    }
}
