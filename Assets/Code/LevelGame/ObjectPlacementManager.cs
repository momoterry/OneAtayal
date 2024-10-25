using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//配合 MazeGameManager 使用，統一放置各房間或通道隨機可破壞物件的系統
//特別是針對可收集 Doll 的隨機擺放

public class ObjectPlacementManager : MonoBehaviour
{
    public RoomObjectPlacement.ObjectInfo[] fixObjects;

    [System.Serializable]
    public class DollObjectInfo : RoomObjectPlacement.ObjectInfo
    {
        public float powerRatio = 1.0f; // 當數值 > 1.0f 時，出現的機率會校正，只用在 forceRandomNum 的場合
    }

    public DollObjectInfo[] randomObjects;
    public int randomSelectNum = 3;
    public bool randomObjectInPathOnly = false;
    public float forceRandomNum = 0;     //如果有指定 > 0 的值，會以量的方式反算回隨機物件的機率


    protected List<MazeGameManagerBase.RoomInfo> roomList = new List<MazeGameManagerBase.RoomInfo>();

    public void AddRooms(List<MazeGameManagerBase.RoomInfo> _list)
    {
        roomList.AddRange(_list);
    }

    public void BuildAll()
    {
        //print("ObjectPlacementManager BuildAll 總共 Room 數: " + roomList.Count);

        int randomCount = randomSelectNum > randomObjects.Length ? randomObjects.Length : randomSelectNum;

        RoomObjectPlacement.ObjectInfo[] allObject = new RoomObjectPlacement.ObjectInfo[fixObjects.Length + randomCount];
        for (int i = 0; i < fixObjects.Length; i++)
        {
            allObject[i] = fixObjects[i];
        }

        if (randomCount > 0)
        {
            OneUtility.Shuffle(randomObjects);
            for (int i = 0; i< randomCount; i++)
            {
                allObject[fixObjects.Length + i] = randomObjects[i];
            }
        }

        GameObject o = new GameObject("ROP_Obj");
        o.transform.SetParent(transform, false);
        RoomObjectPlacement rop = o.AddComponent<RoomObjectPlacement>();
        rop.objs = allObject;

        RoomObjectPlacement noRandomRop = null;
        if (randomObjectInPathOnly)
        {
            GameObject fo = new GameObject("Fix_ROP_Obj");
            fo.transform.SetParent(transform, false);
            noRandomRop = o.AddComponent<RoomObjectPlacement>();
            noRandomRop.objs = fixObjects;
        }

        if (forceRandomNum > 0)
        {
            int allBlockNum = 0;
            foreach (MazeGameManagerBase.RoomInfo ri in roomList)
            {
                if (!(randomObjectInPathOnly && !ri.cell.isPath))
                    allBlockNum += rop.GetRoomBlockNums(ri);
            }
            print("allBlockNum: " + allBlockNum);
            float allRondomPercent = 0;
            for (int i = 0; i < randomCount; i++)
            {
                allRondomPercent += randomObjects[i].placePercent;
            }
            float predictRandomNum = allRondomPercent * allBlockNum * 0.01f;
            print("預計量: " +  predictRandomNum + "  需求量: " + forceRandomNum);
            float forceFixRatio = forceRandomNum / predictRandomNum;
            for (int i = 0; i < randomCount; i++)
            {
                randomObjects[i].placePercent *= forceFixRatio;
                if (randomObjects[i].powerRatio > 1.0f)
                    randomObjects[i].placePercent /= randomObjects[i].powerRatio;       //power 較強的物件需要降低出現率
                print(randomObjects[i].objRef.name + " 修正成: " + randomObjects[i].placePercent);
            }
        }

        foreach (MazeGameManagerBase.RoomInfo ri in roomList)
        {
            if (randomObjectInPathOnly && ri.cell.isPath == false)
            {
                print("不是 Path，不產生石化巫靈");
                noRandomRop.Build(ri);
            }
            else
            {
                rop.Build(ri);
            }
        }

    }

}
