using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterStat
{
    public int LV = 1;
    public int Exp = 0;
    public int ExpMax = 1000;
    public int DollMax = 2;
}

public class CharacterData : MonoBehaviour
{

    //protected int ExpMax = 1000;
    protected int expDefaultMax = 300;
    protected float expRatioPerLevel = 1.25f;

    protected CharacterStat myStat = new CharacterStat();
    protected PlayerData thePlayerData;

    //介面
    public int GetExp() { return myStat.Exp; }
    public int GetExpMax() { return myStat.ExpMax; }
    public int GetLV() { return myStat.LV; }

    public void OnKillEnemy(Enemy e)
    {
        //暴力法  TODO: 用表格設定經驗值
        if (e.GetID() > 3000)
            AddExp(15);
        else
            AddExp(100);
    }
    public void AddExp(int value)
    {
        myStat.Exp += value;
        if (myStat.Exp < 0)
            myStat.Exp = 0;

        while (myStat.Exp >= myStat.ExpMax)
        {
            myStat.Exp -= myStat.ExpMax;
            myStat.LV++;
            CalcMaxExp();
            CalcMaxDoll();

            //print("Level Up!!! " + myStat.LV + " >> " + (myStat.LV + 1));
        }


        thePlayerData.SetMainCharacterData(myStat);
    }

    public void SetupStat()
    {
        print("CharacterData Init !!");
        thePlayerData = GameSystem.GetPlayerData();
        if (!thePlayerData)
        {
            print("ERROR!!!! CharacterData Can not Get PlayerData !!!");
        }
        myStat = thePlayerData.GetMainChracterData();

        CalcMaxExp();
        CalcMaxDoll();

        thePlayerData.SetMainCharacterData(myStat); //因為要存回 Max 值
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupStat();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void CalcMaxExp()
    {
        float defaultMax = expDefaultMax;
        float ratioPerLevel = expRatioPerLevel;
        float calcMax = defaultMax * Mathf.Pow(ratioPerLevel, (float)(myStat.LV - 1));

        myStat.ExpMax = (int)calcMax;
        //print("CalcMaxExp: " + ExpMax);
    }

    protected void CalcMaxDoll()
    {
        myStat.DollMax = myStat.LV + 3; //初始改成 4 隻
    }

}
