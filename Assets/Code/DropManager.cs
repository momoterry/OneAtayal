using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DropMapInfo;

[System.Serializable]
public class DropMapping
{
    public int ID;
    public float dropRatio = 100.0f;
    public GameObject objRef;
    public int exp = 0;
    public string Desc;     //只是方便表格編輯
}

[System.Serializable]
public class DropMapInfo
{
    public int ID;
    [System.Serializable]
    public class DropData
    {
        public float dropPercent;
        public GameObject objRef;
    }
    public DropData[] drops;
    public int exp = 0;
    public string Desc;     //只是方便表格編輯
}

public class DropManager : MonoBehaviour
{
    public DropMapping[] dropMappingArray;
    public DropMapInfo[] dropMapInfos;

    protected static DropManager instance = null;
    public static DropManager GetInstance() { return instance; }

    protected Dictionary<int, DropMapping> dropMap = new Dictionary<int, DropMapping>();
    protected Dictionary<int, DropMapInfo> dropInfoMap = new Dictionary<int, DropMapInfo>();

    //public DropManager() : base()
    //{
    //    //if (instance != null)
    //    //    print("ERROR !! 超過一份 DropManager 存在 ");
    //    //instance = this;
    //}

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 DropManager 存在 ");
        instance = this;

        foreach (DropMapping dm in dropMappingArray)
        {
            dropMap.Add(dm.ID, dm);
        }
        foreach (DropMapInfo info in dropMapInfos)
        {
            dropInfoMap.Add(info.ID, info);
        }
    }

    public int GetExpByID(int ID)
    {
        if (dropMap.ContainsKey(ID))
        {
            return dropMap[ID].exp;
        }
        else if (dropInfoMap.ContainsKey(ID))
        {
            return dropInfoMap[ID].exp;
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
        else if (dropInfoMap.ContainsKey(ID))
        {
            //print("Try Drop by dropInfoMap: " + ID);
            DropMapInfo info = dropInfoMap[ID];
            GameObject objRef = GetRandomObjRef(info.drops);
            if (objRef != null)
            {
                BattleSystem.GetInstance().SpawnGameplayObject(objRef, pos);
            }
        }
    }

    protected GameObject GetRandomObjRef(DropData[] data)
    {
        float rd = Random.Range(0, 100);
        float sum = 0;
        for ( int i = 0; i < data.Length; i++)
        {
            sum += data[i].dropPercent;
            if (rd < sum)
            {
                return data[i].objRef;
            }
        }
        return null;
    }

}
