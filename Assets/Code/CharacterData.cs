using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat
{
    public int LV = 1;
    public int Exp = 0;
}

public class CharacterData : MonoBehaviour
{

    protected int ExpMax = 1000;
    protected int expDefaultMax = 400;
    protected float expRatioPerLevel = 1.2f;

    protected CharacterStat myStat = new CharacterStat();
    protected PlayerData thePlayerData;

    //¤¶­±
    public int GetExp() { return myStat.Exp; }
    public int GetExpMax() { return ExpMax; }
    public int GetLV() { return myStat.LV; }

    public void OnKillEnemy(Enemy e)
    {
        AddExp(100);
    }
    public void AddExp(int value)
    {
        myStat.Exp += value;
        if (myStat.Exp < 0)
            myStat.Exp = 0;

        while (myStat.Exp > ExpMax)
        {
            myStat.Exp -= ExpMax;
            myStat.LV++;
            CalcMaxExp();

            print("Level Up!!! " + myStat.LV + " >> " + (myStat.LV + 1));
        }


        thePlayerData.SetMainCharacterData(myStat);
    }

    // Start is called before the first frame update
    void Start()
    {
        thePlayerData = GameSystem.GetPlayerData();
        if (!thePlayerData)
        {
            print("ERROR!!!! CharacterData Can not Get PlayerData !!!");
        }
        myStat = thePlayerData.GetMainChracterData();

        CalcMaxExp();
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

        ExpMax = (int)calcMax;
        //print("CalcMaxExp: " + ExpMax);
    }
}
