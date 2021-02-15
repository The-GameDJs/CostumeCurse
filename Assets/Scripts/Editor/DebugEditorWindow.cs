using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class DebugEditorWindow : EditorWindow
{
    private IntegerField CandiesField;
    private Button ReloadButton;

    [MenuItem("Tools/Debug Menu _%#D")]
    private static void ShowWindow()
    {
        var window = GetWindow<DebugEditorWindow>();
        window.titleContent = new GUIContent("Debug Menu");
        window.minSize = new Vector2(350, 350);
        window.Show();
    }

    private void OnEnable()
    {
        InitializeUxmlTemplate();

        var root = rootVisualElement.Q("Root");
        CandiesField = root.Q<IntegerField>("Candies");
        ReloadButton = root.Q<Button>("ReloadButton");

        ReloadButton.clicked += OnReloadPressed;
    }

    private void InitializeUxmlTemplate()
    {
        rootVisualElement.Clear();
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/DebugEditorView.uxml");
        template.CloneTree(rootVisualElement);
    }

    private void OnReloadPressed()
    {
        var candyCornManager = GameObject.Find("CandyCornManager").GetComponent<CandyCornManager>();
        if (CandiesField.value >= 0)
        {
            var candiesAmount = CandiesField.value;
            candyCornManager.SetCandyCorn(candiesAmount);
        }
    }
}
