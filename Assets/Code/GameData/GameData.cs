using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : GlobalSystemBase
{
    public const string HEAL_FX = "HEAL_FX";
    public const string DOLL_SUMMON = "DOLL_SUMMON";
    public const string MONEY_ADD = "MONEY_ADD";

    [System.Serializable]
    public class ID_GameData
    {
        public string ID;
        public GameObject ObjectRef;
    }
    public ID_GameData[] ID_Group;
    [System.Serializable]
    public class DataGroup{
        public string GroupName;
        public GameObject[] ObjectRefs;
    }
    //public GameObject[] GMDatas;
    public DataGroup[] DataGroups;

    protected Dictionary<string, GameObject> objMaps = new Dictionary<string, GameObject>();

    static GameData instance = null;
    static public GameData GetInstance()
    {
        return instance;
    }

    static public GameObject GetObjectRef(string _name)
    {
        //One.LOG("GameData.instance = " + instance);
        if (instance.objMaps.ContainsKey(_name))
            return instance.objMaps[_name];
        One.ERROR("GameObject not exist " + _name);
        return null;
    } 

    //private void Awake()
    //{
    //    if (instance != null)
    //        print("ERROR !! 超過一份 GameData 存在 ");
    //    instance = this;

    //    //foreach (GameObject o in GMDatas)
    //    //{
    //    //    objMaps.Add(o.name, o);
    //    //}
    //}

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! 超過一份 GameData 存在 ");
        instance = this;

        base.InitSystem();
        //foreach (GameObject o in GMDatas)
        //{
        //    objMaps.Add(o.name, o);
        //}

        foreach (ID_GameData data in ID_Group)
        {
            objMaps.Add(data.ID, data.ObjectRef);
        }

        foreach (DataGroup group in DataGroups)
        {
            foreach (GameObject o in group.ObjectRefs)
            {
                objMaps.Add(o.name, o);
            }
        }

        //One.LOG("objMaps:");
        //foreach (KeyValuePair<string, GameObject> k in objMaps)
        //{
        //    One.LOG(k.Key + " => " + k.Value);
        //}
    }
}
