using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class DebugEditorWindow : EditorWindow
{
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
    }

    private void InitializeUxmlTemplate()
    {
        rootVisualElement.Clear();
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/DebugEditorView.uxml");
        template.CloneTree(rootVisualElement);
    }
}
