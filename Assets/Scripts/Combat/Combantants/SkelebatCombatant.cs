using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;

public class SkelebatCombatant : WeakPointCombatant
{
    [SerializeField] private float FlyVerticalDistance;
    private Vector3 FlyDownPosition;
    private bool HasWeakPointBeenReset;
    
    new void Start()
    {
        TurnCountSinceWeakPoint = -1;
        base.Start();
    }

    new void Update()
    {
        base.Update();

        if (HasWeakPointBeenHit && transform.position.y - FlyDownPosition.y > 0.2f)
            transform.position = Vector3.Lerp(transform.position,
                FlyDownPosition, Time.deltaTime / 1f * 2f);
        
        if(HasWeakPointBeenReset && transform.position.y - StartCombatPosition.y < 0.2f)
            transform.position = Vector3.Lerp(transform.position,
                StartCombatPosition, Time.deltaTime / 1f * 2f);
    }

    public override void TriggerWeakPoint()
    {
        CombatType = CombatantType.Ground;
        HasWeakPointBeenHit = true;
        HasWeakPointBeenReset = false;
        FlyDownPosition = new Vector3(StartCombatPosition.x, StartCombatPosition.y - FlyVerticalDistance, StartCombatPosition.z);
    }

    protected override void TakeTurnWhileAlive()
    {
        if(HasWeakPointBeenHit)
            TurnCountSinceWeakPoint++;
        
        if(TurnCountSinceWeakPoint == 2)
            StartCoroutine(ResetWeakPoint());
        else
            base.TakeTurnWhileAlive();
    }

    public override IEnumerator ResetWeakPoint()
    {
        CombatType = CombatantType.Flying;
        HasWeakPointBeenHit = false;
        HasWeakPointBeenReset = true;
        TurnCountSinceWeakPoint = -1;
        yield return new WaitForSeconds(3f);
        base.EndTurn();
    }

    protected override void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        if (style == AttackStyle.Ranged && CombatType == CombatantType.Flying)
        {
            TriggerWeakPoint();
        }

        base.TakeDamage(damage, element, style);
    }
}
