using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCarverExplore : MissionCarveGameData
{
    //public CarveOne.RoomSequenceInfo expEnd;
    public override void SetupCarveOne(CarveOne carve)
    {
        myCarve = carve;

        //根據任務內容設定 Carve 參數
        myCarve.width = mapSize.x;
        myCarve.height = mapSize.y;
        //每個 Branch 都要多一個端點
        CarveOne.RoomSequenceInfo expEnd = new CarveOne.RoomSequenceInfo { 
            type = CarveOne.RoomSequenceInfo.TYPE.BRANCH_ADD, roomNum = 1,
            roomWidthMin = 16, roomWidthMax = 16,
            roomHeightMin = 16, roomHeightMax = 16,
            corridorLengthMin  = brainchPathInfo.corridorLengthMin, corridorLengthMax = brainchPathInfo.corridorLengthMax,
        };
        CarveOne.RoomSequenceInfo mainEnd = new CarveOne.RoomSequenceInfo
        {
            type = CarveOne.RoomSequenceInfo.TYPE.MAIN_ADD,
            roomNum = 1,
            roomWidthMin = 16,
            roomWidthMax = 16,
            roomHeightMin = 16,
            roomHeightMax = 16,
            corridorLengthMin = brainchPathInfo.corridorLengthMin,
            corridorLengthMax = brainchPathInfo.corridorLengthMax,
        };
        mainPathInfo.type = CarveOne.RoomSequenceInfo.TYPE.MAIN_ADD;
        brainchPathInfo.type = CarveOne.RoomSequenceInfo.TYPE.BRANCH_NEW;
        myCarve.paths = new CarveOne.RoomSequenceInfo[branchCount * 2 + 2];
        myCarve.paths[0] = mainPathInfo;
        myCarve.paths[1] = mainEnd;
        for (int i = 0; i < branchCount; i ++)
        {
            myCarve.paths[i * 2 + 2] = brainchPathInfo;
            myCarve.paths[i * 2 + 3] = expEnd;

        }
        for (int i = 0; i < myCarve.paths.Length; i++)
        {
            myCarve.paths[i].corridorWidth = pathWidth;
        }
        myCarve.initRoomWidth = initRoomSize.x;
        myCarve.initRoomHeight = initRoomSize.y;
    }

    protected override void SetupSpecialGames()
    {
        base.SetupSpecialGames();
    }
}
