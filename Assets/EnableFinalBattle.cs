using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFinalBattle : MonoBehaviour
{
    [SerializeField] private GameObject FinalBattleZone;
    
    void OnTriggerEnter()
    {
        FinalBattleZone.SetActive(true);
    }
}
