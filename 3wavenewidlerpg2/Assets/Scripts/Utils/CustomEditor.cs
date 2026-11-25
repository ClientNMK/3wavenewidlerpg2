using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class CustomEditor : EditorWindow
{
    [MenuItem("CustomEditor/Example")]
    static void Open()
    {
        GetWindow<CustomEditor>();
    }

    void OnGUI()
    {
        Display();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(true);

        Display();

        EditorGUI.EndDisabledGroup();
    }

    void Display()
    {
        EditorGUILayout.ToggleLeft("Toggle", false);
        EditorGUILayout.IntSlider(0, 10, 0);
        GUILayout.Button("Button");
    }
}
#endif
