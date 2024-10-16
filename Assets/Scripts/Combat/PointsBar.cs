﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsBar : MonoBehaviour
{
    public Animator DamageTextAnimator;
    public TextMeshProUGUI DamageTextField;
    public Image BarUI;
    private float CurrentValue;
    public float NewValue;
    public float MaxValue;
    private const float Speed = 2.0f;

    void Start()
    {
        BarUI = GetComponent<Image>();
    }

    void Update()
    {
        BarUI.fillAmount = CurrentValue / MaxValue;
    }

    void LateUpdate()
    {
        CurrentValue = Mathf.Lerp(CurrentValue, NewValue, Time.deltaTime * Speed);
    }

    public void PlayDamageTextField(int damage)
    {
        DamageTextField.gameObject.SetActive(true);
        DamageTextField.text = $"-{damage.ToString()}";
        DamageTextAnimator.enabled = true;
        DamageTextAnimator.Play("Move");
    }
}
