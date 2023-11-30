using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReset : MonoBehaviour
{
    public string mapNameToRest;

    void OnTG(GameObject whoTG)
    {
        GameSystem.GetPlayerData().SaveMap(mapNameToRest, null);

        SystemUI.ShowMessageBox(null, "���d���m: " + mapNameToRest);
    }
}
