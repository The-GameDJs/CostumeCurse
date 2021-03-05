using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class ResourcesEditorTab : DebugEditorTab
{
    public override string TabName => "Resources";

    private const int ADDCANDIES = 5;

    private CandyCornManager CandyCornManager;

    private IntegerField CandiesField;
    private Button ReloadButton;
    private Button ResetCandiesButton;
    private Button AddCandiesButton;
    private Button RemoveCandiesButton;

    public ResourcesEditorTab()
    {

    }

    public override void RegisterListeners()
    {
        throw new System.NotImplementedException();
    }

    public override void UnregisterListeners()
    {
        throw new System.NotImplementedException();
    }

    private void InitializeUxmlTemplate()
    {
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/ResourcesEditorTab.uxml");
        template.CloneTree(this);
    }

    private void OnReloadPressed()
    {
        if (CandiesField.value >= 0)
        {
            var candiesAmount = CandiesField.value;
            CandyCornManager.SetCandyCorn(candiesAmount);
        }
    }

    private void OnResetCandiesPressed()
    {
        CandyCornManager.SetCandyCorn(0);
    }

    private void OnAddCandiesPressed()
    {
        CandyCornManager.AddCandyCorn(ADDCANDIES);
    }

    private void OnRemoveCandiesPressed()
    {
        if (CandyCornManager.GetTotalCandyCorn() >= 5)
        {
            CandyCornManager.RemoveCandyCorn(ADDCANDIES);
        }
    }
}
