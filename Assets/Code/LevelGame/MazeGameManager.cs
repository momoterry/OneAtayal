using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGameManagerBase:MonoBehaviour
{
    public class RoomLayout
    {
        public float width;
        public float height;
        public float doorWidth;
        public float doorHeight;
        public float wallWidth;
        public float wallHeight;
    }
    public class RoomInfo
    {
        public Vector3 vCenter;
        public float width;
        public float height;
        public float mainRatio;
        public MG_MazeOneBase.CELL cell;
        public float doorWidth;
        public float doorHeight;
        public float wallWidth;
        public float wallHeight;
        public float diffAddRatio;  //難度增加量 預設 = 0，1.0f = > 兩倍敵人數量
        public int enemyLV; //敵人等級，目前只支援使用 EnemyGroup 時
    }

    protected RoomLayout roomLayout = new RoomLayout();

    public float difficultRateMin = 1.0f;   //最小難度率，用來調整敵人數量
    public float difficultRateMax = 2.0f;   //最大難度率，用來調整敵人數量
    public int enmeyLV = 1;                 //敵人等級，目前只有針對 RoomEnemyGroup 中指定了 enemyID 的才有作用

    virtual public void SetDefaultRoomLayout(float width, float height, float doorWidth, float doorHeight, float wallWidth, float wallHeight)
    {
        roomLayout.wallWidth = wallWidth;
        roomLayout.wallHeight = wallHeight;
        roomLayout.width = width;
        roomLayout.height = height;
        roomLayout.doorHeight = doorHeight;
        roomLayout.doorWidth = doorWidth;
    }

    virtual public void Init(GameManagerDataBase data)
    {
        difficultRateMin = data.difficultRateMin <= 0 ? 1.0f : data.difficultRateMin;
        difficultRateMax = data.difficultRateMax <= difficultRateMin ? difficultRateMin : data.difficultRateMax;
        enmeyLV = data.enmeyLV <= 0 ? 1 : data.enmeyLV;
    }

    virtual public RoomInfo AddRoom(Vector3 vCenter, float width, float height, MG_MazeOneBase.CELL cell, float mainRatio, float doorWidth, float doorHeight) 
    {
        RoomInfo roomInfo = AddRoom(vCenter, cell, mainRatio);
        //RoomInfo roomInfo = new RoomInfo();
        //roomInfo.vCenter = vCenter;
        roomInfo.width = width;
        roomInfo.height = height;
        roomInfo.doorWidth = doorWidth;
        roomInfo.doorHeight = doorHeight;
        //roomInfo.mainRatio = mainRatio;
        //roomInfo.cell = cell;
        //roomInfo.diffAddRatio = ((difficultRateMax - difficultRateMin) * mainRatio + difficultRateMin) - 1.0f;
        //roomInfo.enemyLV = enmeyLV;
        return roomInfo;
    }

    virtual public RoomInfo AddRoom(Vector3 vCenter, MG_MazeOneBase.CELL cell, float mainRatio)
    {
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.vCenter = vCenter;
        roomInfo.width = roomLayout.width;
        roomInfo.height = roomLayout.height;
        roomInfo.doorWidth = roomLayout.doorWidth;
        roomInfo.doorHeight = roomLayout.doorHeight;
        roomInfo.wallWidth = roomLayout.wallWidth;
        roomInfo.wallHeight = roomLayout.wallHeight;
        roomInfo.mainRatio = mainRatio;
        roomInfo.cell = cell;
        roomInfo.diffAddRatio = ((difficultRateMax - difficultRateMin) * mainRatio + difficultRateMin) - 1.0f;
        roomInfo.enemyLV = enmeyLV;
        return roomInfo;
    }

    virtual public void BuildAll() {}
}

public class MazeGameManager : MazeGameManagerBase
{

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
    [System.Serializable]
    public class NormalGameInfo
    {
        public RoomGameplayBase game;
        public float ratioPercent = 50;
    }
    //public RoomGameplayBase defaultMainGame;
    public NormalGameInfo[] defaultMainGames;
    public FixGameInfo[] fixStartGames;         //relativeIndex = 0 代表起點，不能用
    public FixGameInfo[] fixEndGames;           //relativeIndex = 0 代表終點，不能用

    public NormalGameInfo[] defautPathGames;

    //public RoomGameplayBase defaultBranchGame;
    public NormalGameInfo[] defaultBranchGames;
    public FixBranchEndGameInfo[] fixBranchEndGames;

    public GameObject treasureBoxRef;

    protected List<RoomInfo> mainRoomList = new List<RoomInfo>();
    protected List<RoomInfo> normalRoomList = new List<RoomInfo>();
    protected List<RoomInfo> branchEndRoomList = new List<RoomInfo>();

    protected List<RoomInfo> pathList = new List<RoomInfo>();
    protected List<RoomInfo> branchEndPathList = new List<RoomInfo>();

    protected List<RoomGameplayBase> allBranchGames = new List<RoomGameplayBase>();

    public override void Init(GameManagerDataBase data)
    {
        base.Init(data);

        //有關寶箱配置
        //if (data.specialReward != null && data.specialReward != "" && treasureBoxRef)
        if (data.specialRewardNum > 0 && treasureBoxRef)
        {
            GameObject rgObj = new GameObject("RewardRoomGameplay");
            rgObj.transform.parent = this.transform;
            RoomGameplay rg = rgObj.AddComponent<RoomGameplay>();
            GameObject tObjRef = BattleSystem.SpawnGameObj(treasureBoxRef, rgObj.transform.position);
            tObjRef.SetActive(false);
            tObjRef.transform.parent = rgObj.transform;
            TreasureBox tBox = tObjRef.GetComponent<TreasureBox>();
            if (tBox != null && data.specialReward != null && data.specialReward != "")
            {
                //tBox.AddSpecialRewardItem(data.specialReward);

                //TODO: 這裡的做法太暴力
                print("暴力法加入了寶物: " + data.specialReward);
                tBox.fixSpecialRewards = new string[1];
                tBox.fixSpecialRewards[0] = data.specialReward;
                //print("Add Treasure....");
            }
            rg.centerGame = tObjRef;
            int count = OneUtility.FloatToRandomInt(data.specialRewardNum);
            for (int i = 0; i < count; i++)
                allBranchGames.Add(rg);
            //print("加入了寶箱數: " + count);
        }
    }

    //override public RoomInfo AddRoom(Vector3 vCenter, float width, float height, MG_MazeOneBase.CELL cell, float mainRatio, float doorWidth, float doorHeight) 
    //{
    //    RoomInfo roomInfo = base.AddRoom(vCenter, width, height, cell, mainRatio, doorWidth, doorHeight);

    //    int doorCount = 0;
    //    doorCount += roomInfo.cell.U ? 1 : 0;
    //    doorCount += roomInfo.cell.D ? 1 : 0;
    //    doorCount += roomInfo.cell.L ? 1 : 0;
    //    doorCount += roomInfo.cell.R ? 1 : 0;
    //    bool isTerminal = doorCount == 1;
    //    print("Door Count: " + doorCount);


    //    if (cell.isPath)
    //    {
    //        if (isTerminal)
    //        {
    //            branchEndPathList.Add(roomInfo);
    //        }
    //        else
    //        {
    //            pathList.Add(roomInfo);
    //        }
    //        return roomInfo;
    //    }

    //    if (cell.isMain)
    //        mainRoomList.Add(roomInfo);
    //    else
    //    {
    //        //int doorCount = 0;
    //        //doorCount += roomInfo.cell.U ? 1 : 0;
    //        //doorCount += roomInfo.cell.D ? 1 : 0;
    //        //doorCount += roomInfo.cell.L ? 1 : 0;
    //        //doorCount += roomInfo.cell.R ? 1 : 0;
    //        //if (doorCount == 1)
    //        if (isTerminal)
    //        {
    //            branchEndRoomList.Add(roomInfo);
    //        }
    //        else
    //        {
    //            normalRoomList.Add(roomInfo);
    //        }
    //    }
    //    return roomInfo;
    //}

    public override RoomInfo AddRoom(Vector3 vCenter, MG_MazeOneBase.CELL cell, float mainRatio)
    {
        RoomInfo roomInfo = base.AddRoom(vCenter, cell, mainRatio);

        int doorCount = 0;
        doorCount += roomInfo.cell.U ? 1 : 0;
        doorCount += roomInfo.cell.D ? 1 : 0;
        doorCount += roomInfo.cell.L ? 1 : 0;
        doorCount += roomInfo.cell.R ? 1 : 0;
        bool isTerminal = doorCount == 1;
        //print("Door Count: " + doorCount);

        if (cell.isPath)
        {
            if (isTerminal)
            {
                branchEndPathList.Add(roomInfo);
            }
            else
            {
                pathList.Add(roomInfo);
            }
            return roomInfo;
        }

        if (cell.isMain)
            mainRoomList.Add(roomInfo);
        else
        {
            //int doorCount = 0;
            //doorCount += roomInfo.cell.U ? 1 : 0;
            //doorCount += roomInfo.cell.D ? 1 : 0;
            //doorCount += roomInfo.cell.L ? 1 : 0;
            //doorCount += roomInfo.cell.R ? 1 : 0;
            //if (doorCount == 1)
            if (isTerminal)
            {
                branchEndRoomList.Add(roomInfo);
            }
            else
            {
                normalRoomList.Add(roomInfo);
            }
        }
        return roomInfo;
    }


    protected int CompareMainRoom(RoomInfo roomA, RoomInfo roomB)
    {
        return Mathf.RoundToInt(1000.0f * (roomA.mainRatio - roomB.mainRatio));
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
        foreach (FixGameInfo fg in fixEndGames)
        {
            if (fg.relativeIndex > 0 && fg.relativeIndex <= mainRoomList.Count)
            {
                if (mainGames[mainGames.Length - fg.relativeIndex] != null)
                    One.ERROR("fixEndGames 跟 fixStartGames 重疊!!!! " + (mainGames.Length - fg.relativeIndex));
                mainGames[mainGames.Length - fg.relativeIndex] = fg.game;
            }
            else
                print("Invalid index in fixEndGames: " + fg.relativeIndex);
        }

        //print("mainRoomList Count: " + mainRoomList.Count);
        mainRoomList.Sort(CompareMainRoom);

        List<RoomInfo> normalMainRoomList = new List<RoomInfo>();
        int mIndex = 0;
        foreach (RoomInfo room in mainRoomList)
        {
            //int mIndex = Mathf.RoundToInt(room.mainRatio * (mainRoomList.Count + 1.0f)) - 1;
            //print("Build One Main Room!! " + mIndex + " main Ratio: " + room.mainRatio);
            if (mainGames[mIndex])
                mainGames[mIndex].Build(room);
            else
                normalMainRoomList.Add(room);
            //else if (defaultMainGame)
            //    defaultMainGame.Build(room);
            mIndex++;
        }

        for (int i=0; i<fixBranchEndGames.Length; i++)
        {
            allBranchGames.Add(fixBranchEndGames[i].game);
        }

        if (allBranchGames.Count > branchEndRoomList.Count)
        {
            int iBranchToAdd = allBranchGames.Count - branchEndRoomList.Count;
            //print("branchEndRoomList 分支端點數量不足!! 需要補足: " + iBranchToAdd);
            while (iBranchToAdd > 0)
            {
                if (branchEndPathList.Count <= 0)
                    break;
                int iRd = Random.Range(0, branchEndPathList.Count);
                branchEndRoomList.Add(branchEndPathList[iRd]);
                branchEndPathList.RemoveAt(iRd);
                iBranchToAdd--;
                //print("用 branchEndPathList 補了一個");
            }
            if (iBranchToAdd > 0)
                print("branchEnd 分支端點數量不足!! 需要補足: " + iBranchToAdd);
            while (iBranchToAdd > 0)
            {
                if (normalRoomList.Count <= 0)
                    break;
                int iRd = Random.Range(0, normalRoomList.Count);
                branchEndRoomList.Add(normalRoomList[iRd]);
                normalRoomList.RemoveAt(iRd);
                iBranchToAdd--;
            }
            while (iBranchToAdd > 0)
            {
                if (normalMainRoomList.Count <= 0)
                    break;
                int iRd = Random.Range(0, normalMainRoomList.Count);
                branchEndRoomList.Add(normalMainRoomList[iRd]);
                normalMainRoomList.RemoveAt(iRd);
                iBranchToAdd--;
            }
            if (iBranchToAdd != 0)
            {
                One.ERROR("補完所有房間還是無法滿足 ......... ");
            }
        }

        OneUtility.Shuffle<RoomInfo>(branchEndRoomList);
        int iCount = 0;
        foreach (RoomGameplayBase g in allBranchGames)
        {
            if (iCount >= branchEndRoomList.Count)
            {
                print("ERROR: branchEndRoomList 分支端點數量不足!! 需要 " + allBranchGames.Count);
                break;
            }
            if (g)
            {
                g.Build(branchEndRoomList[iCount]);
            }
            iCount++;
        }

        //剩下的主線
        foreach (RoomInfo room in normalMainRoomList)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultMainGames);
            if (game != null)
                game.Build(room);
        }
        //剩下的支線端點
        for (int i = iCount; i < branchEndRoomList.Count; i++)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultBranchGames);
            if (game != null)
                game.Build(branchEndRoomList[i]);
        }
        //剩下的支線
        foreach (RoomInfo room in normalRoomList)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultBranchGames);
            if (game != null)
                game.Build(room);
        }
        //所有的「通道」
        foreach (RoomInfo room in pathList)
        {
            RoomGameplayBase game = GetRandomGameplay(defautPathGames);
            if (game != null)
                game.Build(room);
        }
    }

    RoomGameplayBase GetRandomGameplay(NormalGameInfo[] games)
    {
        float accumulated = 0;
        float currRandom = Random.Range(0, 100.0f);
        foreach (NormalGameInfo gInfo in games)
        {
            accumulated += gInfo.ratioPercent;
            if (accumulated > currRandom)
            {
                return gInfo.game;
            }
        }
        return null;
    }

}
