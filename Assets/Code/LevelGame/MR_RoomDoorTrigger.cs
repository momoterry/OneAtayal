using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MR_RoomDoorTrigger : MR_Node
{
    public GameObject doorObj;
    override public void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        print("MR_RoomDoorTrigger.OnSetupByRoom");

        CreateDoor(room.vCenter + Vector3.back * room.height * 0.5f, room.doorWidth, DIRECTION.D);
        CreateDoor(room.vCenter - Vector3.back * room.height * 0.5f, room.doorWidth, DIRECTION.U);
        CreateDoor(room.vCenter + Vector3.left * room.width * 0.5f, room.doorHeight, DIRECTION.L);
        CreateDoor(room.vCenter + Vector3.right * room.width * 0.5f, room.doorHeight, DIRECTION.R);
    }

    public void OnTG(GameObject whoTG)
    {
        print("MR_RoomDoorTrigger.OnTG");
    }


    protected void CreateDoor(Vector3 vCenter, float doorWidth, DIRECTION dir, float doorObjWidth = 1.0f)
    {
        float doorShiftBack = 1.0f;
        int objNum = Mathf.RoundToInt(doorWidth/ doorObjWidth);
        Vector3 objStep = Vector3.zero;
        Vector3 doorOutDir = Vector3.zero;
        float blockWidth = 1.0f;
        float blockHeight = 1.0f;
        switch (dir)
        {
            case DIRECTION.D:
                //objStep = new Vector3(doorObjWidth, 0, 0);
                doorOutDir = Vector3.back;
                break;
            case DIRECTION.U:
                //objStep = new Vector3(doorObjWidth, 0, 0);
                doorOutDir = Vector3.forward;
                break;
            case DIRECTION.L:
                //objStep = new Vector3(0, 0, doorObjWidth);
                doorOutDir = Vector3.left;
                break;
            case DIRECTION.R:
                doorOutDir = Vector3.right;
                break;
        }
        vCenter += doorOutDir * doorShiftBack;

        switch (dir)
        {
            case DIRECTION.D:
            case DIRECTION.U:
                objStep = new Vector3(doorObjWidth, 0, 0);
                blockWidth = doorWidth + 2.0f;  //加入安全值
                break;
            case DIRECTION.L:
            case DIRECTION.R:
                objStep = new Vector3(0, 0, doorObjWidth);
                blockHeight = doorWidth + 2.0f; //加入安全值
                break;
        }

        for (int i = 0; i < objNum; i++) 
        {
            Vector3 pos = vCenter + objStep * (((objNum-1.0f) * -0.5f) + i);
            if (doorObj != null)
            {
                GameObject o = BattleSystem.SpawnGameObj(doorObj, pos);
                o.transform.parent = transform;
            }
        }

        GameObject dObj = new GameObject("Door_Blocker");
        float BlockerMoveDistance = 5.0f;
        dObj.transform.position = vCenter + doorOutDir * BlockerMoveDistance;
        NavMeshObstacle navO = dObj.AddComponent<NavMeshObstacle>();
        navO.size = new Vector3(blockWidth, 2.0f, blockHeight);
        dObj.transform.parent = transform;
    }

}
