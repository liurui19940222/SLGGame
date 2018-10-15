using UnityEngine;
using UnityEditor;
using System.Collections;

public class AStarEditor : EditorWindow {

    [MenuItem("Tools/AStarEditor")]
    public static void OpenWindow()
    {
        GetWindow<AStarEditor>().Show();
    }
}
