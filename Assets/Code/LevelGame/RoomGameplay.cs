using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class RoomGameplayBase : MonoBehaviour
{
    public virtual void Build( MazeGameManager.RoomInfo room ) { }
}

public class RoomGameplay : RoomGameplayBase
{
    public GameObject centerGame;

    private void Awake()
    {
        //centerGame.SetActive(false);
    }
    public override void Build( MazeGameManager.RoomInfo room ) 
    {
        GameObject o = BattleSystem.SpawnGameObj(centerGame, room.vCenter);
        o.SetActive(true);
    }

}
