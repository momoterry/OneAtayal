using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================
//TODO: 先嘗試看看以動態 Instace 的方式實作，之後考慮直接掛在 BattleSystem 下
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
