using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapResetter : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {
        SystemUI.ShowYesNoMessageBox(OnMessageConfirm, "世界地圖重設嗎?");
    }

    public void OnMessageConfirm(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            print("WorldMapResetter: 開始重設");
            GameSystem.GetPlayerData().ResetWorldMap();
            GameSystem.GetInstance().SaveData();
        }
    }

}
