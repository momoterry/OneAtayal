using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Hex : DollManager
{
    protected int N = 4;    //�̤j�h��
    const int MaxSlot = 60; //�ھڼh�� (1+2+3+4) * 6
    protected int[] LayerLimit = new int[] { 5, 15, 30, 61 };

    protected int currN = 1;            //�ثe���\�ϥΪ��h��
    protected int currDollNum = 0;      //�ثe Doll ��

    protected List<Node>[] nodeLayers;

    protected List<Node> allNodes = new List<Node>();

    public class Node           //���F�� UI �ާ@�ݭn public
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
        //const float StepY = 1.0f; //���j�� Y �e�A�i�H�Ҽ{�Y�u
        const float StepY = 0.875f;

        nodeLayers = new List<Node>[N];
        for (int i = 0; i < N; i++)
            nodeLayers[i] = new List<Node>();

        int currIndex = 0;
        float y = N;
        for (int i = -N; i <= N; i++)
        {
            int iAbs = Mathf.Abs(i);
            int lNum = N * 2 + 1 - iAbs;    //�C�@�檺 Node ��
            float x = (lNum - 1) * -0.5f;
            for (int j = 0; j < lNum; j++)
            {
                if (i != 0 || j != lNum / 2)            //��������d�� Player
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

                    // ------ Debug ��
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

        print("�`�@ Build �X�F Node ��: " + currIndex);
    }

    public override bool AddOneDoll(Doll doll)
    {
        //return base.AddOneDoll(doll);
        if (currN < N && currDollNum >= LayerLimit[currN-1])
        {
            currN++;
            print("�h���X�j��: " + currN);
        }

        for (int n = 0; n < currN; n++)
        {
            for (int i = 0; i < nodeLayers[n].Count; i++)
            {
                if (nodeLayers[n][i].doll == null)
                {
                    nodeLayers[n][i].doll = doll;
                    dolls[nodeLayers[n][i].slotIndex] = doll;
                    doll.SetSlot(nodeLayers[n][i].slot);
                    currDollNum++;
                    return true;
                }
            }
        }

        return false;
    }

    public override bool AddOneDollWithGivenPosition(Doll doll, int group, int index)
    {
        if (group == 0 && index>=0 && index < MaxSlot)
        {
            if (allNodes[index].doll == null)
            {
                //print("DM_Hex �N Doll �[����w��m: " + index);
                allNodes[index].doll = doll;
                dolls[index] = doll;
                doll.SetSlot(allNodes[index].slot);
                currDollNum++;
                if (currN < N && currDollNum > LayerLimit[currN-1])
                {
                    currN++;
                    print("AddOneDollWithGivenPosition �h���X�j��: " + currN);
                }
                return true;
            }
        }
        //return AddOneDoll(doll);
        return AddOneDoll(doll);
    }

    public override void OnDollTempDeath(Doll doll)
    {
    }

    public override void OnDollRevive(Doll doll)
    {
    }

    public override void OnDollDestroy(Doll doll)
    {
        currDollNum--;
        while (currN > 1 && currDollNum <= LayerLimit[currN - 2])
        {
            currN--;
            print("�h���Y���: " + currN);
        }
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

    // ==================================== �H�U�� UI �s��ϥΪ�����

    public List<Node> GetValidNodes()
    {
        List<Node> result = new();
        for (int i=0; i<currN; i++)
        {
            result.AddRange(nodeLayers[i]);
        }
        return result;
    }

    public bool ChangeDollPosition(Doll doll, int fromIndex, int toIndex)
    {
        Node nFrom = allNodes[fromIndex];
        Node nTo = allNodes[toIndex];
        if (doll != nFrom.doll || nTo.doll != null)
        {
            One.ERROR("DM_Hex ���ʿ��~ !!");
            return false;
        }

        nTo.doll = doll;
        nFrom.doll = null;
        doll.SetSlot(nTo.slot);
        dolls[toIndex] = doll;
        dolls[fromIndex] = null;

        SaveAllToPlayerData();

        return true;
    }


    protected void SaveAllToPlayerData()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        ContinuousBattleManager.ResetBattleSavedDolls();

        pData.RemoveAllUsingDolls();

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].doll != null)
            {
                if (allNodes[i].doll.joinSaveType == DOLL_JOIN_SAVE_TYPE.FOREVER)
                {
                    pData.AddUsingDoll(allNodes[i].doll.ID, 0, i);
                    //print("PlayerData �s�J Doll: " + allNodes[i].doll.ID + " slot: " + allNodes[i].slotIndex + " i: " + i);
                }
                else if (allNodes[i].doll.joinSaveType == DOLL_JOIN_SAVE_TYPE.BATTLE)
                {
                    ContinuousBattleManager.AddCollectedDoll(allNodes[i].doll.ID, 0, i);
                }
            }
        }
    }
}
