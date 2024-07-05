using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSystemBase : MonoBehaviour
{
    public int InitOrder = 100;     //數字越小越先處理
    virtual public void InitSystem() { }

    static public int Compare(GlobalSystemBase A, GlobalSystemBase B)
    {
        return A.InitOrder - B.InitOrder;
    }
}
