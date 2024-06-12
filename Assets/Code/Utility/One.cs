using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//系統底層的基本功能
//包含重要訊息的輸出

public class One
{
    static public void MSG(string msg)
    {
        Debug.Log(msg);
    }
    static public void ERROR(string msg)
    {
        Debug.Log("ERROR!!!! " + msg);
    }
}
