using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class DebugEditorWindow : EditorWindow
{
    private const int ADDCANDIES = 5;

    private CandyCornManager CandyCornManager;

    private IntegerField CandiesField;
    private Button ReloadButton;
    private Button ResetCandiesButton;
    private Button AddCandiesButton;
    private Button RemoveCandiesButton;

    [MenuItem("Tools/Debug Menu _%#A")]
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

        CandyCornManager = GameObject.Find("CandyCornManager").GetComponent<CandyCornManager>();

        var root = rootVisualElement.Q("Root");
        CandiesField = root.Q<IntegerField>("CandiesField");

        ReloadButton = root.Q<Button>("ReloadButton");
        ReloadButton.clicked += OnReloadPressed;

        ResetCandiesButton = root.Q<Button>("ResetButton");
        ResetCandiesButton.clicked += OnResetCandiesPressed;

        AddCandiesButton = root.Q<Button>("AddButton");
        AddCandiesButton.clicked += OnAddCandiesPressed;

        RemoveCandiesButton = root.Q<Button>("RemoveButton");
        RemoveCandiesButton.clicked += OnRemoveCandiesPressed;
    }

    private void InitializeUxmlTemplate()
    {
        rootVisualElement.Clear();
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/DebugEditorView.uxml");
        template.CloneTree(rootVisualElement);
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
