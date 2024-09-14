using System.Collections;

public abstract class WeakPointCombatant : EnemyCombatant
{
    protected bool HasWeakPointBeenHit;
    protected int TurnCountSinceWeakPoint;

    protected void Start()
    {
        TurnCountSinceWeakPoint = -1;
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }
    
    public abstract void TriggerWeakState();

    public abstract IEnumerator ResetWeakState();
}
