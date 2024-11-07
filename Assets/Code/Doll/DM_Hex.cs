using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Hex : DollManager
{
    protected int N = 4;    //糷计
    const int MaxSlot = 60; //沮糷计 (1+2+3+4) * 6

    protected List<Node>[] nodeLayers;

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
        const float StepX = 1.0f;
        const float StepY = 0.875f; //丁筳 Y 糴σ納罽祏

        nodeLayers = new List<Node>[N];
        for (int i = 0; i < N; i++)
            nodeLayers[i] = new List<Node>();

        int currIndex = 0;
        float y = StepY * N;
        for (int i = -N; i<=N; i++)
        {
            int iAbs = Mathf.Abs(i);
            int lNum = N * 2 + 1 - iAbs;    //–︽ Node 计
            float x = StepX * (lNum-1) * -0.5f;
            for (int j = 0; j<lNum; j++)
            {
                if (i != 0 || j != lNum / 2)            //タい丁痙倒 Player
                {
                    Node node = new Node();
                    node.slotIndex = currIndex;
                    node.slot = DollSlots[currIndex];
                    node.doll = null;
                    node.slot.localPosition = new Vector3(x, 0, y);

                    int edgeDis = Mathf.Min(Mathf.Abs(j), lNum - Mathf.Abs(j) - 1);
                    int L = Mathf.Max(N - edgeDis, iAbs);

                    //Debug ノ
                    //GameObject o = new GameObject();
                    //o.transform.position = node.slot.transform.position;
                    //o.transform.rotation = Quaternion.Euler(90, 0, 0);
                    //o.transform.parent = node.slot;
                    //TextMesh tm = o.AddComponent<TextMesh>();
                    //tm.characterSize = 0.1f;
                    //tm.fontSize = 30;
                    //tm.color = Color.white;
                    //tm.text = L.ToString();

                    nodeLayers[L-1].Add(node);

                    currIndex++;
                }
                x += StepX;
            }
            y -= StepY;
        }

        print("羆 Build  Node 计: " + currIndex);
    }

    public override bool AddOneDoll(Doll doll)
    {
        //return base.AddOneDoll(doll);

        for (int n=0; n<N; n++)
        {
            for (int i=0; i < nodeLayers[n].Count; i++)
            {
                if (nodeLayers[n][i].doll == null)
                {
                    dolls[i] = doll;
                    nodeLayers[n][i].doll = doll;
                    doll.SetSlot(nodeLayers[n][i].slot);
                    return true;
                }
            }
        }

        return false;
    }

}
