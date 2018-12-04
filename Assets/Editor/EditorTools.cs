using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools {

    [MenuItem("Tools/RemoveAllColliders")]
    public static void RemoveAllColliders()
    {
        var previousSelection = Selection.activeGameObject;
        var trans = previousSelection.GetComponentsInChildren<Transform>();
        Debug.Log("选择的:" + previousSelection.name);
        foreach (Transform obj in trans)
        {
            Collider collider = null;
            if (collider = obj.GetComponent<Collider>())
            {
                GameObject.DestroyImmediate(collider);
                Debug.Log("删除" + obj.name);
            }
        }
    }

}