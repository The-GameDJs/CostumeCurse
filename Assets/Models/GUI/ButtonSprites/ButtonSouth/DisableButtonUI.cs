using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButtonUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    public void DisableButtonUIEvent()
    {
        _animator.Play("Default");
        _animator.Rebind();
        _animator.Update(0.0f);
        gameObject.SetActive(false);
    }
}
