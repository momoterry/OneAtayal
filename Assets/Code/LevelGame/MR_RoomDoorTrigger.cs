using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//用來自動產生房間檔牆的元件
// 第一次 被 OnTG 時會自動產生對應的門元件和移動真實檔牆的 NavmeshOstacle
// 第二次被 OnTG 時則進行檔牆移除的動作

public class MR_RoomDoorTrigger : MR_Node
{
    public GameObject doorObj;
    public float doorShiftBack = 2.0f;
    public float fadeOutAnimationTime = 0;

    protected List<GameObject>  doors = new List<GameObject>();
    protected List<GameObject>  doorObjList = new List<GameObject>();

    public enum DOOR_PHASE
    {
        NONE,
        WAIT,
        BLOCKED,
        FADE_OUT,
        FINISH,
    }
    protected DOOR_PHASE currPhase = DOOR_PHASE.NONE;
    protected DOOR_PHASE nextPhase = DOOR_PHASE.NONE;
    protected float stateTime = 0;

    private void Update()
    {
        if (nextPhase != currPhase)
        {
            switch (nextPhase)
            {
                case DOOR_PHASE.FADE_OUT:
                    PlayFadeOutAnimation();
                    break;
                case DOOR_PHASE.FINISH:
                    gameObject.SetActive(false);
                    break;
            }
            currPhase = nextPhase;
            stateTime = 0;
        }
        stateTime += Time.deltaTime;

        switch (currPhase) 
        {
            case DOOR_PHASE.FADE_OUT:
                if (stateTime > fadeOutAnimationTime) 
                {
                    nextPhase = DOOR_PHASE.FINISH;
                }
                break;
        }

    }

    override public void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        //print("MR_RoomDoorTrigger.OnSetupByRoom");

        if (room.cell.D)
        {
            GameObject dDoor = CreateDoor(room.vCenter + Vector3.back * room.height * 0.5f, room.doorWidth, DIRECTION.D);
            doors.Add(dDoor);
        }
        if (room.cell.U)
        {
            GameObject uDoor = CreateDoor(room.vCenter - Vector3.back * room.height * 0.5f, room.doorWidth, DIRECTION.U);
            doors.Add(uDoor);
        }
        if (room.cell.L)
        {
            GameObject lDoor = CreateDoor(room.vCenter + Vector3.left * room.width * 0.5f, room.doorHeight, DIRECTION.L);
            doors.Add(lDoor);
        }
        if (room.cell.R)
        {
            GameObject rDoor = CreateDoor(room.vCenter + Vector3.right * room.width * 0.5f, room.doorHeight, DIRECTION.R);
            doors.Add(rDoor);
        }
        nextPhase = DOOR_PHASE.WAIT;
    }

    public void OnTG(GameObject whoTG)
    {
        //print("MR_RoomDoorTrigger.OnTG: " + currPhase);

        if (currPhase == DOOR_PHASE.WAIT)
        {

            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].SetActive(true);
                doors[i].SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);
            }
            nextPhase=DOOR_PHASE.BLOCKED;
        }
        else if (currPhase == DOOR_PHASE.BLOCKED)
        {
            //for (int i = 0; i < doors.Count; i++)
            //{
            //    doors[i].SetActive(false);
            //}
            if (fadeOutAnimationTime > 0)
                nextPhase = DOOR_PHASE.FADE_OUT;
            else
                nextPhase = DOOR_PHASE.FINISH;
        }
    }


    protected GameObject CreateDoor(Vector3 vCenter, float doorWidth, DIRECTION dir, float doorObjWidth = 1.0f)
    {
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

        GameObject rootObj = new GameObject("Door_" + dir.ToString());
        rootObj.transform.position = vCenter;
        rootObj.transform.parent = transform;

        for (int i = 0; i < objNum; i++) 
        {
            Vector3 pos = vCenter + objStep * (((objNum-1.0f) * -0.5f) + i);
            if (doorObj != null)
            {
                GameObject o = BattleSystem.SpawnGameObj(doorObj, pos);
                o.transform.parent = rootObj.transform;
                doorObjList.Add(o);
            }
        }

        const float BLOCK_OBJ_DISTANCE = 5.0F;

        GameObject blockObj = new GameObject("Door_Blocker");
        float BlockerMoveDistance = BLOCK_OBJ_DISTANCE;
        blockObj.transform.position = vCenter + doorOutDir * BlockerMoveDistance;
        NavMeshObstacle navO = blockObj.AddComponent<NavMeshObstacle>();
        navO.size = new Vector3(blockWidth, 2.0f, blockHeight);
        blockObj.transform.parent = rootObj.transform;

        MoveTrigger mt = rootObj.AddComponent<MoveTrigger>();
        //rootObj.AddComponent<OrderAdjust>();
        mt.moveDuration = 1.0f;
        mt.moveTarget = blockObj;
        mt.moveVector = -doorOutDir * BLOCK_OBJ_DISTANCE;

        rootObj.SetActive(false);
        return rootObj;
    }

    protected void PlayFadeOutAnimation()
    {
        foreach (GameObject o in doorObjList)
        {
            SPAnimator anim = o.GetComponent<SPAnimator>();
            if (anim != null)
                anim.PlaySpecific("FADE_OUT");
        }
    }

}
