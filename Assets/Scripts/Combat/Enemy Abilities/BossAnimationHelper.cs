using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAnimationHelper : MonoBehaviour
{
    [SerializeField] private Combatant BossCombatant;
    public static Action<bool> ActivateChargeUpReleaseAction;

    public void ReturnToOriginalRotation()
    {
        transform.Rotate(90f, 0f, 0f);
    }

    public void EndDeathAnimation()
    {
        StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Game_Victory");
    }

    public void ActivateFireChargeRelease()
    {
        ActivateChargeUpReleaseAction?.Invoke(true);
    }
    
    public void DealBonkDamage()
    {
        BossCombatant.DealBonkDamage();
    }
}
