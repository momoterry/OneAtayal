using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuTrigger : MonoBehaviour
{
    public LevelSelectMenu theMenu;
    protected LevelItemInfo[] itemInfos;
    public string[] levelIDs;

    public string backScene = "";
    public string backEntrance = "";

    private void Start()
    {
        itemInfos = new LevelItemInfo[levelIDs.Length];
        for (int i=0;i<levelIDs.Length; i++)
        {
            LevelInfo info = GameSystem.GetLevelManager().GetLevelInfo(levelIDs[i]);
            if (info == null)
            {
                One.LOG("ERROR!! LevelMenuTrigger has invalid ID: " + levelIDs[i]);
                continue;
            }
            itemInfos[i] = new LevelItemInfo();
            itemInfos[i].ID = info.ID;
            itemInfos[i].levelType = info.type;
            itemInfos[i].scene = info.sceneName;
            itemInfos[i].name = info.prefix + " " + info.name;
            string requireStr = info.requireLevel >= 0 ? info.requireLevel.ToString() : "??";
            itemInfos[i].desc = "建議 LV : " + requireStr;
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu(itemInfos, backScene, backEntrance);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }
}
