using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 目前先實作成 Enemy 專用的 Buff 靈氣
// 等和 DollBuff 整合後，可以成為通用的 BuffAura

public class BuffAura : MonoBehaviour
{
    public BuffBase[] buffs;
    protected FACTION_GROUP group = FACTION_GROUP.ENEMY;    //TODO: 之後也支援玩家方

    public GameObject auraFXRef;

    protected List<GameObject> objListInArea = new List<GameObject>();
    protected List<GameObject> toClear = new List<GameObject>();

    protected float checkTime = 0;

    private void Update()
    {
        checkTime -= Time.time;
        if (checkTime < 0)
        {
            UpdateList();
            checkTime = 0.1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        //print("Enemuy In !!");

        //BuffReceiver br = other.gameObject.GetComponent<BuffReceiver>();

        //if (br)
        //{
        //    foreach (BuffBase buff in buffs)
        //    {
        //        br.AddBuff(buff);
        //        //other.gameObject.SendMessage("AddBuff", buff);
        //    }
        //    br.AddGroundEffect(auraFXRef);
        //}

        AddAura(other.gameObject);

        objListInArea.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        //BuffReceiver br = other.gameObject.GetComponent<BuffReceiver>();

        //if (br)
        //{
        //    foreach (BuffBase buff in buffs)
        //    {
        //        br.RemoveBuff(buff);
        //        //other.gameObject.SendMessage("RemoveBuff", buff);
        //    }
        //    br.RemoveGroundEffect(auraFXRef);
        //}
        RemoveAura(other.gameObject);

        objListInArea.Remove(other.gameObject);
        print("EnmeyOut Total = " + objListInArea.Count);
    }

    protected void AddAura(GameObject target)
    {
        BuffReceiver br = target.GetComponent<BuffReceiver>();

        if (br)
        {
            foreach (BuffBase buff in buffs)
            {
                br.AddBuff(buff);
                //other.gameObject.SendMessage("AddBuff", buff);
            }
            br.AddGroundEffect(auraFXRef);
        }
    }

    protected void RemoveAura(GameObject target)
    {
        BuffReceiver br = target.GetComponent<BuffReceiver>();

        if (br)
        {
            foreach (BuffBase buff in buffs)
            {
                br.RemoveBuff(buff);
            }
            br.RemoveGroundEffect(auraFXRef);
        }
    }

    protected void ClearAllAura()
    {
        foreach (GameObject o in objListInArea)
        {
            if (o != null)
                RemoveAura(o);
        }
    }

    protected void RestartAllAura()
    {
        foreach (GameObject o in objListInArea)
        {
            if (o != null)
                AddAura(o);
        }
    }

    //TODO: 等支援 Doll 之後，再來處理復活時的 Aura 開關

    //private void OnEnable()
    //{
    //    print("OnEnable ................");
    //    RestartAllAura();
    //}

    //private void OnDisable()
    //{
    //    print("OnDisable ................");
    //    ClearAllAura();
    //}

    private void OnDestroy()
    {
        //print("OnDestroy ................");
        ClearAllAura();
    }

    //==================== TODO: 參數化 ===================
    protected virtual bool CheckGameObject(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
            return true;

        return false;
    }

    protected void UpdateList()
    {
        objListInArea.RemoveAll(item => item == null);
    }

}
