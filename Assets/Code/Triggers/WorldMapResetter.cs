using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapResetter : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {
        SystemUI.ShowYesNoMessageBox(OnMessageConfirm, "�@�ɦa�ϭ��]��?");
    }

    public void OnMessageConfirm(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            print("WorldMapResetter: �}�l���]");
            GameSystem.GetPlayerData().ResetWorldMap();
            GameSystem.GetInstance().SaveData();
        }
    }

}
