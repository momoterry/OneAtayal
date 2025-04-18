using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//記錄 Doll 的數據資料的地方
//也負責處理 Global 的 Doll 加入等函式
//TODO: 應該改名叫 DollManager ......


[System.Serializable]
public class DollInfo
{
    public string dollID;
    public GameObject objRef;
    public string dollName;
    public string dollDesc;
    public Sprite icon;
    public int summonCost;
}

public class DollInfoEx : DollInfo
{
    public float ATK;
    public float HP;
}

[System.Serializable]
public class DollCSVData
{
    public string DollID;
    public string BaseID;
    public int Rank;
    public float ATK;
    public float HP;
    public string Name;
    public string Desc;
    public int SummonCost;
}

public class DollData : GlobalSystemBase
{
    public GameObject defautSpawnFX;
    public DollInfo[] DollInfos;
    public TextAsset csvDollData;
    public Sprite[] highRankIcons;
    public Material iconBlendMat;
    public GameObject hpBarWildRef;
    public GameObject hpBarWildManaRef;

    protected Dictionary<string, GameObject> theMapping = new Dictionary<string, GameObject>();

    protected Dictionary<string, DollInfo> theDollMapping = new Dictionary<string, DollInfo>();


    public GameObject GetDollRefByID(string dollID)
    {
        //if (!theMapping.ContainsKey(dollID))
        //{
        //    One.LOG("ERROR!!! Invalid dollID: " + dollID);
        //    return null;
        //}
        //return theMapping[dollID];
        if (!theDollMapping.ContainsKey(dollID))
        {
            One.LOG("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID].objRef;
    }

    public DollInfo GetDollInfoByID(string dollID)
    {
        if (!theDollMapping.ContainsKey(dollID))
        {
            One.LOG("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID];
    }

    //void Awake()
    //{
    //    //string strAll = "";
    //    foreach (DollInfo dInfo in DollInfos)
    //    {
    //        if (dInfo != null && dInfo.objRef)
    //        {
    //            Doll d = dInfo.objRef.GetComponent<Doll>();
    //            if (d.ID != dInfo.dollID)
    //                One.ERROR("dInfo ID 錯誤!! " + dInfo.dollID);
    //            if (dInfo.icon == null)
    //                dInfo.icon = d.icon;
    //            theDollMapping.Add(dInfo.dollID, dInfo);

    //            HitBody hb = dInfo.objRef.GetComponent<HitBody>();
    //            //string sep = "\t";
    //            //string str = dInfo.dollID + sep + dInfo.dollID + sep + 1 + sep + d.AttackInit + sep + hb.HP_Max + sep + dInfo.dollName + sep + dInfo.dollDesc + sep + 1 + "\n";
    //            //strAll += str;
    //        }
    //    }
    //    //print(strAll);

    //    DollCSVData[] csvDolls = CSVReader.FromCSV<DollCSVData>(csvDollData.text);
    //    GameObject objRoot = new GameObject("DollRefRoot");
    //    objRoot.transform.parent = transform;
    //    objRoot.SetActive(false);
    //    foreach (DollCSVData data in csvDolls)
    //    {
    //        //print(data.Name + " base on: " + data.BaseID);
    //        //if (data.DollID == data.BaseID)
    //        //{
    //        //    print("目前不需要放基礎 Doll .....");
    //        //    continue;
    //        //}
    //        DollInfo baseInfo = theDollMapping[data.BaseID];
    //        if (baseInfo == null)
    //        {
    //            One.ERROR("錯誤的 BaseID: " + data.BaseID);
    //            continue;
    //        }
    //        DollInfoEx dInfo = new DollInfoEx();
    //        dInfo.dollID = data.DollID;
    //        dInfo.dollName = data.Name;
    //        dInfo.dollDesc = data.Desc;
    //        dInfo.summonCost = data.SummonCost;
    //        dInfo.ATK = data.ATK;
    //        dInfo.HP = data.HP;
    //        //dInfo.objRef = baseInfo.objRef;
    //        GameObject o = Instantiate(baseInfo.objRef, objRoot.transform);
    //        o.name = data.DollID;
    //        //o.SetActive(false);
    //        Doll d = o.GetComponent<Doll>();
    //        d.ID = data.DollID;
    //        d.AttackInit = data.ATK;
    //        HitBody h = o.GetComponent<HitBody>();
    //        h.HP_Max = data.HP;
    //        dInfo.objRef = o;
    //        dInfo.icon = baseInfo.icon;
    //        //print("baseInfo icon: " + baseInfo.icon);
    //        if (data.Rank > 1 && highRankIcons[data.Rank - 2])
    //        {
    //            dInfo.icon = OneUtility.BlendSprite(baseInfo.icon, highRankIcons[data.Rank-2], iconBlendMat);
    //            d.icon = dInfo.icon;
    //        }
    //        if (data.DollID == data.BaseID)
    //        {
    //            //print("取代基礎 Doll ....." + data.DollID);
    //            theDollMapping.Remove(data.BaseID);
    //        }
    //        theDollMapping.Add(dInfo.dollID, dInfo);
    //    }

    //}

    public override void InitSystem()
    {
        base.InitSystem();
        //One.LOG("DollData  InitSystem");
        foreach (DollInfo dInfo in DollInfos)
        {
            if (dInfo != null && dInfo.objRef)
            {
                Doll d = dInfo.objRef.GetComponent<Doll>();
                if (d.ID != dInfo.dollID)
                    One.ERROR("dInfo ID 錯誤!! " + dInfo.dollID);
                if (dInfo.icon == null)
                    dInfo.icon = d.icon;
                theDollMapping.Add(dInfo.dollID, dInfo);

                HitBody hb = dInfo.objRef.GetComponent<HitBody>();
                //string sep = "\t";
                //string str = dInfo.dollID + sep + dInfo.dollID + sep + 1 + sep + d.AttackInit + sep + hb.HP_Max + sep + dInfo.dollName + sep + dInfo.dollDesc + sep + 1 + "\n";
                //strAll += str;
            }
        }
        //print(strAll);

        DollCSVData[] csvDolls = CSVReader.FromCSV<DollCSVData>(csvDollData.text);
        GameObject objRoot = new GameObject("DollRefRoot");
        objRoot.transform.parent = transform;
        objRoot.SetActive(false);
        foreach (DollCSVData data in csvDolls)
        {
            //print(data.Name + " base on: " + data.BaseID);
            //if (data.DollID == data.BaseID)
            //{
            //    print("目前不需要放基礎 Doll .....");
            //    continue;
            //}
            DollInfo baseInfo = theDollMapping[data.BaseID];
            if (baseInfo == null)
            {
                One.ERROR("錯誤的 BaseID: " + data.BaseID);
                continue;
            }
            DollInfoEx dInfo = new DollInfoEx();
            dInfo.dollID = data.DollID;
            dInfo.dollName = data.Name;
            dInfo.dollDesc = data.Desc;
            dInfo.summonCost = data.SummonCost;
            dInfo.ATK = data.ATK;
            dInfo.HP = data.HP;
            //dInfo.objRef = baseInfo.objRef;
            GameObject o = Instantiate(baseInfo.objRef, objRoot.transform);
            o.name = data.DollID;
            //o.SetActive(false);
            Doll d = o.GetComponent<Doll>();
            d.ID = data.DollID;
            d.AttackInit = data.ATK;
            HitBody h = o.GetComponent<HitBody>();
            h.HP_Max = data.HP;
            dInfo.objRef = o;
            dInfo.icon = baseInfo.icon;
            //print("baseInfo icon: " + baseInfo.icon);
            if (data.Rank > 1 && highRankIcons[data.Rank - 2])
            {
                dInfo.icon = OneUtility.BlendSprite(baseInfo.icon, highRankIcons[data.Rank - 2], iconBlendMat);
                d.icon = dInfo.icon;
            }
            if (data.DollID == data.BaseID)
            {
                //print("取代基礎 Doll ....." + data.DollID);
                theDollMapping.Remove(data.BaseID);
            }
            theDollMapping.Add(dInfo.dollID, dInfo);
        }
    }

    public DollInfo[] GetAllDollInfo()
    {
        DollInfo[] infos = new DollInfo[theDollMapping.Count];
        int i = 0;
        foreach (KeyValuePair<string, DollInfo> kvp in theDollMapping)
        {
            infos[i] = kvp.Value;
            i++;
        }
        return infos;
    }


    public void FixupBattleDollHPBar(GameObject dollObj, Doll theDoll)
    {
        Hp_BarHandler hh = dollObj.GetComponent<Hp_BarHandler>();
        if (hh)
        {
            hh.barRef = (theDoll is DollBeta && ((DollBeta)theDoll).isUsingMana) ? hpBarWildManaRef : hpBarWildRef;
        }
    }


    protected GameObject SpawnDollByID(string ID, Vector3 pos, DOLL_JOIN_SAVE_TYPE join_type)
    {
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(ID);
        if (dInfo == null)
        {
            print("嘗試加入 Doll 錯誤，不是正確的 doll ID" + ID);
            return null;
        }
        GameObject dollRef = dInfo.objRef;

        if (defautSpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(defautSpawnFX, pos, false);

        GameObject dollObj = BattleSystem.SpawnGameObj(dollRef, pos);
        Doll theDoll = dollObj.GetComponent<Doll>();

        dollObj.name = dInfo.dollID;

        //TODO: 先暴力法修，因 Action 觸發的 Doll Spawn ，可能會讓 NavAgent 先 Update
        NavMeshAgent dAgent = theDoll.GetComponent<NavMeshAgent>();
        if (dAgent)
        {
            dAgent.updateRotation = false;
            dAgent.updateUpAxis = false;
            dAgent.enabled = false;
        }

        //血條校正
        if (join_type == DOLL_JOIN_SAVE_TYPE.BATTLE)
        {
            FixupBattleDollHPBar(dollObj, theDoll);
            //Hp_BarHandler hh = dollObj.GetComponent<Hp_BarHandler>();
            //if (hh)
            //{
            //    hh.barRef = (theDoll is DollBeta && ((DollBeta)theDoll).isUsingMana) ? hpBarWildManaRef : hpBarWildRef;
            //}
        }

        if (!theDoll.TryJoinThePlayer(join_type))
        {
            print("Woooooooooops.......");
            return null;
        }

        return dollObj;
    }

    public bool AddForeverDollByID(string ID, ref bool isToBackpack)
    {
        isToBackpack = false;
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(ID);
        if (dollRef == null)
        {
            print("嘗試加入 Doll 錯誤，不是正確的 doll ID" + ID);
            return false;
        }

        DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();
        if (dm == null || GameSystem.GetPlayerData().GetCurrDollNum() >= GameSystem.GetPlayerData().GetMaxDollNum())
        {
            GameSystem.GetPlayerData().AddDollToBackpack(ID);
            isToBackpack = true;
            return true;
        }

        // Spawn Doll 實體並加入隊列

        Vector3 pos = dm.transform.position + Vector3.back * 1.0f;

        //if (defautSpawnFX)
        //    BattleSystem.GetInstance().SpawnGameplayObject(defautSpawnFX, pos, false);

        //GameObject dollObj = BattleSystem.SpawnGameObj(dollRef, pos);

        //Doll theDoll = dollObj.GetComponent<Doll>();

        ////TODO: 先暴力法修，因 Action 觸發的 Doll Spawn ，可能會讓 NavAgent 先 Update
        //NavMeshAgent dAgent = theDoll.GetComponent<NavMeshAgent>();
        //if (dAgent)
        //{
        //    dAgent.updateRotation = false;
        //    dAgent.updateUpAxis = false;
        //    dAgent.enabled = false;
        //}

        //if (!theDoll.TryJoinThePlayer(DOLL_JOIN_SAVE_TYPE.FOREVER))
        //{
        //    print("Woooooooooops.......");
        //    return false;
        //}

        GameObject dollObj = SpawnDollByID(ID, pos, DOLL_JOIN_SAVE_TYPE.FOREVER);

        return dollObj != null ? true : false;
    }

    public GameObject SpawnBattleDollByID(string ID, Vector3 pos)
    {
        return SpawnDollByID(ID, pos, DOLL_JOIN_SAVE_TYPE.BATTLE);
    }
}