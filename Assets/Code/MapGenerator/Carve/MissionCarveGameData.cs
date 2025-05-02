using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCarveGameData : MonoBehaviour
{
    //�o�ǬO�򥻦@�P��T
    public Vector2Int mapSize = new Vector2Int(120, 160);
    public Vector2Int initRoomSize = new Vector2Int(10, 12);
    public int pathWidth = 6;   //�|��g���᭱�� RoomSequence


    //�o�ǬO�򥻸��|��T
    public CarveOne.RoomSequence mainPathInfo;
    public CarveOne.RoomSequence brainchPathInfo;
    public int branchCount = 2;

    public RoomGameplayBase[] defaultRoomGameplay;
    public RoomGameplayBase[] corridorGameplay;
}
