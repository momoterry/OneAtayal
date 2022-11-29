using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuTrigger : MonoBehaviour
{
    public LevelSelectMenu theMenu;
    protected LevelItemInfo[] itemInfos;
    public string[] levelIDs;

    private void Start()
    {
        itemInfos = new LevelItemInfo[levelIDs.Length];
        for (int i=0;i<levelIDs.Length; i++)
        {
            LevelInfo info = GameSystem.GetLevelManager().GetLevelInfo(levelIDs[i]);
            if (info == null)
            {
                print("ERROR!! LevelMenuTrigger has invalid ID: " + levelIDs[i]);
                continue;
            }
            itemInfos[i] = new LevelItemInfo();
            itemInfos[i].ID = info.ID;
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
            theMenu.OpenMenu(itemInfos);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }
}
