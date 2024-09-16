using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AbilitySelectHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator AbilityAnim;
    private Animator ButtonAnim;
    [SerializeField]
    private AbilityDescription AbilityDescription;
    [SerializeField]
    private GameObject AbilityDescriptionPanel;

    [SerializeField] private GameObject ButtonIcon;
    [SerializeField] private bool IsFirstButton;
    
    private TMP_Text Name;
    private TMP_Text Description;
    private bool IsUsingMouse;

    void Start()
    {
        AbilityDescriptionPanel.SetActive(false);
        AbilityAnim = GetComponent<Animator>();
        ButtonAnim = ButtonIcon.GetComponent<Animator>();
        Name = AbilityDescriptionPanel.GetComponentsInChildren<TMP_Text>()[0];
        Description = AbilityDescriptionPanel.GetComponentsInChildren<TMP_Text>()[1];
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == ButtonIcon)
        {
            SetUpSelectedButton(true);
            Name.text = AbilityDescription.Name;
            Description.text = AbilityDescription.Description;
        }
        else
            SetUpSelectedButton(false);
    }

    private void SetUpSelectedButton(bool isActive)
    {
        AbilityDescriptionPanel.SetActive(true);
        AbilityAnim.SetBool("isSelected", isActive);
        ButtonAnim.SetTrigger(isActive ? "Highlighted" : "Normal");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // IsUsingMouse = true;
        AbilityAnim.SetBool("isSelected", true);
        Name.text = AbilityDescription.Name;
        Description.text = AbilityDescription.Description;
        AbilityDescriptionPanel.SetActive(true);
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        IsUsingMouse = false;
        AbilityAnim.SetBool("isSelected", false);
        AbilityDescriptionPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (IsFirstButton)
        {
            EventSystem.current.SetSelectedGameObject(ButtonIcon);
        }
    }
}
