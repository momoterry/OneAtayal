using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTraveler : MonoBehaviour
{
    static string sceneToGo = "";
    static string entraceToGo = "";
    static string backSceneToGo = "";
    static string backEntranceToGo = "";

    static public void GotoScene(string sceneName, string entraceName)
    {
        SceneManager.LoadScene(sceneName);
        if (entraceName != "")
        {
            sceneToGo = sceneName;
            entraceToGo = entraceName;
            //needSetBattleSystem = true;
            BattleSystem.RegisterAwakeCallBack(SetupBattleSystem);
        }
    }

    static public void GotoSceneWithBackInfo(string sceneName, string entraceName, string backScene, string backEntrace)
    {
        SceneManager.LoadScene(sceneName);
        if (backScene != "" || entraceName != "")
        {
            sceneToGo = sceneName;
            entraceToGo = entraceName;
            backSceneToGo = backScene;
            backEntranceToGo = backEntrace;
            BattleSystem.RegisterAwakeCallBack(SetupBattleSystem);
        }
    }


    static void SetupBattleSystem(BattleSystem bs)
    {
        //print(sceneToGo + "SetupBattleSystem");
        //BattleSystem bs = BattleSystem.GetInstance();

        if (bs != null)
        {
            //print("SceneTraveler: MG = " + bs.theMG);
            MapGeneratorBase mg = bs.theMG;
            if (entraceToGo != "" && mg.entraceList != null && mg.entraceList.Length > 0)
            {
                for (int i = 0; i < mg.entraceList.Length; i++)
                {
                    if (mg.entraceList[i].name == entraceToGo)
                    {
                        bs.initPlayerPos = mg.entraceList[i].pos;
                        if (Camera.main)    //暴力法移動位置，應該透過 BattleCamera
                        {
                            Vector3 newPos = mg.entraceList[i].pos.position;
                            Camera.main.transform.position = new Vector3(newPos.x, Camera.main.transform.position.y, newPos.z);
                        }
                    }
                }
            }

            if (backSceneToGo != "")
            {
                //print("本來的回家點: " + bs.backScene);
                //print("設定回家點: " + backSceneToGo + " - " + backEntranceToGo);
                bs.backScene = backSceneToGo;
                bs.backEntrance = backEntranceToGo;
            }

        }

        sceneToGo = "";
        entraceToGo = "";
        backSceneToGo = "";
        backEntranceToGo = "";
    }
}
