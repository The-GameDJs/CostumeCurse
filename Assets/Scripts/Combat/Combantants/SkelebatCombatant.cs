using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

public class SkelebatCombatant : WeakPointCombatant
{
    [SerializeField] private float FlyVerticalDistance;
    private Vector3 OriginalPosition;
    
    new void Start()
    {
        base.Start();
        OriginalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    new void Update()
    {
        base.Update();

        if (HasWeakPointBeenHit)
            transform.position = Vector3.Lerp(OriginalPosition,
                new Vector3(OriginalPosition.x, OriginalPosition.y - FlyVerticalDistance, OriginalPosition.z), Time.deltaTime / 1f);
    }

    public override void TriggerWeakPoint()
    {
        CombatType = CombatantType.Ground;
        HasWeakPointBeenHit = true;
    }

    protected override void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        if (style == AttackStyle.Ranged && CombatType == CombatantType.Flying)
        {
            TriggerWeakPoint();
        }
    }
}
