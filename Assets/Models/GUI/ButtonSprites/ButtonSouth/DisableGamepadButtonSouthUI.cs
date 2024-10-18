using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGamepadButtonSouthUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    public void DisableGamepadButtonSouthUIEvent()
    {
        _animator.Play("Default");
        _animator.Rebind();
        _animator.Update(0.0f);
        gameObject.SetActive(false);
    }
}
