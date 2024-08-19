using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : GlobalSystemBase
{
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
        if (instance.objMaps.ContainsKey(_name))
            return instance.objMaps[_name];
        One.ERROR("GameObject not exist " + _name);
        return null;
    } 

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 GameData 存在 ");
        instance = this;

        //foreach (GameObject o in GMDatas)
        //{
        //    objMaps.Add(o.name, o);
        //}
    }

    public override void InitSystem()
    {
        base.InitSystem();
        //foreach (GameObject o in GMDatas)
        //{
        //    objMaps.Add(o.name, o);
        //}

        foreach (DataGroup group in DataGroups)
        {
            foreach (GameObject o in group.ObjectRefs)
            {
                objMaps.Add(o.name, o);
            }
        }
    }
}
