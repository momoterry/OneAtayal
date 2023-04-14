using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat : MonoBehaviour
{
    static private BattleStat instance;

    protected Dictionary<string, float> dollDamageTotal = new Dictionary<string, float>();

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 BattleStat 存在 ");
        instance = this;
    }

    static public BattleStat GetInstance()
    {
        //if (instance == null)
        //{
        //    instance = new BattleStat();
        //}
        return instance;
    }

    //各種包裝成 Static 的函式
    static public void AddOneDamage(Damage damage, float damageDone)
    {
        GetInstance()._AddOneDamage(damage, damageDone);
    }
    public void _AddOneDamage(Damage damage, float damageDone)
    {
        if (damage.type == Damage.OwnerType.DOLL && damage.ID != "")
        {
            if (dollDamageTotal.ContainsKey(damage.ID))
            {
                dollDamageTotal[damage.ID] += damageDone;
            }
            else
            {
                dollDamageTotal.Add(damage.ID, damageDone);
            }
        }
    }

    public void DebugPrintAll()
    {
        foreach (KeyValuePair<string, float> kvp in dollDamageTotal)
        {
            print(kvp.Key + "造成了總傷: " + kvp.Value.ToString());
        }
    }

    public void DebugClearAll()
    {
        dollDamageTotal.Clear();
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
