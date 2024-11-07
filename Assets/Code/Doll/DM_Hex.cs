using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Hex : DollManager
{
    protected int N = 4;    //�h��
    const int MaxSlot = 70; //�ھڼh��

    protected class Node
    {
        public int slotIndex;
        public Transform slot;
        public Doll doll;
    }

    protected override void Start()
    {
        slotNum = MaxSlot;   //TEST
        DollSlots = new Transform[slotNum];
        for (int i = 0; i < slotNum; i++)
        {
            GameObject o = new GameObject("HexSlot_" + i);
            o.transform.position = transform.position;
            o.transform.parent = transform;
            DollSlots[i] = o.transform;
        }
        dolls = new Doll[slotNum];

        BuildHex();

        //BattleSystem.GetHUD().RegisterDollLayoutUI(this);
    }

    protected void BuildHex()
    {
        float StepX = 1.0f;
        float StepY = 0.875f; //���j�� Y �e�A�i�H�Ҽ{�Y�u

        int currIndex = 0;
        float y = StepY * N;
        for (int i = -N; i<=N; i++)
        {
            int lNum = N * 2 + 1 - Mathf.Abs(i);    //�C�@�檺 Node ��
            float x = StepX * lNum * -0.5f;
            for (int j = 0; j<=lNum; j++)
            {
                Node node = new Node();
                node.slotIndex = currIndex;
                node.slot = DollSlots[currIndex];
                node.doll = null;
                node.slot.localPosition = new Vector3(x, 0, y);

                currIndex++;
                x += StepX;
            }
            y -= StepY;
        }

        print("�`�@ Build �X�F Node ��: " + currIndex);
    }

}
