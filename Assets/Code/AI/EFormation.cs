using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EFormation : MonoBehaviour
{
    public GameObject leaderRef;
    public GameObject frontEnemyRef;
    public GameObject middleEnemyRef;
    public GameObject backEnemyRef;

    public float wakUpDistance = 20.0f;

    protected float toRotateMin = 8.0f;
    protected float toRotateMax = 12.0f;

    //陣型樣貌用參數
    public int frontCount = 4;
    public int middleCount = 4;
    public int backCount = 3;
    protected int FrontWidth = 4;
    protected int MiddleDepth = 3;
    protected int BackWidth = 4;
    protected float allShift = 0.0f;

    //隨機 Aura 效果
    public GameObject[] randomAttachRefs;

    protected enum PHASE
    {
        NONE,
        SLEEP,
        BATTLE,
        FINISH,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;

    protected GameObject myMaster;
    protected List<GameObject> frontList = new List<GameObject>();
    protected List<GameObject> middleList = new List<GameObject>();
    protected List<GameObject> backList = new List<GameObject>();

    protected float toRotateTime = 0;
    protected float wakeUpCheckTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextPhase = PHASE.SLEEP;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextPhase != currPhase)
        {
            currPhase = nextPhase;
        }

        switch (currPhase)
        {
            case PHASE.SLEEP:
                UpdateSleep();
                break;
            case PHASE.BATTLE:
                UpdateBattle();
                break;
            case PHASE.FINISH:
                Destroy(gameObject);
                break;
        }

    }

    protected void UpdateBattle()
    {
        if (myMaster)
        {
            transform.position = myMaster.transform.position;
        }
        else
        {
            nextPhase = PHASE.FINISH;
            return;
        }

        toRotateTime -= Time.deltaTime;
        if (toRotateTime <= 0)
        {
            SetupDirection();
            toRotateTime = Random.Range(toRotateMin, toRotateMax);
        }
    }

    protected void UpdateSleep()
    {
        wakeUpCheckTime -= Time.deltaTime;
        if (wakeUpCheckTime <= 0)
        {
            wakeUpCheckTime = 0.2f;

            if (GetPlayerDistance() <= wakUpDistance)
            {
                WakeUp();
                nextPhase = PHASE.BATTLE;
            }
        }
    }


    protected void WakeUp()
    {
        GameObject lo = BattleSystem.SpawnGameObj(leaderRef, transform.position);
        myMaster = lo;
        BuildFrontSlots();
        BuildMiddleSlots();
        BuildBackSlots();

        SetupDirection();

        if (frontEnemyRef && frontCount > 0)
        {
            for (int i = 0; i < frontCount; i++)
            {
                GameObject slot = frontList[i];
                GameObject eo = BattleSystem.SpawnGameObj(frontEnemyRef, slot.transform.position);
                Enemy e = eo.GetComponent<Enemy>();
                e.SetSlot(slot.transform);
            }
        }
        if (middleEnemyRef && middleCount > 0)
        {
            for (int i = 0; i < middleCount; i++)
            {
                GameObject slot = middleList[i];
                GameObject eo = BattleSystem.SpawnGameObj(middleEnemyRef, slot.transform.position);
                Enemy e = eo.GetComponent<Enemy>();
                e.SetSlot(slot.transform);
            }
        }
        if (backEnemyRef && middleCount > 0)
        {
            for (int i = 0; i < backCount; i++)
            {
                GameObject slot = backList[i];
                GameObject eo = BattleSystem.SpawnGameObj(backEnemyRef, slot.transform.position);
                Enemy e = eo.GetComponent<Enemy>();
                e.SetSlot(slot.transform);
            }
        }

        //產生隨機 Aura
        if (randomAttachRefs!=null && randomAttachRefs.Length > 0)
        {
            GameObject o = BattleSystem.SpawnGameObj(randomAttachRefs[Random.Range(0, randomAttachRefs.Length)], transform.position);
            o.transform.parent = transform;
        }
    }


    protected float GetPlayerDistance()
    {
        if (BattleSystem.GetPC() == null)
            return Mathf.Infinity;
        Vector3 playerPos = BattleSystem.GetPC().transform.position;
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, playerPos, NavMesh.AllAreas, path))
        {
            float pathLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return pathLength;
        }
        return Mathf.Infinity;
    }



    protected void SetupDirection()
    {
        PlayerControllerBase pc = BattleSystem.GetPC();
        if (pc)
        {
            float angle = 0;
            Vector3 pcDir = pc.transform.position - transform.position;
            if (pcDir.z > pcDir.x)
            {
                if (pcDir.z > -pcDir.x)
                {
                    angle = 0;
                }
                else
                {
                    angle = 270;
                }
            }
            else
            {
                if (pcDir.z > -pcDir.x)
                {
                    angle = 90;
                }
                else
                {
                    angle = 180;
                }
            }
            transform.rotation = Quaternion.Euler(0, angle, 0);
            //print("SetupDirection!! " + angle);
        }
    }

    protected void BuildFrontSlots()
    {
        int frontNum = frontCount;

        if (frontNum <= 0)
            return;

        int nLine = ((frontNum - 1) / FrontWidth) + 1;
        int lastLineCount = (frontNum - 1) % FrontWidth + 1;

        float fPos = Mathf.Max(1.0f, 2.0f - (float)(nLine - 1) * 0.5f) + allShift;  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine - 1);

        for (int l = 0; l < nLine; l++)
        {
            int num = FrontWidth;
            if (l == nLine - 1)
                num = lastLineCount;
            //print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i = l * FrontWidth; i < l * FrontWidth + num; i++)
            {
                //print("Prepare ..." + i);
                //frontList[i].GetSlot().localPosition = new Vector3(lPos, 0, fPos);
                Vector3 pos = new Vector3(lPos, 0, fPos);

                GameObject sO = new GameObject("ESlot_Front_" + i);
                sO.transform.position = gameObject.transform.position + pos;
                sO.transform.parent = gameObject.transform;
                frontList.Add(sO);

                lPos += slotWidth;
            }
            fPos -= slotDepth;
        }
    }


    protected void BuildMiddleSlots()
    {
        int middleNum = middleCount;

        if (middleNum <= 0)
            return;

        int circleNum = MiddleDepth + MiddleDepth;
        int nCircle = (middleNum - 1) / circleNum + 1;
        int lastCircleCount = (middleNum - 1) % circleNum + 1;

        float slotWidth = 1.0f;
        float innerWidth = 1.5f;    //最內圈距離

        float width = innerWidth;
        for (int c = 0; c < nCircle; c++)
        {
            int num = circleNum;
            if (c == nCircle - 1)
                num = lastCircleCount;
            int nLine = (num - 1) / 2 + 1;
            float slotDepth = Mathf.Max(1.0f, 1.5f - (nLine - 1) * 0.25f);
            float totalDepth = (float)(nLine - 1) * slotDepth;
            float fPos = totalDepth * 0.5f + allShift;

            for (int l = 0; l < nLine; l++)
            {
                int i = c * circleNum + l * 2;
                //middleList[i].GetSlot().localPosition = new Vector3(-width, 0, fPos);  //左

                Vector3 pos = new Vector3(-width, 0, fPos);  //左
                GameObject sO = new GameObject("ESlot_Middle_" + i);
                sO.transform.position = gameObject.transform.position + pos;
                sO.transform.parent = gameObject.transform;
                middleList.Add(sO);

                i++;
                if (i < middleNum)
                {
                    //middleList[i].GetSlot().localPosition = new Vector3(width, 0, fPos);   //右
                    pos = new Vector3(width, 0, fPos);   //右
                    sO = new GameObject("ESlot_Middle_" + i);
                    sO.transform.position = gameObject.transform.position + pos;
                    sO.transform.parent = gameObject.transform;
                    middleList.Add(sO);
                }

                fPos -= slotDepth;
            }
            width += slotWidth;
        }

    }

    protected void BuildBackSlots()
    {
        int backNum = backCount;

        if (backNum <= 0)
            return;

        int nLine = ((backNum - 1) / BackWidth) + 1;
        int lastLineCount = (backNum - 1) % BackWidth + 1;

        float bkPos = Mathf.Max(1.0f, 2.0f - (float)(nLine - 1) * 0.5f) - allShift;  //後方起始
        float slotDepth = 1.0f;
        bkPos += slotDepth * (float)(nLine - 1);

        for (int l = 0; l < nLine; l++)
        {
            int num = BackWidth;
            if (l == nLine - 1)
                num = lastLineCount;

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i = l * BackWidth; i < l * BackWidth + num; i++)
            {
                //backList[backNum - i - 1].GetSlot().localPosition = new Vector3(lPos, 0, -bkPos);
                Vector3 pos = new Vector3(lPos, 0, -bkPos);
                GameObject sO = new GameObject("ESlot_Back_" + i);
                sO.transform.position = gameObject.transform.position + pos;
                sO.transform.parent = gameObject.transform;
                backList.Add(sO);

                lPos += slotWidth;
            }
            bkPos -= slotDepth;
        }
    }

}
