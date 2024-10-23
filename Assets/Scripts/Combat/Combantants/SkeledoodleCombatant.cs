using System.Collections;
using UnityEngine;

public class SkeledoodleCombatant : WeakPointCombatant
{
    new void Start()
    {
        base.Start();
    }

    protected override void TakeTurnWhileDead()
    {
        HasWeakPointBeenHit = true;
        TriggerWeakState();
    }

    public override void TriggerWeakState()
    {
        TurnCountSinceWeakPoint++;
        if (TurnCountSinceWeakPoint == 2)
        {
            Debug.Log("Skeledoodle has been reanimated!");
            StartCoroutine(ResetWeakState());
        }
        else
        {
            base.TakeTurnWhileDead();
        }
    }

    public override IEnumerator ResetWeakState()
    {
        HasWeakPointBeenHit = false;
        CurrentHealthPoints = MaxHealthPoints / 2;
        IsAlive = true;
        TurnCountSinceWeakPoint = -1;
        Animator.Play("Base Layer.Skelemusic");
        yield return new WaitForSeconds(3f);
        Animator.Play("Base Layer.Idle");
        EndTurn();
    }
}
