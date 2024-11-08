using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Hex : DollManager
{
    protected int N = 4;    //層數
    const int MaxSlot = 60; //根據層數 (1+2+3+4) * 6

    protected List<Node>[] nodeLayers;

    protected List<Node> allNodes = new List<Node>();

    public class Node           //為了給 UI 操作需要 public
    {
        public int slotIndex;
        public Transform slot;
        public Doll doll;
        public float x;
        public float y;
    }

    protected override void Start()
    {
        slotNum = MaxSlot;
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

        BattleSystem.GetHUD().RegisterDollLayoutUI(this);
    }

    protected void BuildHex()
    {
        const float StepX = 1.0f;
        //const float StepY = 1.0f; //間隔的 Y 寬，可以考慮縮短
        const float StepY = 0.875f;

        nodeLayers = new List<Node>[N];
        for (int i = 0; i < N; i++)
            nodeLayers[i] = new List<Node>();

        int currIndex = 0;
        float y = N;
        for (int i = -N; i <= N; i++)
        {
            int iAbs = Mathf.Abs(i);
            int lNum = N * 2 + 1 - iAbs;    //每一行的 Node 數
            float x = (lNum - 1) * -0.5f;
            for (int j = 0; j < lNum; j++)
            {
                if (i != 0 || j != lNum / 2)            //正中間位留給 Player
                {
                    Node node = new Node();
                    node.slotIndex = currIndex;
                    node.slot = DollSlots[currIndex];
                    node.doll = null;
                    node.slot.localPosition = new Vector3(x * StepX, 0, y * StepY);
                    node.x = x;
                    node.y = y;

                    int edgeDis = Mathf.Min(Mathf.Abs(j), lNum - Mathf.Abs(j) - 1);
                    int L = Mathf.Max(N - edgeDis, iAbs);

                    // ------ Debug 用
                    //GameObject o = new GameObject();
                    //o.transform.position = node.slot.transform.position;
                    //o.transform.rotation = Quaternion.Euler(90, 0, 0);
                    //o.transform.parent = node.slot;
                    //TextMesh tm = o.AddComponent<TextMesh>();
                    //tm.characterSize = 0.1f;
                    //tm.fontSize = 30;
                    //tm.color = Color.white;
                    //tm.text = L.ToString();
                    // ------

                    nodeLayers[L - 1].Add(node);
                    allNodes.Add(node);

                    currIndex++;
                }
                x += 1.0f;
            }
            y -= 1.0f;
        }

        print("總共 Build 出了 Node 數: " + currIndex);
    }

    public override bool AddOneDoll(Doll doll)
    {
        //return base.AddOneDoll(doll);

        for (int n = 0; n < N; n++)
        {
            for (int i = 0; i < nodeLayers[n].Count; i++)
            {
                if (nodeLayers[n][i].doll == null)
                {
                    nodeLayers[n][i].doll = doll;
                    dolls[nodeLayers[n][i].slotIndex] = doll;
                    doll.SetSlot(nodeLayers[n][i].slot);
                    return true;
                }
            }
        }

        return false;
    }

    public override void GetDollGroupAndIndex(Doll doll, ref int group, ref int index) 
    { 
        group = -1; index = -1;
        foreach (Node node in allNodes)
        {
            if (node.doll == doll)
            {
                group = 0;
                index = node.slotIndex;
            }
        }
    }

    public override void OnDollDestroy(Doll doll)
    {
        //print("DM_Hex OnDollDestroy: " + doll.name);
        base.OnDollDestroy(doll);
    }

    // ==================================== 以下為 UI 編輯使用的介面

    public List<Node> GetValidNodes()
    {
        return allNodes;
    }

    public bool ChangeDollPosition(Doll doll, int fromIndex, int toIndex)
    {
        Node nFrom = allNodes[fromIndex];
        Node nTo = allNodes[toIndex];
        if (doll != nFrom.doll || nTo.doll != null)
        {
            One.ERROR("DM_Hex 移動錯誤 !!");
            return false;
        }

        nTo.doll = doll;
        nFrom.doll = null;
        doll.SetSlot(nTo.slot);
        dolls[toIndex] = doll;
        dolls[fromIndex] = null;

        //TODO: SaveAllToPlayerData

        return true;
    }
}
