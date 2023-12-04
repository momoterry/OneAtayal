using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReset : MonoBehaviour
{
    public string mapNameToRest;

    void OnTG(GameObject whoTG)
    {
        GameSystem.GetPlayerData().SaveMap(mapNameToRest, null);
        GameSystem.GetInstance().SaveData();

        SystemUI.ShowMessageBox(null, "Ãö¥d­«¸m: " + mapNameToRest);
    }
}
