using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;


public class DecadeGenerateParameter
{
    public int mapValue;
}

public class MapDecadeGeneratorBase : MonoBehaviour
{
    public virtual void BuildAll(OneMap theMap, DecadeGenerateParameter para ) { }
}


[System.Serializable]
public class DecadeData
{
    public GameObject decadeObjRef;
    public float ratePercent;
    public Vector3 posShift;
    public int upBound = 1;
    public int lowBound = 1;
    public int leftBound = 1;
    public int rightBound = 1;
}

public class MapDecadeGenerator : MapDecadeGeneratorBase
{
    public DecadeData[] decades;
    public bool decadeDefaultArea = true;

    protected OneMap theMap;
    protected DecadeGenerateParameter thePara = null;
    protected GameObject decadeRoot = null;

    public override void BuildAll(OneMap _theMap, DecadeGenerateParameter para)
    {
        theMap = _theMap;
        thePara = para;
        //theMap.PrintMap();
        decadeRoot = new GameObject("DecadeRoot");
        //decadeRoot.transform.rotation = Quaternion.Euler(90, 0, 0);
        decadeRoot.transform.position = Vector3.zero;
        for (int x = theMap.xMin; x <= theMap.xMax; x++)
        {
            for (int y = theMap.yMin; y <= theMap.yMax; y++)
            {
                //int value = theMap.GetValue(x, y);
                //if (value == para.mapValue || (value == OneMap.DEFAULT_VALUE && decadeDefaultArea))
                //{
                //    TryGenerateDecadeAt(x, y);
                //}
                //if (CheckDecadePossible(x, y))
                //{
                //    TryGenerateDecadeAt(x, y);
                //}
                TryGenerateDecadeAt(x, y);
            }
        }
        SceneStaticManager sm = decadeRoot.AddComponent<SceneStaticManager>();
        sm.sortingByLowerBound = true;
        sm.runAgainAtStart = false;
        sm.SetupSorting();
    }

    protected bool CheckDecadePossible( DecadeData dData, int x, int y)
    {
        for (int ix = x - dData.leftBound; ix <= x + dData.rightBound; ix++)
        {
            for (int iy = y - dData.lowBound; iy <= y + dData.upBound; iy++)
            {
                int value = theMap.GetValue(ix, iy);
                if (value != thePara.mapValue && !(decadeDefaultArea && value == OneMap.DEFAULT_VALUE))
                {
                    return false;
                }
            }
        }
        return true;
    }

    protected void TryGenerateDecadeAt(int x, int y)
    {
        DecadeData dData = null;
        //GameObject oRef = null;
        float rd = Random.Range(0, 100.0f);
        for (int i=0; i<decades.Length; i++)
        {
            if (rd < decades[i].ratePercent)
            {
                if (CheckDecadePossible(decades[i], x, y))
                {
                    dData = decades[i];
                    break;
                }
            }
            rd -= decades[i].ratePercent;
        }
        if (dData == null)
            return;

        GameObject o = BattleSystem.SpawnGameObj(dData.decadeObjRef, new Vector3(x+0.5f, 0, y+0.5f) + dData.posShift);
        o.transform.SetParent(decadeRoot.transform);
    }

    //protected bool CheckDecadePossible(int x, int y)
    //{
    //    for (int ix = x-1; ix <= x+1; ix++)
    //    {
    //        for (int iy = y-1; iy <= y+1; iy++)
    //        {
    //            int value = theMap.GetValue(ix, iy);
    //            if (value != thePara.mapValue && !(decadeDefaultArea&&value == OneMap.DEFAULT_VALUE))
    //            {
    //                return false;
    //            }
    //        }
    //    }

    //    return true;
    //}

}
