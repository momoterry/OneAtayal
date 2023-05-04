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
        if (selectedObjects.Length > 0)
        {
            foreach (GameObject o in selectedObjects)
            {
                processOneObject(o);
            }
            return;
        }
        Debug.Log("---- 沒有選到的物件，只進行 MapGenerator 測試 ");

        //var nModifier = Object.FindObjectsOfType<Unity.AI.Navigation.NavMeshModifier>();
        //Debug.Log("============ 總共找到的 Modifier 數: " + nModifier.Length);

        var nMGs = Object.FindObjectsOfType<MapGeneratorBase>();
        Debug.Log("總共找到的 MapGeneratorBase 數: " + nMGs.Length);
        if (nMGs.Length > 1)
            Debug.Log("ERROR!!!! 超過一份 MapGeneratorBase 存在 !!");
        if (nMGs.Length == 0)
        {
            Debug.Log("沒有 MapGeneratorBase 存在，結束");
            return;
        }

        //尋找之前的 Nav2D Root，這算是暴力法
        GameObject gNav;
        NavMeshPlus.Components.NavMeshSurface theSurface = Object.FindAnyObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        if (theSurface)
        {
            theSurface.defaultArea = 1;
            Debug.Log("已經有 Surface 存在，改一下 Defaut Area, Bye Bye");
            EditorUtility.SetDirty(theSurface.gameObject);
            return;
        }
        Grid g = Object.FindAnyObjectByType<Grid>();
        if (g)
        {
            gNav = g.transform.root.gameObject;
        }
        else
            gNav = nMGs[0].gameObject;

        theSurface = gNav.AddComponent<NavMeshPlus.Components.NavMeshSurface>();
        theSurface.defaultArea = 1;
        gNav.AddComponent<NavMeshPlus.Extensions.CollectSources2d>();

        foreach ( MapGeneratorBase mg in nMGs)
        {
            //Debug.Log("MG: " + mg.name + " => " + mg.theSurface2D);
            if (mg.theSurface2D)
            {
                //Debug.Log("theSurface2D : " + mg.theSurface2D.gameObject);
            }
            mg.theSurface2D = theSurface;
            Debug.Log("MG: " + mg.name + " => " + mg.theSurface2D);
            EditorUtility.SetDirty(mg.gameObject);
        }

    }

    static void processOneObject( GameObject obj)
    {
        Debug.Log("---- 處理物件 : " + obj.name);

        var oldMS = obj.GetComponentsInChildren<Unity.AI.Navigation.NavMeshModifier>(true);
        Debug.Log("找到的舊元件 : " + oldMS.Length);
        foreach (Unity.AI.Navigation.NavMeshModifier m in oldMS)
        {
            NavMeshPlus.Components.NavMeshModifier newM = m.gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
            newM.overrideArea = m.overrideArea;
            newM.area = m.area;
            Debug.Log("加入新的 Modifier: " + newM + " -- " + newM.area);

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
