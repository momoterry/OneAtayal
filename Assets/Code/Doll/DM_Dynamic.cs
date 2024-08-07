using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Dynamic : DollManager
{
    const int MaxSlot = 60;

    public int FrontWidth = 4;
    public int MiddleDepth = 3;
    public int BackWidth = 4;

    public enum GROUP_TYPE
    {
        FRONT, LEFT, RIGHT, BACK
    }

    protected float allShift = 0.0f;

    protected List<Doll> frontList = new List<Doll>();
    //protected List<Doll> middleList = new List<Doll>();
    protected List<Doll> backList = new List<Doll>();

    protected List<Doll> leftList = new List<Doll>();
    protected List<Doll> rightList = new List<Doll>();

    //protected bool needRebuild = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        slotNum = MaxSlot;
        DollSlots = new Transform[slotNum];
        for (int i = 0; i < slotNum; i++)
        {
            GameObject o = new GameObject("DynaSlot_" + i);
            o.transform.position = transform.position;
            o.transform.parent = transform;
            DollSlots[i] = o.transform;
        }
        dolls = new Doll[slotNum];

        BattleSystem.GetHUD().RegisterDollLayoutUI(this);
    }


    protected void BuildFrontSlots()
    {
        int frontNum = frontList.Count;

        if (frontNum <= 0)
            return;

        int nLine = ((frontNum-1) / FrontWidth ) + 1;
        int lastLineCount = (frontNum-1) % FrontWidth + 1;

        float fPos = Mathf.Max(1.0f, 2.0f - (float)(nLine-1) * 0.5f) + allShift;  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine-1);

        for (int l=0; l<nLine; l++)
        {
            int num = FrontWidth;
            if (l == nLine - 1)
                num = lastLineCount;
            //print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i= l * FrontWidth; i< l * FrontWidth + num; i++)
            {
                //print("Prepare ..." + i);
                frontList[i].GetSlot().localPosition = new Vector3(lPos, 0, fPos);
                lPos += slotWidth;
            }
            fPos -= slotDepth;
        }
    }

    //protected void RebuildMiddleSlots()
    //{
    //    int middleNum = middleList.Count;

    //    if (middleNum <= 0)
    //        return;

    //    int circleNum = MiddleDepth + MiddleDepth;
    //    int nCircle = (middleNum - 1) / circleNum + 1;
    //    int lastCircleCount = (middleNum - 1) % circleNum + 1;

    //    float slotWidth = 1.0f;
    //    float innerWidth = 1.0f;    //最內圈距離

    //    float width = innerWidth;
    //    for (int c=0; c<nCircle; c++)
    //    {
    //        int num = circleNum;
    //        if (c == nCircle - 1)
    //            num = lastCircleCount;
    //        int nLine = (num - 1) / 2 + 1;
    //        float slotDepth = Mathf.Max(1.0f, 1.5f - (nLine - 1) * 0.25f);
    //        float totalDepth = (float)(nLine - 1) * slotDepth;
    //        float fPos = totalDepth * 0.5f + allShift;

    //        for (int l=0; l<nLine; l++)
    //        {
    //            int i = c * circleNum + l * 2;
    //            middleList[i].GetSlot().localPosition = new Vector3(-width, 0, fPos);  //左
    //            //print("Prepare... " + i);

    //            i++;
    //            if (i < middleNum)
    //            {
    //                middleList[i].GetSlot().localPosition = new Vector3(width, 0, fPos);   //右
    //                //print("Prepare... " + i);
    //            }

    //            fPos -= slotDepth;
    //        }
    //        width += slotWidth;
    //    }

    //}

    protected void BuildLRSlots( bool isLeft)
    {
        List<Doll> currList = isLeft ? leftList : rightList;
        int totalNum = currList.Count;

        if (totalNum <= 0)
            return;

        //int circleNum = MiddleDepth + MiddleDepth;
        int nCols = ((totalNum - 1) / MiddleDepth) + 1;
        int lastColCount = (totalNum - 1) % MiddleDepth + 1;

        float slotWidth = 1.0f;
        float innerWidth = 1.0f;    //最內圈距離

        float width = innerWidth;
        int i = 0;
        for (int c = 0; c < nCols; c++)
        {
            int nLine = MiddleDepth;
            if (c == nCols - 1)
                nLine = lastColCount;
            //int nLine = (num - 1) / 2 + 1;
            float slotDepth = Mathf.Max(1.0f, 1.5f - (nLine - 1) * 0.25f);
            float totalDepth = (float)(nLine - 1) * slotDepth;
            float fPos = totalDepth * 0.5f + allShift;

            for (int l = 0; l < nLine; l++)
            {
                //int i = c * MiddleDepth + l;
                currList[i].GetSlot().localPosition = new Vector3(isLeft ? -width:width, 0, fPos);
                //print("Prepare... " + i);

                //i++;
                //if (i < totalNum)
                //{
                //    middleList[i].GetSlot().localPosition = new Vector3(width, 0, fPos);   //右
                //    //print("Prepare... " + i);
                //}

                i++;
                fPos -= slotDepth;
            }
            width += slotWidth;
        }
    }

    protected void BuildBackSlots()
    {
        int backNum = backList.Count;

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
            //print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i = l * BackWidth; i < l * BackWidth + num; i++)
            {
                //print("Prepare ..." + (backNum - i));
                backList[backNum-i-1].GetSlot().localPosition = new Vector3(lPos, 0, -bkPos);
                lPos += slotWidth;
            }
            bkPos -= slotDepth;
        }
    }

    //protected void RebuildFormation()
    //{
    //    frontList.Clear();
    //    middleList.Clear();
    //    leftList.Clear();
    //    rightList.Clear();
    //    backList.Clear();

    //    for (int i = 0; i < slotNum; i++)
    //    {
    //        if (dolls[i] && dolls[i].gameObject.activeInHierarchy)
    //        {
    //            switch(dolls[i].positionType)
    //            {
    //                case DOLL_POSITION_TYPE.FRONT:
    //                    frontList.Add(dolls[i]);
    //                    break;
    //                case DOLL_POSITION_TYPE.MIDDLE:
    //                    //middleList.Add(dolls[i]);
    //                    if (leftList.Count > rightList.Count)
    //                        rightList.Add(dolls[i]);
    //                    else
    //                        leftList.Add(dolls[i]);
    //                    break;
    //                case DOLL_POSITION_TYPE.BACK:
    //                    backList.Add(dolls[i]);
    //                    break;
    //            }
    //        }
    //    }
    //    if (frontList.Count > 0)
    //    {
    //        BuildFrontSlots();
    //    }
    //    if (middleList.Count > 0)
    //    {
    //        RebuildMiddleSlots();
    //    }
    //    if (leftList.Count > 0)
    //    {
    //        BuildLRSlots(true);
    //    }
    //    if (rightList.Count > 0)
    //    {
    //        BuildLRSlots(false);
    //    }
    //    if (backList.Count > 0)
    //    {
    //        BuildBackSlots();
    //    }
    //}

    public override void GetDollGroupAndIndex(Doll doll, ref int group, ref int index)
    {
        index = frontList.IndexOf(doll);
        if (index >= 0)
        {
            group = (int)GROUP_TYPE.FRONT;
            return;
        }
        index = leftList.IndexOf(doll);
        if (index >= 0)
        {
            group = (int)GROUP_TYPE.LEFT;
            return;
        }
        index = rightList.IndexOf(doll);
        if (index >= 0)
        {
            group = (int)GROUP_TYPE.RIGHT;
            return;
        }
        index = backList.IndexOf(doll);
        if (index >= 0)
        {
            group = (int)GROUP_TYPE.BACK;
            return;
        }
    }

    public override bool AddOneDollWithGivenPosition(Doll doll, int group, int index)
    {
        bool isOK = false;
        for (int i = 0; i < slotNum; i++)
        {
            if (dolls[i] == null && DollSlots[i] != null)
            {
                dolls[i] = doll;
                doll.SetSlot(DollSlots[i]);
                isOK = true;
                break;
            }
        }

        if (isOK)
        {
            //print("--To DoAddDollToList: " + group + " -- " + index);
            DoAddDollToList(doll, group, index);
        }
        return isOK;
    }

    public override bool AddOneDoll(Doll doll/*, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT*/)
    {
        //bool isOK = false;
        //for (int i=0; i<slotNum; i++)
        //{
        //    if (dolls[i] == null && DollSlots[i] != null)
        //    {
        //        dolls[i] = doll;
        //        doll.SetSlot(DollSlots[i]);
        //        isOK = true;
        //        break;
        //    }
        //}

        //if (isOK)
        //{
        //    DoAddDollToList(doll, -1, -1);
        //}

        //return isOK;
        return AddOneDollWithGivenPosition(doll, -1, -1);
    }

    public override void OnDollTempDeath(Doll doll)
    {
        DoRemoveDollFronList(doll);
    }

    public override void OnDollRevive(Doll doll)
    {
        //RebuildFormation();   //TODO: 用本來記錄的位置來回復?
        DoAddDollToList(doll, -1, -1);
    }

    public override void OnDollDestroy(Doll doll)
    {
        DoRemoveDollFronList(doll);
    }

    protected void DoAddDollToList(Doll doll, int group, int index)
    {
        if (group < 0)
        {
            switch (doll.positionType)
            {
                case DOLL_POSITION_TYPE.FRONT:
                    frontList.Add(doll);
                    BuildFrontSlots();
                    break;
                case DOLL_POSITION_TYPE.MIDDLE:
                    if (leftList.Count <= rightList.Count)
                    {
                        leftList.Add(doll);
                        BuildLRSlots(true);
                    }
                    else
                    {
                        rightList.Add(doll);
                        BuildLRSlots(false);
                    }
                    break;
                case DOLL_POSITION_TYPE.BACK:
                    backList.Add(doll);
                    BuildBackSlots();
                    break;
            }
        }
        else
        {
            switch (group)
            {
                case (int)GROUP_TYPE.FRONT:
                    if (index >=0 && index < frontList.Count)
                    {
                        frontList.Insert(index, doll);
                    }
                    else
                    {
                        frontList.Add(doll);
                    }
                    BuildFrontSlots();
                    break;
                case (int)GROUP_TYPE.LEFT:
                    if (index >= 0 && index < leftList.Count)
                    {
                        leftList.Insert(index, doll);
                    }
                    else
                    {
                        leftList.Add(doll);
                    }
                    BuildLRSlots(true);
                    break;
                case (int)GROUP_TYPE.RIGHT:
                    if (index >= 0 && index < rightList.Count)
                    {
                        rightList.Insert(index, doll);
                    }
                    else
                    {
                        rightList.Add(doll);
                    }
                    BuildLRSlots(false);
                    break;
                case (int)GROUP_TYPE.BACK:
                    if (index >= 0 && index < backList.Count)
                    {
                        backList.Insert(index, doll);
                    }
                    else
                    {
                        backList.Add(doll);
                    }
                    BuildBackSlots();
                    break;
            }
        }

    }

    protected void DoRemoveDollFronList(Doll doll)
    {
        //switch (doll.positionType)
        //{
        //    case DOLL_POSITION_TYPE.FRONT:
        //        frontList.Remove(doll);
        //        BuildFrontSlots();
        //        break;
        //    case DOLL_POSITION_TYPE.MIDDLE:
        //        //middleList.Remove(doll);
        //        //RebuildMiddleSlots();
        //        if (leftList.Remove(doll))
        //        {
        //            BuildLRSlots(true);
        //        }
        //        else if (rightList.Remove(doll))
        //        {
        //            BuildLRSlots(false);
        //        }
        //        break;
        //    case DOLL_POSITION_TYPE.BACK:
        //        backList.Remove(doll);
        //        BuildBackSlots();
        //        break;
        //}
        if (frontList.Contains(doll)) {
            frontList.Remove(doll);
            BuildFrontSlots();
        }
        else if (leftList.Contains(doll))
        {
            leftList.Remove(doll);
            BuildLRSlots(true);
        }
        else if (rightList.Contains(doll))
        {
            rightList.Remove(doll);
            BuildLRSlots(false);
        }
        else if (backList.Remove(doll))
        {
            backList.Remove(doll);
            BuildBackSlots();
        }
    }

    //以下為支援陣型編輯 TODO: 有些以後也會要整合到 Base 中
    //public void GetAllList( ref List<Doll> _frontList, ref List<Doll> _middleList, ref List<Doll> _backList)
    //{
    //    _frontList = frontList;
    //    _middleList = middleList;
    //    _backList = backList;
    //}

    //由於會有插入導致許多 Doll 的 Index 都改變的情況，直接一次性把 PlayerData 中的資料重新設定
    protected void SaveAllToPlayerData()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        ContinuousBattleManager.ResetBattleSavedDolls();

        pData.RemoveAllUsingDolls();

        for (int i=0; i<frontList.Count; i++)
        {
            if (frontList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.FOREVER)
            {
                pData.AddUsingDoll(frontList[i].ID, (int)GROUP_TYPE.FRONT, i);
            }
            else if (frontList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.BATTLE)
            {
                ContinuousBattleManager.AddCollectedDoll(frontList[i].ID, (int)GROUP_TYPE.FRONT, i);
            }
        }
        for (int i = 0; i < leftList.Count; i++)
        {
            if (leftList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.FOREVER)
            {
                pData.AddUsingDoll(leftList[i].ID, (int)GROUP_TYPE.LEFT, i);
            }
            else if (leftList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.BATTLE)
            {
                ContinuousBattleManager.AddCollectedDoll(leftList[i].ID, (int)GROUP_TYPE.LEFT, i);
            }
        }
        for (int i = 0; i < rightList.Count; i++)
        {
            if (rightList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.FOREVER)
            {
                pData.AddUsingDoll(rightList[i].ID, (int)GROUP_TYPE.RIGHT, i);
            }
            else if (rightList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.BATTLE)
            {
                ContinuousBattleManager.AddCollectedDoll(rightList[i].ID, (int)GROUP_TYPE.RIGHT, i);
            }
        }
        for (int i = 0; i < backList.Count; i++)
        {
            if (backList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.FOREVER)
            {
                pData.AddUsingDoll(backList[i].ID, (int)GROUP_TYPE.BACK, i);
            }
            else if (backList[i].joinSaveType == DOLL_JOIN_SAVE_TYPE.BATTLE)
            {
                ContinuousBattleManager.AddCollectedDoll(backList[i].ID, (int)GROUP_TYPE.BACK, i);
            }
        }
    }

    protected List<Doll> GetListByGroupID(int group)
    {
        switch (group)
        {
            case (int)GROUP_TYPE.FRONT:
                return frontList;
            case (int)GROUP_TYPE.LEFT:
                return leftList;
            case (int)GROUP_TYPE.RIGHT:
                return rightList;
            case (int)GROUP_TYPE.BACK:
                return backList;
        }
        return null;
    }

    public bool ChangeDollPosition(Doll doll, int fromGroup, int toGroup, int fromIndex, int toIndex)
    {
        List<Doll> fmList = GetListByGroupID(fromGroup);
        List<Doll> toList = GetListByGroupID(toGroup);

        if (fromGroup == toGroup)
        {
            //print("同群移動");
            if (toIndex > fromIndex)    //往後移的 Case
            {
                toIndex--;
            }
            if (toIndex == fromIndex)
            {
                //print("移動 Index 實質相同，不必去移!!");
                return false;
            }
        }

        if (fmList[fromIndex] != doll)
        {
            print("ERROR: ChangeDollPosition!!  Doll not in fromGroup " + fromGroup + " Index:  " + fromIndex + " -- " + doll.name);
            return false;
        }
        fmList.RemoveAt(fromIndex);

        if (toIndex >= toList.Count)
        {
            toList.Add(doll);
        }
        else
        {
            toList.Insert(toIndex, doll);
        }

        if (fromGroup == (int)GROUP_TYPE.FRONT || toGroup == (int)GROUP_TYPE.FRONT)
            BuildFrontSlots();
        if (fromGroup == (int)GROUP_TYPE.LEFT || toGroup == (int)GROUP_TYPE.LEFT)
            BuildLRSlots(true);
        if (fromGroup == (int)GROUP_TYPE.RIGHT || toGroup == (int)GROUP_TYPE.RIGHT)
            BuildLRSlots(false);
        if (fromGroup == (int)GROUP_TYPE.BACK || toGroup == (int)GROUP_TYPE.BACK)
            BuildBackSlots();

        //由於會有插入導致許多 Doll 的 Index 都改變的情況，直接一次性把 PlayerData 中的資料重新設定
        SaveAllToPlayerData();

        return true;
    }

    public List<Doll> GetFrontList() { return frontList; }
    public List<Doll> GetBackList() { return backList; }
    //public List<Doll> GetMiddleList() { return middleList; }
    public List<Doll> GetLeftList() { return leftList; }
    public List<Doll> GetRightList() { return rightList; }
}
