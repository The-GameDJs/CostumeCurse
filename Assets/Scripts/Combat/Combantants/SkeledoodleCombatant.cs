using System.Collections;
using UnityEngine;

public class SkeledoodleCombatant : WeakPointCombatant
{
    [SerializeField] private Transform _rootTransform;
    [SerializeField] private Timer _timer;
    [SerializeField] private float _wiggleTime;
    private bool IsWiggling;
    
    new void Start()
    {
        IsWiggling = false;
        base.Start();
    }
    
    protected void Update()
    {
        if (!IsAlive && IsWiggling && _timer.IsInProgress())
        {
            var position = _rootTransform.localPosition;
            // Graph ensures the timer and local height start from (0,0) and then climb up
            // https://www.desmos.com/calculator/uzaljeb9he
            position.y = _wiggleTime * Mathf.Cos(-12.0f * Mathf.PI * _timer.GetProgress() - Mathf.PI) + _wiggleTime;
            _rootTransform.localPosition = position;
        }
        base.Update();
    }

    protected override void TakeTurnWhileDead()
    {
        HasWeakPointBeenHit = true;
        TriggerWeakState();
    }

    public override void TriggerWeakState()
    {
        TurnCountSinceWeakPoint++;
        if (TurnCountSinceWeakPoint == 2)
        {
            Debug.Log("Skeledoodle has been reanimated!");
            StartCoroutine(ResetWeakState());
        }
        else
        {
            StartCoroutine(Wiggle());
        }
    }

    private IEnumerator Wiggle()
    {
        if (TargetSelectorLight.gameObject.activeSelf)
        {
            TargetSelectorLight.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.3f);
        IsWiggling = true;
        _timer.StartTimer(_wiggleTime);
        yield return new WaitForSeconds(_wiggleTime * 2.0f);
        IsWiggling = false;
        _rootTransform.localPosition = Vector3.zero;
        base.TakeTurnWhileDead();
    }

    public override IEnumerator ResetWeakState()
    {
        HasWeakPointBeenHit = false;
        CurrentHealthPoints = MaxHealthPoints / 2;
        IsAlive = true;
        TurnCountSinceWeakPoint = -1;
        Animator.Play("Base Layer.Skelemusic");
        yield return new WaitForSeconds(3f);
        Animator.Play("Base Layer.Idle");
        EndTurn();
    }
}
