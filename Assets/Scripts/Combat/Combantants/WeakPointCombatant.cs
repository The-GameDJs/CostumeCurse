using System.Collections;
using Assets.Scripts.Combat;

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

    public void ResetWeakPointHit()
    {
        HasWeakPointBeenHit = false;
        ElementResistance.Add(ElementType.Normal);
    }
    
    public abstract void TriggerWeakState();

    public abstract IEnumerator ResetWeakState();
}
