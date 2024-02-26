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

    [System.Serializable]
    public class FixGameInfo        //固定出現的 Game
    {
        public int relativeIndex;
        public RoomGameplayBase game;
    }
    [System.Serializable]
    public class FixBranchEndGameInfo
    {
        public RoomGameplayBase game;
    }
    public RoomGameplayBase defautMainGame;
    public FixGameInfo[] fixStartGames;

    public RoomGameplayBase defaultBranchGame;
    public FixBranchEndGameInfo[] fixBranchEndGames;

    protected List<RoomInfo> mainRoomList = new List<RoomInfo>();
    protected List<RoomInfo> normalRoomList = new List<RoomInfo>();
    protected List<RoomInfo> branchEndRoomList = new List<RoomInfo>();

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
        else
        {
            int doorCount = 0;
            doorCount += roomInfo.cell.U ? 1 : 0;
            doorCount += roomInfo.cell.D ? 1 : 0;
            doorCount += roomInfo.cell.L ? 1 : 0;
            doorCount += roomInfo.cell.R ? 1 : 0;
            if (doorCount == 1)
            {
                branchEndRoomList.Add(roomInfo);
            }
            else
            {
                normalRoomList.Add(roomInfo);
            }
        }
    }

    override public void BuildAll()
    {
        RoomGameplayBase[] mainGames = new RoomGameplayBase[mainRoomList.Count];
        foreach (FixGameInfo fg in fixStartGames)
        {
            if (fg.relativeIndex > 0 && fg.relativeIndex <= mainRoomList.Count)
                mainGames[fg.relativeIndex - 1] = fg.game;
            else
                print("Invalid index in fixStartGames: " + fg.relativeIndex);
        }

        foreach (RoomInfo room in mainRoomList)
        {
            int mIndex = Mathf.RoundToInt(room.mainRatio * (mainRoomList.Count + 1.0f)) - 1;
            print("Build One Main Room!! " + mIndex);
            if (mainGames[mIndex])
                mainGames[mIndex].Build(room);
            else if (defautMainGame)
                defautMainGame.Build(room);
        }

        OneUtility.Shuffle<RoomInfo>(branchEndRoomList);
        int iCount = 0;
        foreach (FixBranchEndGameInfo fg in fixBranchEndGames)
        {
            if (iCount >= branchEndRoomList.Count)
            {
                print("ERROR: branchEndRoomList 分支端點數量不足!! 需要 " + fixBranchEndGames.Length);
                break;
            }
            if (fg.game)
            {
                fg.game.Build(branchEndRoomList[iCount]);
            }
            iCount++;
        }
        //剩下的支線端點
        if (!defaultBranchGame)
            return;
        for (int i=iCount; i < branchEndRoomList.Count; i++)
        {
            defaultBranchGame.Build(branchEndRoomList[i]);
        }
        foreach (RoomInfo room in normalRoomList)
        {
            defaultBranchGame.Build(room);
        }
    }

}
