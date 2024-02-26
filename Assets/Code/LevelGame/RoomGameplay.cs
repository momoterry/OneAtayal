using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class RoomGameplayBase : MonoBehaviour
{
    public virtual void Build( MazeGameManager.RoomInfo room ) { }
}

public class MR_NodeBase : MonoBehaviour
{
    public const float ROOM_RELATIVE_SIZE = 10.0f;     //�Y�񵥪����
    virtual public void OnSetupByRoom(MazeGameManager.RoomInfo room)
    {
        //TODO: Local ��m�ե�
    }
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

        foreach (MR_NodeBase node in o.GetComponentsInChildren<MR_NodeBase>())
        {
            node.OnSetupByRoom(room);
        }
    }

}
