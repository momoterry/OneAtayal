using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    protected int Money = 1000;
    protected int Level = 1;
    protected int Exp = 0;
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
}
