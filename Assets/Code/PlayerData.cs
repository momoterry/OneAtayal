using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public DollData theDollData;

    protected int Money = 1000;

    //Main Character Data: 
    //protected int LV = 1;
    //protected int Exp = 0;
    protected CharacterStat mainCharacterStat = new CharacterStat();

    protected List<string> usingDollList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMainCharacterData(CharacterStat stat)
    {
        mainCharacterStat = stat;
    }

    public CharacterStat GetMainChracterData()
    {
        return mainCharacterStat;
    }

    // ¤¶­±
    public int GetMoney() { return Money; }
    public int AddMoney(int value)
    {
        Money += value;
        if (Money < 0)
            Money = 0;

        return Money;
    }

    //public int AddExp( int value)
    //{
    //    Exp += value;
    //    if (Exp < 0)
    //        Exp = 0;

    //    //TODO: ¤É¯Å§@·~

    //    return Exp;
    //}

    public int GetMaxDollNum()
    {
        return mainCharacterStat.DollMax;
    }

    public int GetCurrDollNum()
    {
        return usingDollList.Count;
    }

    public void AddUsingDoll( string dollID)
    {
        usingDollList.Add(dollID);
    }

    public string[] GetAllUsingDolls()
    {
        if (usingDollList.Count > 0)
        {
            string[] allDolls = new string[usingDollList.Count];
            int i = 0;
            foreach( string ds in usingDollList)
            {
                allDolls[i] = ds;
                i++;
            }

            return allDolls;
        }

        return null;
    }

    public GameObject GetDollRefByID(string ID)
    {
        if (theDollData)
        {
            return theDollData.GetDollRefByID(ID);
        }
        return null;
    }
    
}
