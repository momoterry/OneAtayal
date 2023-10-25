using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ヘ玡龟Θ Enemy 盡ノ Buff 艶
// 单㎝ DollBuff 俱Θ硄ノ BuffAura

public class BuffAura : MonoBehaviour
{
    public BuffBase[] buffs;
    public FACTION_GROUP group = FACTION_GROUP.ENEMY;

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
        //print("OnTriggerEnter ................" + objListInArea.Count);

        AddAura(other.gameObject);

        objListInArea.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        //print("OnTriggerExit ................" + objListInArea.Count);


        RemoveAura(other.gameObject);

        objListInArea.Remove(other.gameObject);
        //print("EnmeyOut Total = " + objListInArea.Count);
    }

    protected void AddAura(GameObject target)
    {
        BuffReceiver br = target.GetComponent<BuffReceiver>();

        if (br)
        {
            foreach (BuffBase buff in buffs)
            {
                br.AddBuff(buff);
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


    private void OnEnable()
    {
        //print("OnEnable ................" + objListInArea.Count);
        RestartAllAura();
    }

    private void OnDisable()
    {
        //print("OnDisable ................" + objListInArea.Count);
        ClearAllAura();
        objListInArea.Clear(); //场睲埃 Enable 穦 OnTriggerEnter
    }

    private void OnDestroy()
    {
        //print("OnDestroy ................");
        ClearAllAura();
    }

    //==================== TODO: 把计て ===================
    protected virtual bool CheckGameObject(GameObject obj)
    {
        if (group == FACTION_GROUP.ENEMY)
            return obj.CompareTag("Enemy");
        else if (group == FACTION_GROUP.PLAYER)
            return obj.CompareTag("Doll") || obj.CompareTag("Player");

        return false;
    }

    protected void UpdateList()
    {
        objListInArea.RemoveAll(item => item == null);
    }

}
