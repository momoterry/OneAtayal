using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_DynamicBeta : DM_Dynamic
{
    protected int[] frontLineNums = new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    protected int GetFrontLines( int _num)
    {
        if (_num <= 0)
            return 0;

        int sum = 0;
        for (int i=0; i<frontLineNums.Length; i++)
        {
            sum += frontLineNums[i];
            if (_num <= sum)
            {
                return i + 1;
            }
        }
        One.LOG("超過 Front 數量上限 !!!!");
        return frontLineNums.Length + 1;
    }

    protected override void BuildFrontSlots()
    {
        //FrontWidth = 5;
        int frontNum = frontList.Count;

        if (frontNum <= 0)
            return;

        //int nLine = ((frontNum - 1) / FrontWidth) + 1;
        int nLine = GetFrontLines(frontNum);
        print("前排需要行數: " + nLine);
        //int lastLineCount = (frontNum - 1) % FrontWidth + 1;
        int leftCount = frontNum;
        int currIndex = 0;

        float fPos = Mathf.Max(1.5f, 2.0f - (float)(nLine - 1) * 0.5f) + allShift;  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine - 1);

        for (int l = 0; l < nLine; l++)
        {
            //int num = FrontWidth;
            //if (l == nLine - 1)
            //    num = lastLineCount;
            int num = frontLineNums[nLine - l - 1];     //倒著來
            if (leftCount < num)
                num = leftCount;
            print("Line: " + l + " Count: " + num);

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
        print("算完 Front, leftCount = " + leftCount);
    }
}
