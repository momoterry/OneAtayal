using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理需要連續通過多個關卡來完成的戰鬥，比如連續地城
//需要記錄必要的關卡參數，讓 MapGenerate 能透過參數變化來產生不同地表，一個 Scene 就可以產生不同的關卡

public class LevelGenInfoBase { }

public class ContinuousBattleManager : MonoBehaviour
{
    static ContinuousBattleManager instance;
    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 ContinuousBattleManager 存在: ");
        instance = this;
    }

}
