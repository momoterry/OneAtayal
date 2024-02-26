using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


//
public class RoomGameplayBase : MonoBehaviour
{
    public virtual void Build( MazeGameManager.RoomInfo room ) { }
}

//public class MR_NodeBase : MonoBehaviour
//{
//    public const float ROOM_RELATIVE_SIZE = 10.0f;     //縮放等的基準
//    protected float widthRatio = 1;
//    protected float heightRatio = 1;
//    virtual public void OnSetupByRoom(MazeGameManager.RoomInfo room)
//    {
//        //TODO: Local 位置校正
//        widthRatio = room.width / ROOM_RELATIVE_SIZE;
//        heightRatio = room.height / ROOM_RELATIVE_SIZE;
//    }
//}

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

        foreach (MR_Node node in o.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }
    }

}
