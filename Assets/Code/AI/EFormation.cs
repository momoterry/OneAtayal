using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFormation : MonoBehaviour
{
    public GameObject myMaster;

    public GameObject frontEnemyRef;
    public GameObject middleEnemyRef;
    public GameObject backEnemyRef;

    protected int frontCount = 4;
    protected int middleCount = 4;
    protected int backCount = 2;

    protected int FrontWidth = 4;
    protected int MiddleDepth = 3;
    protected int BackWidth = 4;

    protected float allShift = 0.0f;

    protected List<GameObject> frontList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        RebuildFrontSlots();
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
    }

    // Update is called once per frame
    void Update()
    {
        if (myMaster)
        {
            transform.position = myMaster.transform.position;
        }
    }

    protected void RebuildFrontSlots()
    {
        //int FrontWidth = 4; //TODO: ���ܼ�
        int frontNum = frontCount;

        if (frontNum <= 0)
            return;

        int nLine = ((frontNum - 1) / FrontWidth) + 1;
        int lastLineCount = (frontNum - 1) % FrontWidth + 1;

        float fPos = Mathf.Max(1.0f, 2.0f - (float)(nLine - 1) * 0.5f) + allShift;  //�e��_�l
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

                GameObject sO = new GameObject("ESlot_" + i);
                sO.transform.position = gameObject.transform.position + pos;
                sO.transform.parent = gameObject.transform;
                frontList.Add(sO);

                lPos += slotWidth;
            }
            fPos -= slotDepth;
        }
    }

}