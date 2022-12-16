using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileReplaceTool))]
public class TileReplaceToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ReplaceTarget"))
        {
            TileReplaceTool replaceTool = (TileReplaceTool)target;
            replaceTool.ReplaceTarget();
        }
    }
}
