using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreasureBox : MonoBehaviour
{
    [System.Serializable]
    public class RewardInfo
    {
        public GameObject objRef;
        public float numMin;
        public float numMax;
    }
    public RewardInfo[] rewards;
    public GameObject specialRewardTemplateRef;
    public float timeToSpawn = 0.5f;
    public float timeToFly = 0.25f;
    public Vector2 spawnAreaMax = new Vector2(6, 6);
    public Vector2 spawnAreaIn = new Vector2(2, 2);

<<<<<<< HEAD
    protected List<string> specialRewardItemIDs = new List<string>();

=======

    //protected List<string> special
>>>>>>> fb7e93f71d491560878903b9f1c2d60e4dd45d9f
    protected float waitTime;

    protected enum Phase
    {
        NONE,
        WAIT,
        TO_SPAWN,
        FLYING,
        DONE,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.NONE;

    public void AddSpecialRewardItem(string itemID)
    {
        if (ItemDef.GetInstance().GetItemInfo(itemID) != null)
            specialRewardItemIDs.Add(itemID);
        else
            print("ERROR!!!! AddSpecialRewardItem invalid item ID: " + itemID);
    }

    void Start()
    {
        nextPhase = Phase.WAIT;

        //TEST
        AddSpecialRewardItem("Mat_BookBlue");
        AddSpecialRewardItem("Mat_BookBlue");
    }

    // Update is called once per frame
    void Update()
    {
        currPhase = nextPhase;
        if (currPhase == Phase.TO_SPAWN)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                DoSpawnReward();
                nextPhase = Phase.FLYING;
                waitTime = timeToFly;
            }
        }
        else if (currPhase == Phase.FLYING)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <=0)
            {
                //print("Done......");
                UpdateFlying(1.0f);
                nextPhase = Phase.DONE;
            }
            else
                UpdateFlying(1.0f - waitTime / timeToFly);
        }
    }

    protected class FlyingObjInfo
    {
        public GameObject obj;
        public Vector3 targetPos;
        public HeightController hc;
    }
    protected List<FlyingObjInfo> flyList = new List<FlyingObjInfo>();

    protected void UpdateFlying(float ratio)
    {
        float jumpHeight = 2.0f;
        float jumpLow = 0.5f;
        float ratioGap = 0.7f;  //超過這個比例以後往下掉
        foreach (FlyingObjInfo fly in flyList)
        {
            if (fly.obj)
            {
                Vector3 pos;// = (fly.targetPos - transform.position) * ratio + transform.position;
                Vector3 posUp;
                if (ratio <= ratioGap)
                {
                    pos = (fly.targetPos - transform.position) * ratio / ratioGap + transform.position;
                    posUp = Vector3.forward * Mathf.Sin(ratio / ratioGap * Mathf.PI) * jumpHeight;
                }
                else
                {
                    pos = fly.targetPos;
                    posUp = Vector3.forward * Mathf.Sin((ratio - ratioGap) / (1.0f - ratioGap) * Mathf.PI) * jumpLow;
                }
                if (fly.hc != null)
                {
                    fly.obj.transform.position = pos;
                    fly.hc.SetHeight(posUp.z);
                }
                else
                    fly.obj.transform.position = pos + posUp;
            }
            //else
            //{
            //    print("Wooops....");
            //}
        }
    }


    protected void DoSpawnReward()
    {
        //print("DoSpawnReward !!!!");
        int totalSpawn = 0;
        int[] spawnNum = new int[rewards.Length];
        for (int i = 0; i < rewards.Length; i++)
        {
            spawnNum[i] = OneUtility.FloatToRandomInt(Random.Range(rewards[i].numMin, rewards[i].numMax));
            totalSpawn += spawnNum[i];
        }
        totalSpawn += specialRewardItemIDs.Count;

        List<Vector3> allPos = new List<Vector3>();
        float fStep = 1.0f;
        int hWidth = Mathf.RoundToInt(spawnAreaMax.x * 0.5f / fStep);
        int hHeight = Mathf.RoundToInt(spawnAreaMax.y * 0.5f / fStep);
        float hWidthMin = spawnAreaIn.x * 0.5f;
        float hHeightMin = spawnAreaIn.y * 0.5f;
        for (int x = -hWidth; x <= hWidth; x++)
        {
            for (int y = -hHeight; y <= hHeight; y++)
            {
                float posX = x * fStep;
                float posY = y * fStep;
                if (posX < -hWidthMin || posX > hWidthMin || posY < -hHeightMin || posY > hHeightMin)
                {
                    Vector3 pos = transform.position + new Vector3(posX, 0, posY);
                    allPos.Add(pos);
                }
            }
        }

        List<Vector3> choosePos = new List<Vector3>();
        int[] chooseIndex = OneUtility.GetRandomNonRepeatNumbers(0, allPos.Count, totalSpawn);
        if (chooseIndex == null)
        {
            print("ERROR!!!! 需要產生的 Reward 超過格子數啦!!! ");
            return;
        }
        for (int i = 0; i < totalSpawn; i++)
            choosePos.Add(allPos[chooseIndex[i]]);


        int n = 0;
        for (int i = 0; i < rewards.Length; i++)
        {
            for (int k=0; k< spawnNum[i]; k++)
            {
                GameObject o = Instantiate(rewards[i].objRef);
                o.transform.position = transform.position;
                FlyingObjInfo fly = new FlyingObjInfo();
                fly.obj = o;
                fly.targetPos = choosePos[n];
                fly.hc = o.GetComponent<HeightController>();
                flyList.Add(fly);
                n++;
            }
        }

        //Special Rewards
        for (int i=0; i<specialRewardItemIDs.Count; i++)
        {
            GameObject o = Instantiate(specialRewardTemplateRef);
            o.transform.position = transform.position;

            ItemInfo item = ItemDef.GetInstance().GetItemInfo(specialRewardItemIDs[i]);
            ItemPickup ip = o.GetComponent<ItemPickup>();
            if (ip)
            {
                ip.ItemID = item.ID;
            }

            FlyingObjInfo fly = new FlyingObjInfo();
            fly.obj = o;
            fly.targetPos = choosePos[n];
            fly.hc = o.GetComponent<HeightController>();
            SpriteRenderer sr = fly.hc.mainBody.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                sr.sprite = item.Icon;
            }
            flyList.Add(fly);
            n++;
        }
    }

    public void OnTG(GameObject whoTG)
    {
        if (currPhase == Phase.WAIT)
        {
            waitTime = timeToSpawn;
            nextPhase = Phase.TO_SPAWN;
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }

}
