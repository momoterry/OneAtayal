using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 用來描述一個 MazeOneBase 迷宮參數的資料結構
// 目前主要用來支援 CSV 的檔案描述
// 以後可以跟 CMazeJsonData 也做某種程度上的整合




//一個 MazeOne Dungeion 的描述
public class MODungeonData
{
    public string DungeonID;
}

//MazeOne Dungeion 中每一「層」的描述

public class MODungeonStateData
{
    public int Level;       //第幾層
    public string SceneName;

}

public class MazeOneData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
