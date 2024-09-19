using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//配合 MazeGameManager 使用，統一放置各房間或通道隨機可破壞物件的系統
//特別是針對可收集 Doll 的隨機擺放

public class ObjectPlacementManager : MonoBehaviour
{
    public RoomObjectPlacement.ObjectInfo[] finxObjects;
    protected List<MazeGameManagerBase.RoomInfo> roomList = new List<MazeGameManagerBase.RoomInfo>();

    public void AddRooms(List<MazeGameManagerBase.RoomInfo> _list)
    {
        roomList.AddRange(_list);
    }

    public void BuildAll()
    {
        print("ObjectPlacementManager BuildAll 總共 Room 數: " + roomList.Count);

        //RoomObjectPlacement theROP = new RoomObjectPlacement();
        //theROP.gameObject.transform.parent = transform;

        GameObject o = new GameObject("ROP_Obj");
        o.transform.SetParent(transform, false);
        RoomObjectPlacement rop = o.AddComponent<RoomObjectPlacement>();
        rop.objs = finxObjects;

        foreach (MazeGameManagerBase.RoomInfo ri in roomList)
        {
            rop.Build(ri);
        }

    }

}
