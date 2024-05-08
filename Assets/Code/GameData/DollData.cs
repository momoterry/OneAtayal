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

[System.Serializable]
public class DollCSVData
{
    public string DollID;
    public string BaseID;
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
    // Start is called before the first frame update

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
            DollInfo dInfo = new DollInfo();
            dInfo.dollID = data.DollID;
            dInfo.dollName = data.Name;
            dInfo.dollDesc = data.Desc;
            dInfo.summonCost = data.SummonCost;
            GameObject o = Instantiate(baseInfo.objRef, transform);
            o.name = data.DollID;
            o.SetActive(false);
            Doll d = o.GetComponent<Doll>();
            d.AttackInit = data.ATK;
            HitBody h = o.GetComponent<HitBody>();
            h.HP_Max = data.HP;
            dInfo.objRef = o;
            theDollMapping.Add(dInfo.dollID, dInfo);
        }

    }


    public bool AddDollByID(string ID, ref bool isToBackpack)
    {
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
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(ID);
        if (dollRef == null)
        {
            print("���ե[�J Doll ���~�A���O���T�� doll ID" + ID);
            return null;
        }

        DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();
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