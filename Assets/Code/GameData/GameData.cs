using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameObject[] GMDatas;

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
        print("ERROR!!!! GameObject not exist " + _name);
        return null;
    } 

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 GameData 存在 ");
        instance = this;

        foreach (GameObject o in GMDatas)
        {
            objMaps.Add(o.name, o);
        }
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
