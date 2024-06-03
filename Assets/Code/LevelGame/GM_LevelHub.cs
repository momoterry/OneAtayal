using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_LevelHub : MazeGameManagerBase
{
    public DungeonEnteryHandler.EnteryData[] easyDungeons;

    public GameObject defaultPortalRef;

    protected List<RoomInfo> mainList = new List<RoomInfo>();
    protected List<RoomInfo> branchList = new List<RoomInfo>();


    public override RoomInfo AddRoom(Vector3 vCenter, float width, float height, MG_MazeOneBase.CELL cell, float mainRatio, float doorWidth, float doorHeight)
    {
        if (cell.isPath)
            return null;

        RoomInfo roomInfo =  base.AddRoom(vCenter, width, height, cell, mainRatio, doorWidth, doorHeight);

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

        return roomInfo;
    }

    public override void BuildAll()
    {
        base.BuildAll();

        int mainDunNum = mainList.Count > easyDungeons.Length ? easyDungeons.Length : mainList.Count;
        for (int i = 0; i < mainDunNum; i++)
        {
            GameObject o = BattleSystem.SpawnGameObj(defaultPortalRef, mainList[i].vCenter);
            DungeonEnteryHandler handler = o.GetComponent<DungeonEnteryHandler>();
            if (handler)
            {
                handler.SetEnteryData(easyDungeons[i]);
            }
        }

        for (int i= mainDunNum; i<mainList.Count; i++)
        {
            BattleSystem.SpawnGameObj(defaultPortalRef, mainList[i].vCenter);
        }

        for (int i = 0; i < branchList.Count; i++)
        {
            BattleSystem.SpawnGameObj(defaultPortalRef, branchList[i].vCenter);
        }
    }

}
