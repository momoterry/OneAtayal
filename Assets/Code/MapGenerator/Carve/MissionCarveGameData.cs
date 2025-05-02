using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCarveGameData : MonoBehaviour
{
    //這些是基本共同資訊
    public Vector2Int mapSize = new Vector2Int(120, 160);
    public Vector2Int initRoomSize = new Vector2Int(10, 12);
    public int pathWidth = 6;   //會改寫掉後面的 RoomSequence


    //這些是基本路徑資訊
    public CarveOne.RoomSequence mainPathInfo;
    public CarveOne.RoomSequence brainchPathInfo;
    public int branchCount = 2;

    public RoomGameplayBase[] defaultRoomGameplay;
    public RoomGameplayBase[] corridorGameplay;
}
