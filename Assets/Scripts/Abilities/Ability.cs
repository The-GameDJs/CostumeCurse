using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void UseAbility();

    protected abstract void ConcludeAbility();
}
