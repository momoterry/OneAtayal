using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public DollData theDollData;

    protected int Money = 1000;
    protected int Level = 1;
    protected int Exp = 0;

    protected List<string> usingDollList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ¤¶­±
    public int GetMoney() { return Money; }
    public int AddMoney (int value)
    {
        Money += value;
        if (Money < 0)
            Money = 0;
        
        return Money;
    }

    public int AddExp( int value)
    {
        Exp += value;
        if (Exp < 0)
            Exp = 0;

        //TODO: ¤É¯Å§@·~

        return Exp;
    }

    public void AddUsingDoll( string dollID)
    {
        usingDollList.Add(dollID);

        print("================== usingDollList: " + usingDollList.Count);
        //foreach (string s in usingDollList)
        //{
        //    print("usingDollList: " + dollID);
        //}
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
