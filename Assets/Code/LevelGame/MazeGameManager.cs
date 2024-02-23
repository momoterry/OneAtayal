using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGameManagerBase:MonoBehaviour
{
    virtual public void AddRoom(Vector3 center, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth) { }
    virtual public void BuildAll() {}
}

public class MazeGameManager : MazeGameManagerBase
{
    public class RoomInfo
    {
        public Vector3 vCenter;
        public float width;
        public float height;
        public float mainRatio;
        public CELL_BASE cell;
    }
    public RoomGameplayBase mainGame;

    protected List<RoomInfo> mainRoomList = new List<RoomInfo>();
    override public void AddRoom(Vector3 vCenter, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth) 
    {
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.vCenter = vCenter;
        roomInfo.width = width;
        roomInfo.height = height;
        roomInfo.mainRatio = mainRatio;
        roomInfo.cell = cell;
        if (isMain)
            mainRoomList.Add(roomInfo);
    }

    override public void BuildAll() 
    {
        foreach (RoomInfo room in mainRoomList)
        {
            //print("Build One Main Room!!" + room.mainRatio);
            if (mainGame)
                mainGame.Build(room);
        }
    }

}
