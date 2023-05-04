using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Drawing.Printing;

public static class FixNavmeshTool
{
    [MenuItem("One/Fix Navmesh")]
    static public void DoFix()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        foreach (GameObject o in selectedObjects)
        {
            processOneObject(o);
        }
        return;

        //var nModifier = Object.FindObjectsOfType<Unity.AI.Navigation.NavMeshModifier>();
        //Debug.Log("============ �`�@��쪺 Modifier ��: " + nModifier.Length);

        //foreach (Unity.AI.Navigation.NavMeshModifier m in nModifier)
        //{
        //    Debug.Log("�ª� Modifier: " + m  + " -- " + m.area);

        //    NavMeshPlus.Components.NavMeshModifier newM = m.gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        //    newM.overrideArea = m.overrideArea;
        //    newM.area = m.area;
        //    Debug.Log("�[�J�s�� Modifier: " + newM + " -- " + newM.area);
        //}

        //var newMs = Object.FindObjectsOfType<NavMeshPlus.Components.NavMeshModifier>();
        //Debug.Log("============ �`�@��쪺 �s�� Modifier ��: " + newMs.Length);
        //foreach (NavMeshPlus.Components.NavMeshModifier newM in newMs)
        //{
        //    Unity.AI.Navigation.NavMeshModifier oldM = newM.gameObject.GetComponent<Unity.AI.Navigation.NavMeshModifier>();
        //    if (oldM)
        //    {
        //        Debug.Log("�R���ª� Component: " + oldM);
        //        Object.DestroyImmediate(oldM);
        //    }
        //}

    }

    static void processOneObject( GameObject obj)
    {
        Debug.Log("---- �B�z���� : " + obj.name);

        var oldMS = obj.GetComponentsInChildren<Unity.AI.Navigation.NavMeshModifier>(true);
        Debug.Log("��쪺�¤��� : " + oldMS.Length);
        foreach (Unity.AI.Navigation.NavMeshModifier m in oldMS)
        {
            NavMeshPlus.Components.NavMeshModifier newM = m.gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
            newM.overrideArea = m.overrideArea;
            newM.area = m.area;
            Debug.Log("�[�J�s�� Modifier: " + newM + " -- " + newM.area);

            EditorUtility.SetDirty(m.gameObject);
            Object.DestroyImmediate(m);
        }
    }
}

//[CustomEditor(typeof(FixNavigatioin))]
//public class NavmeshFixTool : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        if (GUILayout.Button("Fix up Navigation"))
//        {

//        }
//    }
//}
