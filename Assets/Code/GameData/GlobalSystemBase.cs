using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSystemBase : MonoBehaviour
{
    public int InitOrder = 100;     //�Ʀr�V�p�V���B�z
    virtual public void InitSystem() { }

    static public int Compare(GlobalSystemBase A, GlobalSystemBase B)
    {
        return A.InitOrder - B.InitOrder;
    }
}
