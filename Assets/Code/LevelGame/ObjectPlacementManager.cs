using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//配合 MazeGameManager 使用，統一放置各房間或通道隨機可破壞物件的系統
//特別是針對可收集 Doll 的隨機擺放

public class ObjectPlacementManager : MonoBehaviour
{
    public RoomObjectPlacement.ObjectInfo[] fixObjects;

    public RoomObjectPlacement.ObjectInfo[] randomObjects;
    public int randomSelectNum = 3;

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

        foreach (MazeGameManagerBase.RoomInfo ri in roomList)
        {
            rop.Build(ri);
        }

    }

}
