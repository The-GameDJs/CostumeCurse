using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject ObjectToAppear;
    [SerializeField] private List<GameObject> BattlesAffected;

    private int _currentBattleCounter;
    
    void Start()
    {
        CombatZone.BattleEnded += OnBattleEnded;
    }

    private void OnBattleEnded(int gameObjId)
    {
        var battles = BattlesAffected.Where(x => x.GetInstanceID() == gameObjId).ToList();

        if (battles.Count == 0)
            return;
        
        _currentBattleCounter++;

        if (_currentBattleCounter == BattlesAffected.Count)
        {
            TriggerBattleEvent();
            CombatZone.BattleEnded -= OnBattleEnded;
        }
    }

    private void TriggerBattleEvent()
    {
        ObjectToAppear.SetActive(true);
    }

    private void OnDestroy()
    {
        CombatZone.BattleEnded -= OnBattleEnded;
    }
}
