using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AbilitySelectHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator AbilityAnim;
    [SerializeField]
    private AbilityDescription AbilityDescription;
    [SerializeField]
    private GameObject AbilityDescriptionPanel;
    private TMP_Text Name;
    private TMP_Text Description;

    void Start()
    {
        AbilityDescriptionPanel.SetActive(false);
        AbilityAnim = GetComponent<Animator>();
        Name = AbilityDescriptionPanel.GetComponentsInChildren<TMP_Text>()[0];
        Description = AbilityDescriptionPanel.GetComponentsInChildren<TMP_Text>()[1];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilityAnim.SetBool("isSelected", true);
        Name.text = AbilityDescription.Name;
        Description.text = AbilityDescription.Description;
        AbilityDescriptionPanel.SetActive(true);
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityAnim.SetBool("isSelected", false);
        AbilityDescriptionPanel.SetActive(false);
    }
}
