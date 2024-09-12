using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_DynamicBeta : DM_Dynamic
{
    protected int[] frontLineNums = new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    protected void GetFrontLines( int _num, out int nLine, out int nLastCount)
    {
        nLine = 0;
        nLastCount = 0;
        if (_num <= 0)
            return;

        //int sum = 0;
        int nLeft = _num;
        for (int i=0; i<frontLineNums.Length; i++)
        {
            //sum += frontLineNums[i];
            if (nLeft <= frontLineNums[i])
            {
                nLine = i + 1;
                nLastCount = (nLeft == frontLineNums[i]) ? 0:nLeft;
                return;
            }
            nLeft -= frontLineNums[i];
        }
        One.LOG("超過 Front 數量上限 !!!!");
        return;
    }

    protected override void BuildFrontSlots()
    {
        int frontNum = frontList.Count;

        if (frontNum <= 0)
            return;


        int nLine, nLastCount;
        GetFrontLines(frontNum, out nLine, out nLastCount);
        //print("前排需要行數: " + nLine + "最後一排數: " + nLastCount);
        int leftCount = frontNum;
        int currIndex = 0;

        int nLineReduceOne = 0; //需要減少一個的行數
        if (nLastCount > 0 && nLastCount < nLine)
        {
            nLineReduceOne = nLine - nLastCount;
        }

        float fPos = Mathf.Max(1.5f, 2.0f - (float)(nLine - 1) * 0.5f) + allShift;  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine - 1);

        //if ( nL)

        for (int l = 0; l < nLine; l++)
        {
            int num = frontLineNums[nLine - l - 1];     //倒著來
            //if (l < nLineReduceOne)
            if ((nLine - l - 1) <= nLineReduceOne && l != nLine - 1)
                num--;
            if (leftCount < num)
                num = leftCount;
            //print("Line: " + l + " Count: " + num + " nLineReduceOne: " + nLineReduceOne);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i = currIndex; i < currIndex + num; i++)
            {
                //print("Prepare ..." + i);
                frontList[i].GetSlot().localPosition = new Vector3(lPos, 0, fPos);
                lPos += slotWidth;
            }
            fPos -= slotDepth;
            leftCount -= num;
            currIndex += num;
        }
        //print("算完 Front, leftCount = " + leftCount);
    }
}
