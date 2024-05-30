using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGameManagerBase:MonoBehaviour
{
    public class RoomInfo
    {
        public Vector3 vCenter;
        public float width;
        public float height;
        public float mainRatio;
        public CELL_BASE cell;
        public float doorWidth;
        public float doorHeight;
        public float diffAddRatio;  //���׼W�[�q �w�] = 0�A1.0f = > �⭿�ĤH�ƶq
        public int enemyLV; //�ĤH���šA�ثe�u�䴩�ϥ� EnemyGroup ��
    }

    public float difficultRateMin = 1.0f;   //�̤p���ײv�A�Ψӽվ�ĤH�ƶq
    public float difficultRateMax = 2.0f;   //�̤j���ײv�A�Ψӽվ�ĤH�ƶq
    public int enmeyLV = 1;                 //�ĤH���šA�ثe�u���w�� RoomEnemyGroup �����w�F enemyID ���~���@��

    virtual public void Init(GameManagerDataBase data)
    {
        difficultRateMin = data.difficultRateMin;
        difficultRateMax = data.difficultRateMax;
        enmeyLV = data.enmeyLV;
    }

    virtual public RoomInfo AddRoom(Vector3 vCenter, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth, float doorHeight) 
    {
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.vCenter = vCenter;
        roomInfo.width = width;
        roomInfo.height = height;
        roomInfo.doorWidth = doorWidth;
        roomInfo.doorHeight = doorHeight;
        roomInfo.mainRatio = mainRatio;
        roomInfo.cell = cell;
        roomInfo.diffAddRatio = ((difficultRateMax - difficultRateMin) * mainRatio + difficultRateMin) - 1.0f;
        roomInfo.enemyLV = enmeyLV;
        return roomInfo;
    }
    virtual public RoomInfo AddPath(Vector3 vCenter, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth, float doorHeight) 
    {
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.vCenter = vCenter;
        roomInfo.width = width;
        roomInfo.height = height;
        roomInfo.doorWidth = doorWidth;
        roomInfo.doorHeight = doorHeight;
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
    public class FixGameInfo        //�T�w�X�{�� Game
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
    public FixGameInfo[] fixStartGames;         //relativeIndex = 0 �N��_�I�A�����
    public FixGameInfo[] fixEndGames;           //relativeIndex = 0 �N����I�A�����

    public NormalGameInfo[] defautPathGames;

    //public RoomGameplayBase defaultBranchGame;
    public NormalGameInfo[] defaultBranchGames;
    public FixBranchEndGameInfo[] fixBranchEndGames;

    public GameObject treasureBoxRef;

    protected List<RoomInfo> mainRoomList = new List<RoomInfo>();
    protected List<RoomInfo> normalRoomList = new List<RoomInfo>();
    protected List<RoomInfo> branchEndRoomList = new List<RoomInfo>();
    protected List<RoomInfo> pathList = new List<RoomInfo>();

    protected List<RoomGameplayBase> allBranchGames = new List<RoomGameplayBase>();

    public override void Init(GameManagerDataBase data)
    {
        base.Init(data);

        //�����_�c�t�m
        if (data.specialReward != null && data.specialReward != "" && treasureBoxRef)
        {
            GameObject rgObj = new GameObject("RewardRoomGameplay");
            rgObj.transform.parent = this.transform;
            RoomGameplay rg = rgObj.AddComponent<RoomGameplay>();
            GameObject tObjRef = BattleSystem.SpawnGameObj(treasureBoxRef, rgObj.transform.position);
            tObjRef.SetActive(false);
            tObjRef.transform.parent = rgObj.transform;
            TreasureBox tBox = tObjRef.GetComponent<TreasureBox>();
            if (tBox != null)
            {
                //tBox.AddSpecialRewardItem(data.specialReward);
                //print("�[�J�F�_��: " + data.specialReward);

                //TODO: �o�̪����k�ӼɤO
                tBox.fixSpecialRewards = new string[1];
                tBox.fixSpecialRewards[0] = data.specialReward;
                //print("Add Treasure....");
            }
            rg.centerGame = tObjRef;
            int count = OneUtility.FloatToRandomInt(data.specialRewardNum);
            for (int i = 0; i < count; i++)
                allBranchGames.Add(rg);
            //print("�[�J�F�_�c��: " + count);
        }
    }

    override public RoomInfo AddRoom(Vector3 vCenter, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth, float doorHeight) 
    {
        //RoomInfo roomInfo = new RoomInfo();
        //roomInfo.vCenter = vCenter;
        //roomInfo.width = width;
        //roomInfo.height = height;
        //roomInfo.doorWidth = doorWidth;
        //roomInfo.doorHeight = doorHeight;
        //roomInfo.mainRatio = mainRatio;
        //roomInfo.cell = cell;
        //roomInfo.diffAddRatio = ((difficultRateMax - difficultRateMin) * mainRatio + difficultRateMin) - 1.0f;
        //roomInfo.enemyLV = enmeyLV;
        RoomInfo roomInfo = base.AddRoom(vCenter, width, height, cell, isMain, mainRatio, doorWidth, doorHeight);
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
        return roomInfo;
    }

    override public RoomInfo AddPath(Vector3 vCenter, float width, float height, CELL_BASE cell, bool isMain, float mainRatio, float doorWidth, float doorHeight)
    {
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.vCenter = vCenter;
        roomInfo.width = width;
        roomInfo.height = height;
        roomInfo.doorWidth = doorWidth;
        roomInfo.doorHeight = doorHeight;
        roomInfo.mainRatio = mainRatio;
        roomInfo.cell = cell;
        roomInfo.diffAddRatio = ((difficultRateMax - difficultRateMin) * mainRatio + difficultRateMin) - 1.0f;
        roomInfo.enemyLV = enmeyLV;

        pathList.Add(roomInfo);
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
                    print("ERROR!!!! fixEndGames �� fixStartGames ���|!!!! " + (mainGames.Length - fg.relativeIndex));
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
            print("branchEndRoomList ������I�ƶq����!! �ݭn�ɨ�: " + iBranchToAdd);
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
                print("ERROR!!!! �ɧ��Ҧ��ж��٬O�L�k���� ......... ");
            }
        }

        OneUtility.Shuffle<RoomInfo>(branchEndRoomList);
        int iCount = 0;
        foreach (RoomGameplayBase g in allBranchGames)
        {
            if (iCount >= branchEndRoomList.Count)
            {
                print("ERROR: branchEndRoomList ������I�ƶq����!! �ݭn " + allBranchGames.Count);
                break;
            }
            if (g)
            {
                g.Build(branchEndRoomList[iCount]);
            }
            iCount++;
        }

        //�ѤU���D�u
        foreach (RoomInfo room in normalMainRoomList)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultMainGames);
            if (game != null)
                game.Build(room);
        }
        //�ѤU����u���I
        for (int i = iCount; i < branchEndRoomList.Count; i++)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultBranchGames);
            if (game != null)
                game.Build(branchEndRoomList[i]);
        }
        //�ѤU����u
        foreach (RoomInfo room in normalRoomList)
        {
            RoomGameplayBase game = GetRandomGameplay(defaultBranchGames);
            if (game != null)
                game.Build(room);
        }
        //�Ҧ����u�q�D�v
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
