using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextHelper : MonoBehaviour
{
    public Animator DamageTextAnimator;
    public TextMeshProUGUI DamageTextField;
    
    public void OnAnimationDone()
    {
        DamageTextAnimator.enabled = false;
        DamageTextField.gameObject.SetActive(false);
    }
}
