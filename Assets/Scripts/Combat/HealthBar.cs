using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{   
    public Image BarUI;
    private float CurrentHealth;
    public float NewHealth;
    public float MaxHealth;
    private const float speed = 2.0f;

    void Start()
    {
        BarUI = GetComponent<Image>();
    }

    void Update()
    {
        BarUI.fillAmount = CurrentHealth / MaxHealth;
    }

    void LateUpdate()
    {
        CurrentHealth = Mathf.Lerp(CurrentHealth, NewHealth, Time.deltaTime * speed);
    }
}
