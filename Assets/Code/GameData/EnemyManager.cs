using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public string EnemyID;
        public string BaseRef;
        public int Rank;
        public float HP;
        public float ATK;
        public int DropID;
        public GameObject objRef;
    }

    [System.Serializable]
    public class BaseRefInfo
    {
        public string refID;
        public GameObject objRef;
    }
    public TextAsset csvFile;
    public BaseRefInfo[] baseRefs;

    protected Dictionary<string, EnemyData> enemyMap = new Dictionary<string, EnemyData>();
    protected Dictionary<string, GameObject> refMap = new Dictionary<string, GameObject>();

    static EnemyManager instance;
    static public EnemyManager GetInstance() { return instance; }

    public GameObject SpawnEnemyByID(string _ID, Vector3 _pos)
    {
        if (!enemyMap.ContainsKey(_ID))
        {
            print("ERROR!!!! No Enemy ID: " + _ID);
            return null;
        }

        EnemyData data = enemyMap[_ID];
        GameObject o = BattleSystem.SpawnGameObj(data.objRef, _pos);

        return o;
    }

    void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 EnemyManager 存在 ... ");
        instance = this;


        for (int i=0; i<baseRefs.Length; i++)
        {
            refMap.Add(baseRefs[i].refID, baseRefs[i].objRef);
        }


        EnemyData[] enemyDatas = CSVReader.FromCSV<EnemyData>(csvFile.text);
        for (int i = 0; i < enemyDatas.Length; i++)
        {
            enemyDatas[i].objRef = refMap[enemyDatas[i].BaseRef];
            print("Enemy " + enemyDatas[i].objRef);
        }
    }
}
