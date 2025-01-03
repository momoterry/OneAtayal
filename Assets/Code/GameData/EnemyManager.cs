using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyManager : GlobalSystemBase
{
    [System.Serializable]
    public class EnemyData
    {
        public string EnemyID;
        public string BaseRef;
        public int LV;
        public float HP;
        public float ATK;
        public int DropID;
        public GameObject objRef;
    }

    //[System.Serializable]
    //public class BaseRefInfo
    //{
    //    public string refID;
    //    public GameObject objRef;
    //}
    public TextAsset csvFile;
    //public BaseRefInfo[] baseRefs;

    protected Dictionary<string, EnemyData> enemyMap = new Dictionary<string, EnemyData>();
    protected Dictionary<string, GameObject> refMap = new Dictionary<string, GameObject>();

    static EnemyManager instance;
    static public EnemyManager GetInstance() { return instance; }

    public GameObject SpawnEnemyByID(string _ID, Vector3 _pos, int _LV = 1)
    {
        if (_LV > 1)
        {
            _ID = _ID + _LV;
            //print("高級 Enemy: " + _ID);
        }
        if (!enemyMap.ContainsKey(_ID))
        {
            One.ERROR("No Enemy ID: " + _ID);
            return null;
        }

        EnemyData data = enemyMap[_ID];
        GameObject o = BattleSystem.SpawnGameObj(data.objRef, _pos);

        Enemy e = o.GetComponent<Enemy>();
        if (e != null)
        {
            e.Attack = data.ATK;
            e.MaxHP = data.HP;
            e.ID = data.DropID;
        }
        else
            One.ERROR("No Enemy Component: " + _ID);

        return o;
    }

    float[] defaultUpgrate = { 1.0f, 1.0f, 1.5f, 2.25f, 3.375f, 5.0625f, 7.59375f, 11.390625f};

    public GameObject SpawnEnemyByRef(GameObject objRef, Vector3 _pos, int _LV = 1)
    {
        //if (_LV > 1)
        //{
        //    _ID = _ID + _LV;
        //    //print("高級 Enemy: " + _ID);
        //}
        //if (!enemyMap.ContainsKey(_ID))
        //{
        //    One.ERROR("No Enemy ID: " + _ID);
        //    return null;
        //}

        //EnemyData data = enemyMap[_ID];
        GameObject o = BattleSystem.SpawnGameObj(objRef, _pos);

        Enemy e = o.GetComponent<Enemy>();

        if (_LV > 1)
        {
            //print("產生敵人等級: " + _LV);
            if (_LV >= defaultUpgrate.Length)
            {
                One.LOG("ERROR!!!! 敵人等級超過上限!! " + _LV);
                _LV = defaultUpgrate.Length - 1;
            }
            float lvUpRate = defaultUpgrate[_LV];
            e.Attack *= lvUpRate;
            e.MaxHP *= lvUpRate;
        }

        return o;
    }

    //void Awake()
    //{
    //    if (instance != null)
    //        print("ERROR !! 超過一份 EnemyManager 存在 ... ");
    //    instance = this;
    //}

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! 超過一份 EnemyManager 存在 ... ");
        instance = this;
        base.InitSystem();


        EnemyData[] enemyDatas = CSVReader.FromCSV<EnemyData>(csvFile.text);
        //One.LOG("enemyDatas" + enemyDatas.Length);
        for (int i = 0; i < enemyDatas.Length; i++)
        {
            //One.LOG("Check " + enemyDatas[i].EnemyID + "base  = " + enemyDatas[i].BaseRef);
            enemyDatas[i].objRef = GameData.GetObjectRef(enemyDatas[i].BaseRef);
            if (enemyDatas[i].LV > 1)
                enemyDatas[i].EnemyID = enemyDatas[i].EnemyID + enemyDatas[i].LV;
            enemyMap.Add(enemyDatas[i].EnemyID, enemyDatas[i]);
            //One.LOG("Enemy " + enemyDatas[i].EnemyID + " => " + enemyDatas[i].BaseRef);
        }
    }

}
