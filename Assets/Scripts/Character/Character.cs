using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    int hitPoints;

    public abstract void PlayTurn();

    private void AddHitPoints(int value)
    {
        hitPoints += value;
    }
}
