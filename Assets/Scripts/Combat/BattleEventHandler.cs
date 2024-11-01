using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class BattleEventHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera CinemachineCamera;
    [SerializeField] private GameObject ObjectToAppear;
    [SerializeField] private List<GameObject> BattlesAffected;
    [SerializeField] private string GrantAbilityTo;
    [SerializeField] private int AbilityNumber;

    private int _currentBattleCounter;
    
    public static Action DialogueEnded;
    
    void Start()
    {
        var save = SaveSystem.Load();
        int abilityIndex;

        switch (GrantAbilityTo)
        {
            case "Sield":
                abilityIndex = save.SieldAbilityIndex;
                break;
            default:
                abilityIndex = save.GanielAbilityIndex;
                break;
        }
        
        if (abilityIndex >= AbilityNumber)
            return;
        
        CombatZone.BattleEnded += OnBattleEnded;
        DialogueEnded += OnDialogueEnded;
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
        StartCoroutine(SummonMonk());
    }

    private void OnDialogueEnded()
    {
        StartCoroutine(VanishMonk());
    }

    private IEnumerator SummonMonk()
    {
        var sield = GameObject.Find("Sield").GetComponent<Player>();
        sield.DisableMovement();
        
        yield return new WaitForSeconds(2.0f);
        
        ObjectToAppear.SetActive(true);
        if (ObjectToAppear.TryGetComponent<Monk>(out var monk))
        {
            monk.PlayParticlesSummoningCloud();
        }

        var previousCamera = CinemachineCameraRig.Instance.CurrentCinemachineCamera;
        CinemachineCameraRig.Instance.SetCinemachineCamera(CinemachineCamera);
        
        yield return new WaitForSeconds(4.0f);
        
        CinemachineCameraRig.Instance.SetCinemachineCamera(previousCamera);
        
        yield return new WaitForSeconds(2.0f);
        
        sield.EnableMovement();
    }

    private IEnumerator VanishMonk()
    {
        var monk = ObjectToAppear.GetComponent<Monk>();
        if (monk)
        {
            monk.PlayParticlesDisappearingCloud();
        }
        
        yield return new WaitForSeconds(0.6f);
        
        ObjectToAppear.SetActive(false);
    }

    private void OnDestroy()
    {
        CombatZone.BattleEnded -= OnBattleEnded;
        DialogueEnded -= OnDialogueEnded;
    }
}
