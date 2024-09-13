using System.Collections;

public abstract class WeakPointCombatant : EnemyCombatant
{
    protected bool HasWeakPointBeenHit;
    protected int TurnCountSinceWeakPoint;

    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }
    
    public abstract void TriggerWeakPoint();

    public abstract IEnumerator ResetWeakPoint();
}
