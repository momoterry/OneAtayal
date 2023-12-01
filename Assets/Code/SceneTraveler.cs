using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTraveler : MonoBehaviour
{
    static string sceneToGo = "";
    static string entraceToGo = "";
    static public void GotoScene(string sceneName, string entraceName)
    {
        sceneToGo = sceneName;
        entraceToGo = entraceName;
        SceneManager.LoadScene(sceneName);
        //BattleSystem bs = BattleSystem.GetInstance();
        //print("SceneToGo:LoadScene Done..... BattleSystem.MapGenetatorBase: " + bs.theMG);
        //if (bs != null) {
        //    MapGeneratorBase mg = bs.GetMapGenerator();
        //    if (mg && mg.entraceList.Length > 0)
        //    {
        //        for (int i = 0; i < mg.entraceList.Length; i++)
        //        {
        //            print("....Check Entrance: " + mg.entraceList[i].name);
        //            if (mg.entraceList[i].name == entraceName)
        //            {
        //                print("....Check Entrance: 成功 !!" + mg.entraceList[i].name);
        //                bs.initPlayerPos = mg.entraceList[i].pos;
        //            }
        //        }
        //    }
        //}
    }

    private void Update()
    {
        if (sceneToGo != "")
        {
            //print("SceneTraveler: 有 sceneToGo:" + sceneToGo + " _ " + entraceToGo);
            BattleSystem bs = BattleSystem.GetInstance();
            if (bs != null && bs.theMG != null)
            {
                //print("SceneTraveler: MG = " + bs.theMG);
                MapGeneratorBase mg = bs.GetMapGenerator();
                if (mg && mg.entraceList.Length > 0)
                {
                    for (int i = 0; i < mg.entraceList.Length; i++)
                    {
                        //print("....Check Entrance: " + mg.entraceList[i].name);
                        if (mg.entraceList[i].name == entraceToGo)
                        {
                            //print("....Check Entrance: 成功 !!" + mg.entraceList[i].name);
                            bs.initPlayerPos = mg.entraceList[i].pos;
                        }
                    }
                }
            }

            sceneToGo = "";
            entraceToGo = "";
        }
    }
}
