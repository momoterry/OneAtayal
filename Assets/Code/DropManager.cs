using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropMapping
{
    public int ID;
    public float dropRatio = 100.0f;
    public GameObject objRef;
    public int exp = 0;
    public string Desc;     //只是方便表格編輯
}

public class DropManager : MonoBehaviour
{
    public DropMapping[] dropMappingArray;

    protected static DropManager instance = null;
    public static DropManager GetInstance() { return instance; }

    protected Dictionary<int, DropMapping> dropMap = new Dictionary<int, DropMapping>();

    public DropManager() : base()
    {
        //if (instance != null)
        //    print("ERROR !! 超過一份 DropManager 存在 ");
        //instance = this;
    }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 DropManager 存在 ");
        instance = this;

        foreach (DropMapping dm in dropMappingArray)
        {
            dropMap.Add(dm.ID, dm);
        }
    }

    public int GetExpByID(int ID)
    {
        if (dropMap.ContainsKey(ID))
        {
            return dropMap[ID].exp;
        }
        return 0;
    }
    public void DoDropByID(int ID, Vector3 pos)
    {
        //print("Try Drop: " + ID);
        if (dropMap.ContainsKey(ID))
        { 
            DropMapping dm = dropMap[ID];
            if (dm!=null)
            {
                float rd = Random.Range(0, 100.0f);
                if (rd <= dm.dropRatio)
                {
                    //print("Drop !!!!!!!!!!!!!!!!!!!!!!!");
                    //GameObject newDrop = Instantiate(dm.objRef, pos, Quaternion.Euler(90, 0, 0), null);
                    BattleSystem.GetInstance().SpawnGameplayObject(dm.objRef, pos);
                }
            }        
        }
    }

}
