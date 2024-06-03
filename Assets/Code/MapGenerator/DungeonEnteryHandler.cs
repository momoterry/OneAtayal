using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEnteryHandler : MonoBehaviour
{
    [System.Serializable]
    public class EnteryData
    {
        public string dungeonID;
        public string infoText;
    }

    public TextMesh enteryText;
    public CMODungeonPortal thePortal;
    

    public void SetEnteryData(EnteryData data)
    {
        if (enteryText)
            enteryText.text = data.infoText;
        if (thePortal)
        {
            thePortal.DungeonID = data.dungeonID;
            thePortal.hintLevelName = data.infoText;
        }
    }

}
