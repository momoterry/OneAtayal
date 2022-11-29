using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//負責管理關卡開放順序，以及記錄關卡資料的系統
//關於開放與否的存檔，交給 PlayerData 的 Event 來記錄

[System.Serializable]
public class LevelInfo
{
    public string ID;
    public string sceneName;
    public string name;
    public int requireLevel;    //-1 表示 ??
}

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] mainLevels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
