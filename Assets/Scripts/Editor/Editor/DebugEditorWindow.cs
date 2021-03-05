using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class DebugEditorWindow : EditorWindow
{
    private const int ADDCANDIES = 5;

    private CandyCornManager CandyCornManager;
    private List<DebugPosition> Locations = new List<DebugPosition>();
    private DebugPosition currentLocationSelected;

    private IntegerField CandiesField;
    private Button ReloadCandiesButton;
    private Button ResetCandiesButton;
    private Button AddCandiesButton;
    private Button RemoveCandiesButton;

    private ToolbarMenu LocationDropdown;
    private Button ReloadLocationButton;

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
        InitializeLocations();

        CandyCornManager = GameObject.Find("CandyCornManager").GetComponent<CandyCornManager>();
        //currentLocationSelected = GameObject.Find("DebugPositions").transform.GetChild(0).GetComponent<DebugPosition>();

        var root = rootVisualElement.Q("Root");
        CandiesField = root.Q<IntegerField>("CandiesField");

        ReloadCandiesButton = rootVisualElement.Q<Button>("ReloadCandiesButton");
        ReloadCandiesButton.clicked += OnReloadCandiesPressed;

        ResetCandiesButton = root.Q<Button>("ResetButton");
        ResetCandiesButton.clicked += OnResetCandiesPressed;

        AddCandiesButton = root.Q<Button>("AddButton");
        AddCandiesButton.clicked += OnAddCandiesPressed;

        RemoveCandiesButton = root.Q<Button>("RemoveButton");
        RemoveCandiesButton.clicked += OnRemoveCandiesPressed;

        ReloadLocationButton = rootVisualElement.Q<Button>("ReloadLocationButton");
        ReloadLocationButton.clicked += OnReloadLocationPressed;

        LocationDropdown = root.Q<ToolbarMenu>("LocationDropdown");
        PopulateLocationsDropdown();
    }

    private void OnDisable()
    {
        
    }

    private void InitializeUxmlTemplate()
    {
        rootVisualElement.Clear();
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Debug/Editor/DebugEditorView.uxml");
        template.CloneTree(rootVisualElement);
    }

    private void PopulateLocationsDropdown()
    {
        var debugEmpty = GameObject.Find("DebugPositions");
        foreach (var location in Locations)
        {
            LocationDropdown.menu.AppendAction(
                location.LocationName,
                action => OnLocationSelected(location),
                action => GetLocationDropdownState(location));
        }
    }

    private void InitializeLocations()
    {
        var debugEmpty = GameObject.Find("DebugPositions");
        for (int i = 0; i < debugEmpty.transform.childCount; i++)
        {
            Locations.Add(debugEmpty.transform.GetChild(i).GetComponent<DebugPosition>());
        }
    }

    private DropdownMenuAction.Status GetLocationDropdownState(DebugPosition location)
    {
        return location == currentLocationSelected ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
    }

    private void OnLocationSelected(DebugPosition location)
    {
        currentLocationSelected = location;
        LocationDropdown.text = location.LocationName;
    }

    private void OnReloadCandiesPressed()
    {
        if (CandiesField.value >= 0)
        {
            var candiesAmount = CandiesField.value;
            CandyCornManager.SetCandyCorn(candiesAmount);
        }
    }

    private void OnReloadLocationPressed()
    {
        if (currentLocationSelected == null)
        {
            currentLocationSelected = GameObject.Find("DebugPositions").transform.GetChild(0).GetComponent<DebugPosition>();
        }
        var Sield = GameObject.Find("Sield").transform;
        var Ganiel = GameObject.Find("Ganiel").transform;

        Sield.position = currentLocationSelected.Coordinates;
        Ganiel.GetComponent<Player>().DisableMovement();
        Ganiel.position = Ganiel.GetComponent<Player>().GetTargetPosition();
        Ganiel.rotation = Ganiel.GetComponent<Player>().GetTargetRotation();
        Ganiel.GetComponent<Player>().EnableMovement();
        Debug.Log(currentLocationSelected.LocationName);
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
        if (CandyCornManager.GetTotalCandyCorn() >= ADDCANDIES)
        {
            CandyCornManager.RemoveCandyCorn(ADDCANDIES);
        }
    }
}

public abstract class DebugEditorTab : VisualElement
{
    public abstract string TabName { get; }

    public virtual void OnGui() { }

    public abstract void RegisterListeners();
    public abstract void UnregisterListeners();
}
