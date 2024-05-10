using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//�O�� Doll ���ƾڸ�ƪ��a��
//�]�t�d�B�z Global �� Doll �[�J���禡
//TODO: ���ӧ�W�s DollManager ......


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

public class DollData : MonoBehaviour
{
    public GameObject defautSpawnFX;
    public DollInfo[] DollInfos;
    public TextAsset csvDollData;
    public Sprite[] highRankIcons;
    public Material iconBlendMat;

    protected Dictionary<string, GameObject> theMapping = new Dictionary<string, GameObject>();

    protected Dictionary<string, DollInfo> theDollMapping = new Dictionary<string, DollInfo>();


    public GameObject GetDollRefByID(string dollID)
    {
        //if (!theMapping.ContainsKey(dollID))
        //{
        //    print("ERROR!!! Invalid dollID: " + dollID);
        //    return null;
        //}
        //return theMapping[dollID];
        if (!theDollMapping.ContainsKey(dollID))
        {
            print("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID].objRef;
    }

    public DollInfo GetDollInfoByID(string dollID)
    {
        if (!theDollMapping.ContainsKey(dollID))
        {
            print("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID];
    }

    void Awake()
    {
        //foreach ( GameObject doObj in DollRefs)
        //{
        //    Doll d = doObj.GetComponent<Doll>();
        //    if (d)
        //    {
        //        theMapping.Add(d.ID, doObj);
        //    }
        //}
        foreach (DollInfo dInfo in DollInfos)
        {
            if (dInfo != null && dInfo.objRef)
            {
                Doll d = dInfo.objRef.GetComponent<Doll>();
                if (d.ID != dInfo.dollID)
                    print("ERROR!!!! dInfo ID ���~!! " + dInfo.dollID);
                if (dInfo.icon == null)
                    dInfo.icon = d.icon;
                theDollMapping.Add(dInfo.dollID, dInfo);
            }
        }

        DollCSVData[] csvDolls = CSVReader.FromCSV<DollCSVData>(csvDollData.text);
        GameObject objRoot = new GameObject("DollRefRoot");
        objRoot.transform.parent = transform;
        objRoot.SetActive(false);
        foreach (DollCSVData data in csvDolls)
        {
            //print(data.Name + " base on: " + data.BaseID);
            if (data.DollID == data.BaseID)
            {
                print("�ثe���ݭn���¦ Doll .....");
                continue;
            }
            DollInfo baseInfo = theDollMapping[data.BaseID];
            if (baseInfo == null)
            {
                print("ERROR!!!! ���~�� BaseID: " + data.BaseID);
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
            print("Rank: " + data.Rank);
            if (data.Rank > 1 && highRankIcons[data.Rank - 2])
            {
                dInfo.icon = OneUtility.BlendSprite(baseInfo.icon, highRankIcons[data.Rank-2], iconBlendMat);
                d.icon = dInfo.icon;
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


    public bool AddDollByID(string ID, ref bool isToBackpack)
    {
        //TODO: �o��]���Ӹ� SpawnBattleDoll ��X
        isToBackpack = false;
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(ID);
        if (dollRef == null)
        {
            print("���ե[�J Doll ���~�A���O���T�� doll ID" + ID);
            return false;
        }

        DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();
        if (dm == null || GameSystem.GetPlayerData().GetCurrDollNum() >= GameSystem.GetPlayerData().GetMaxDollNum())
        {
            GameSystem.GetPlayerData().AddDollToBackpack(ID);
            isToBackpack = true;
            return true;
        }

        // Spawn Doll ����å[�J���C

        Vector3 pos = dm.transform.position + Vector3.back * 1.0f;

        if (defautSpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(defautSpawnFX, pos, false);

        GameObject dollObj = BattleSystem.SpawnGameObj(dollRef, pos);

        Doll theDoll = dollObj.GetComponent<Doll>();

        //TODO: ���ɤO�k�סA�] Action Ĳ�o�� Doll Spawn �A�i��|�� NavAgent �� Update
        NavMeshAgent dAgent = theDoll.GetComponent<NavMeshAgent>();
        if (dAgent)
        {
            dAgent.updateRotation = false;
            dAgent.updateUpAxis = false;
            dAgent.enabled = false;
        }

        if (!theDoll.TryJoinThePlayer(DOLL_JOIN_SAVE_TYPE.FOREVER))
        {
            print("Woooooooooops.......");
            return false;
        }

        return true;
    }

    public GameObject SpawnBattleDoll(string ID, Vector3 pos)
    {
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(ID);
        //GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(ID);
        if (dInfo == null)
        {
            print("���ե[�J Doll ���~�A���O���T�� doll ID" + ID);
            return null;
        }
        GameObject dollRef = dInfo.objRef;

        //DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();
        //if (dm == null || GameSystem.GetPlayerData().GetCurrDollNum() >= GameSystem.GetPlayerData().GetMaxDollNum())
        //{
        //    GameSystem.GetPlayerData().AddDollToBackpack(ID);
        //    return true;
        //}

        // Spawn Doll ����å[�J���C

        //Vector3 pos = dm.transform.position + Vector3.back * 1.0f;

        if (defautSpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(defautSpawnFX, pos, false);

        GameObject dollObj = BattleSystem.SpawnGameObj(dollRef, pos);
        Doll theDoll = dollObj.GetComponent<Doll>();

        dollObj.name = dInfo.dollID;
        //dollObj.SetActive(true);
        //if (dInfo.GetType() == typeof(DollInfoEx))
        //{
        //    DollInfoEx dInfoEx = (DollInfoEx)dInfo;
        //    theDoll.AttackInit = dInfoEx.ATK;
        //    HitBody h = dollObj.GetComponent<HitBody>();
        //    h.HP_Max = dInfoEx.HP;
        //}

        //TODO: ���ɤO�k�סA�] Action Ĳ�o�� Doll Spawn �A�i��|�� NavAgent �� Update
        NavMeshAgent dAgent = theDoll.GetComponent<NavMeshAgent>();
        if (dAgent)
        {
            dAgent.updateRotation = false;
            dAgent.updateUpAxis = false;
            dAgent.enabled = false;
        }

        if (!theDoll.TryJoinThePlayer(DOLL_JOIN_SAVE_TYPE.BATTLE))
        {
            print("Woooooooooops.......");
            return null;
        }

        return dollObj;
    }
}