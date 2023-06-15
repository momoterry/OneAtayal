using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_MazeCave : MG_MazeDungeon
{
    protected override void InitPuzzleMap()
    {
        if (bigRooms.Length > 0)
        {
            print("ERROR!!!! �ثe MazeCave �L�k�䴩 Big Rooms !!!!�j��M�� !!");
            foreach (BigRoomInfo r in bigRooms)
            {
                r.size = Vector2Int.zero;
            }
        }
        for (int i=0; i<2; i++)
        {
            for (int j=0; j<2; j++)
            {
                puzzleMap[i][j].value = cellInfo.INVALID;
            }
        }
    }
}

