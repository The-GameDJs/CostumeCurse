using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    int hitPoints;

    public abstract void PlayTurn();

    protected void TakeDamage(int damage)
    {
        hitPoints -= damage;
    }
}
