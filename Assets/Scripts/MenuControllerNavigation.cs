using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuControllerNavigation : MonoBehaviour
{
    [SerializeField] private Button FirstMenuItem;
    private GameObject _previousFirstMenuItem;
    
    private void OnEnable()
    {
        _previousFirstMenuItem = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(FirstMenuItem.gameObject);
        FirstMenuItem.Select();
    }

    private void OnDisable()
    {
        if(_previousFirstMenuItem != null)
            EventSystem.current.SetSelectedGameObject(_previousFirstMenuItem);
    }
}
