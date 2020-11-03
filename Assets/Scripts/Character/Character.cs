using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, ISubject
{
    int hitPoints;
    bool isDead;
    List<IObserver> observers;

    void Awake()
    {
        observers = new List<IObserver>();
        isDead = false;
    }

    public abstract void PlayTurn();

    public void NotifyEndOfTurn()
    {
        CharacterEvent CEvent = new CharacterEvent(true);
        this.Notify(CEvent);
    }

    protected void TakeDamage(int damage)
    {
        hitPoints -= damage;
    }
    
    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(CharacterEvent gameEvent)
    {
        foreach (IObserver obs in observers)
        {
            obs.OnNotify(gameEvent);
        }
    }
}

public struct CharacterEvent
{
    public bool isTurnCompleted;

    public CharacterEvent(bool turnBool)
    {
        this.isTurnCompleted = turnBool;
    }
}