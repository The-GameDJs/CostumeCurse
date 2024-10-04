using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TeleportationEditorWindow : EditorWindow
{
    private const float WINDOW_WIDTH = 400;
    private const float WINDOW_HEIGHT = 300;
    
    private GameObject debugPositionsParent;
    private Transform player;

    [MenuItem("Debug/Gameplay/Teleportation Menu %&d")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<TeleportationEditorWindow>();
        wnd.titleContent = new GUIContent("Teleportation Menu");
        wnd.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        wnd.maxSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
    }

    public void CreateGUI()
    {
        // Check if the game is in play mode
        if (EditorApplication.isPlaying)
        {
            // Ensure objects are correctly assigned
            FindRequiredObjects();

            if (debugPositionsParent != null && player != null)
            {
                // Create buttons for each child of DebugPositions
                CreateDebugPositionButtons();
            }
            else
            {
                // Display error message if required objects are not found
                DisplayErrorMessage();
            }
        }
        else
        {
            // If not in play mode, display a message
            DisplayPlayModeMessage();
        }
    }

    private void FindRequiredObjects()
    {
        debugPositionsParent = GameObject.Find("DebugPositions");
        player = GameObject.Find("Sield")?.transform;

        if (debugPositionsParent == null)
            Debug.LogWarning("DebugPositions parent not found.");
        if (player == null)
            Debug.LogWarning("Player object not found or not tagged as 'Player'.");
    }

    private void CreateDebugPositionButtons()
    {
        // Clear any existing buttons
        rootVisualElement.Clear();

        var messageLabel = new Label("NOTE: Teleportation will only work if you DOUBLE CLICK button")
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

        // Create buttons for each child of DebugPositions
        foreach (Transform child in debugPositionsParent.transform)
        {
            var button = new Button(() => TeleportToDebugPosition(child))
            {
                text = child.name
            };

            rootVisualElement.Add(button);
        }
    }

    private void TeleportToDebugPosition(Transform targetTransform)
    {
        if (player != null)
        {
            player.position = targetTransform.position;

            // Force the scene to update immediately
            SceneView.RepaintAll();
            EditorUtility.SetDirty(player.gameObject);

            Debug.Log($"Teleported to {targetTransform.name}");
        }
        else
        {
            Debug.LogError("Teleportation failed: Player or Camera is null.");
            FindRequiredObjects();  // Attempt to re-find objects
        }
    }

    private void DisplayErrorMessage()
    {
        var messageLabel = new Label("Required objects not found in the scene.")
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleCenter,
                flexGrow = 1
            }
        };
        rootVisualElement.Add(messageLabel);
    }

    private void DisplayPlayModeMessage()
    {
        var messageLabel = new Label("Start the game to use the teleportation window.")
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleCenter,
                flexGrow = 1
            }
        };
        rootVisualElement.Add(messageLabel);
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