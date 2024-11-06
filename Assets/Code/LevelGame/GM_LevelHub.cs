using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GM_LevelHub : MazeGameManagerBase
{
    public DungeonEnteryHandler.EnteryData[] easyDungeons;
    public DungeonEnteryHandler.EnteryData[] hardDungeons;

    public GameObject defaultPortalRef;

    protected List<RoomInfo> mainList = new List<RoomInfo>();
    protected List<RoomInfo> branchList = new List<RoomInfo>();

    protected List<RoomInfo> allList = new List<RoomInfo>();

    public override RoomInfo AddRoom(Vector3 vCenter, float width, float height, MG_MazeOneBase.CELL cell, float mainRatio, float doorWidth, float doorHeight, RectInt mapRect)
    {
        if (cell.isPath)
            return null;

        RoomInfo roomInfo =  base.AddRoom(vCenter, width, height, cell, mainRatio, doorWidth, doorHeight, mapRect);

        if (cell.isMain)
            mainList.Add(roomInfo);
        else
        {
            int doorCount = 0;
            doorCount += roomInfo.cell.U ? 1 : 0;
            doorCount += roomInfo.cell.D ? 1 : 0;
            doorCount += roomInfo.cell.L ? 1 : 0;
            doorCount += roomInfo.cell.R ? 1 : 0;
            if (doorCount == 1)
            {
                branchList.Add(roomInfo);
            }
        }

        allList.Add(roomInfo);

        return roomInfo;
    }

    public override RoomInfo AddRoom(Vector3 vCenter, MG_MazeOneBase.CELL cell, float mainRatio, RectInt mapRect)
    {
        if (cell.isPath)
            return null;
        RoomInfo roomInfo = base.AddRoom(vCenter, cell, mainRatio, mapRect);

        if (cell.isMain)
            mainList.Add(roomInfo);
        else
        {
            int doorCount = 0;
            doorCount += roomInfo.cell.U ? 1 : 0;
            doorCount += roomInfo.cell.D ? 1 : 0;
            doorCount += roomInfo.cell.L ? 1 : 0;
            doorCount += roomInfo.cell.R ? 1 : 0;
            if (doorCount == 1)
            {
                branchList.Add(roomInfo);
            }
        }

        allList.Add(roomInfo);

        return roomInfo;
    }

    int CompareRoom(RoomInfo A, RoomInfo B)
    {
        return (int)(100.0f * (A.mainRatio - B.mainRatio));
    }

    public override void BuildAll()
    {
        base.BuildAll();

        allList.Sort(CompareRoom);

        if (allList.Count < (easyDungeons.Length + hardDungeons.Length))
            One.ERROR("Room 的數量不足，HUB 將會有重疊的現象 !!!!");

        for (int i = 0; i < easyDungeons.Length; i++)
        {
            GameObject o = BattleSystem.SpawnGameObj(defaultPortalRef, allList[i].vCenter);
            DungeonEnteryHandler handler = o.GetComponent<DungeonEnteryHandler>();
            if (handler)
            {
                handler.SetEnteryData(easyDungeons[i]);
            }
        }

        for (int i = 0; i < hardDungeons.Length; i++)
        {
            GameObject o = BattleSystem.SpawnGameObj(defaultPortalRef, allList[allList.Count - i - 1].vCenter);
            DungeonEnteryHandler handler = o.GetComponent<DungeonEnteryHandler>();
            if (handler)
            {
                handler.SetEnteryData(hardDungeons[i]);
            }
        }

        //for (int i= mainDunNum; i<mainList.Count; i++)
        //{
        //    BattleSystem.SpawnGameObj(defaultPortalRef, mainList[i].vCenter);
        //}

        //for (int i = 0; i < branchList.Count; i++)
        //{
        //    BattleSystem.SpawnGameObj(defaultPortalRef, branchList[i].vCenter);
        //}
    }

}
