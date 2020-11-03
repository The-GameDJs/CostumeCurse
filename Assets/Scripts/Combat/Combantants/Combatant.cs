using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField]
    public int TurnPriority;
    [SerializeField]
    public int MaxHealthPoints;
    [SerializeField]
    public int CurrentHealthPoints;
    [SerializeField]
    CombatSystem combatSystem;

    public bool isAlive = true;

    public abstract void StartTurn();
    public abstract void EndTurn();

    public abstract void Defend(Attack attack);



    protected void TakeDamage(int damage)
    {
        CurrentHealthPoints -= damage;
    }

}
