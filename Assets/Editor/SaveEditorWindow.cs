using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveEditorWindow : EditorWindow
{
    private const float WINDOW_WIDTH = 400;
    private const float WINDOW_HEIGHT = 300;
    
    private Transform _player;
    private CandyCornManager _candyCornManager;
    
    [MenuItem("Debug/Gameplay/Save Menu %&s")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<SaveEditorWindow>();
        wnd.titleContent = new GUIContent("Save Menu");
        wnd.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        wnd.maxSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
    }
    
    public void CreateGUI()
    {
        // Check if the game is in play mode
        if (EditorApplication.isPlaying)
        {
            FindObjects();
            
            if(_player != null && _candyCornManager != null)
                CreateEditorButtons();
        }
        else
        {
            // If not in play mode, display a message
            DisplayPlayModeMessage();
        }
    }

    private void FindObjects()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _candyCornManager = FindObjectOfType<CandyCornManager>();
    }

    private void DisplayPlayModeMessage()
    {
        var messageLabel = new Label("Start the game to use the save window.")
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleCenter,
                flexGrow = 1
            }
        };
        rootVisualElement.Add(messageLabel);
    }

    private void CreateEditorButtons()
    {
        // Clear any existing buttons
        rootVisualElement.Clear();
        
        var messageLabel = new Label("Save Editor Menu")
        {
            style =
            {
                unityTextAlign = TextAnchor.UpperCenter,
                marginTop = 5,
                marginBottom = 5,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };
        rootVisualElement.Add(messageLabel);
        
        var button = new Button(SaveGame)
        {
            text = $"Save"
        };
        
        rootVisualElement.Add(button);
        
        var candyButton = new Button(AddCandyCorn)
        {
            text = $"Add Candy Corn (+50)"
        };
        
        rootVisualElement.Add(candyButton);
    }

    private void SaveGame()
    {
        var saveData = new SaveSystem.SaveData(_player.transform.position, 150, 3, 2);
        SaveSystem.Save(saveData);
    }

    private void AddCandyCorn()
    {
        _candyCornManager.AddCandyCorn(50);
    }

    private void OnEnable()
    {
        // Rebuild the UI when play mode changes
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Rebuild the UI when the play mode state changes
        rootVisualElement.Clear();
        CreateGUI();
    }
}
