using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneStaticManager))]
public class SceneStaticEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set up Order by Y"))
        {
            SceneStaticManager theManager = (SceneStaticManager)target;
            theManager.SetupSorting();
        }
    }
}
