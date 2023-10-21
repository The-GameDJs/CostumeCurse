using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAnimationHelper : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Combatant BossCombatant;
    [SerializeField] private Animator CrossfadeAnimator;
    [SerializeField] private AudioSource MusicSound;
    public static Action<bool> ActivateChargeUpReleaseAction;
    public static Action DealCandyStormDamageAction;

    public void ReturnToOriginalRotation()
    {
        transform.Rotate(90f, 0f, 0f);
    }

    public void StartCrossfadeAnimation()
    {
        CrossfadeAnimator.SetTrigger("Start");
    }

    public void EndDeathAnimation()
    {
        StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        MusicSound.Stop();
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Epilogue_Scene");
    }

    public void ActivateFireChargeRelease()
    {
        ActivateChargeUpReleaseAction?.Invoke(true);
    }

    public void DealCandyStormDamage()
    {
        DealCandyStormDamageAction?.Invoke();
    }
    
    public void DealBonkDamage()
    {
        BossCombatant.DealBonkDamage();
    }

    public void SetCastingToFalse()
    {
        Animator.SetBool("IsFinishedCasting", false);
    }

}
