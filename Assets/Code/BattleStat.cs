using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================
//TODO: �����լݬݥH�ʺA Instace ���覡��@�A����Ҽ{�������b BattleSystem �U
//==================

public class BattleStat : MonoBehaviour
{
    static private BattleStat instance;

    //protected 

    public BattleStat GetInstance()
    {
        if (instance == null)
        {
            instance = new BattleStat();
        }
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
