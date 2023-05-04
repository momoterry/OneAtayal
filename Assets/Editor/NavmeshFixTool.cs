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
        Debug.Log("---- �S����쪺����A�u�i�� MapGenerator ���� ");

        //var nModifier = Object.FindObjectsOfType<Unity.AI.Navigation.NavMeshModifier>();
        //Debug.Log("============ �`�@��쪺 Modifier ��: " + nModifier.Length);

        var nMGs = Object.FindObjectsOfType<MapGeneratorBase>();
        Debug.Log("�`�@��쪺 MapGeneratorBase ��: " + nMGs.Length);
        if (nMGs.Length > 1)
            Debug.Log("ERROR!!!! �W�L�@�� MapGeneratorBase �s�b !!");
        if (nMGs.Length == 0)
        {
            Debug.Log("�S�� MapGeneratorBase �s�b�A����");
            return;
        }

        //�M�䤧�e�� Nav2D Root�A�o��O�ɤO�k
        GameObject gNav;
        NavMeshPlus.Components.NavMeshSurface theSurface = Object.FindAnyObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        if (theSurface)
        {
            theSurface.defaultArea = 1;
            Debug.Log("�w�g�� Surface �s�b�A��@�U Defaut Area, Bye Bye");
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
