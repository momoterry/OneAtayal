using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//系統底層的基本功能
//包含重要訊息的輸出

public class One
{
    static public string GetLogs()
    {
        string all = "";
        for (int i=0; i<logs.Count; i++)
        {
            all += (logs[i] + "\n");
        }
        return all;
    }

    static public void LOG(string msg)
    {
        Debug.Log(msg);
        AddLog(msg);
    }

    static public void ERROR(string msg)
    {
        string eMsg = "ERROR!!!! " + msg;
        Debug.Log(eMsg);
        AddLog(eMsg);
    }

    static protected List<string> logs = new List<string>();
    static protected int logTotal = 0;

    static protected void AddLog(string logMsg)
    {
        logs.Add(logMsg);
        if (logs.Count > 50)
            logs.RemoveAt(0);
    }

}
