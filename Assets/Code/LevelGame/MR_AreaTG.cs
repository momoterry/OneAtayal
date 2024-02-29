using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_AreaTG : MR_Node
{
    public GameObject[] TriggerTargets;
    public float Width = ROOM_RELATIVE_SIZE;
    public float Height = ROOM_RELATIVE_SIZE;
    public bool triggerOnce = true;
    private bool isTriggered = false;

    protected BoxCollider col = null;

    public override void OnSetupByRoom(MazeGameManager.RoomInfo room)
    {
        base.OnSetupByRoom(room);

        DIRECTION dir = DIRECTION.D;
        float colX = Width;
        float colY = Height;
        if (shiftType != POS_SHIFT.NONE)
        {
            if (shiftType == POS_SHIFT.ENTER)
                dir = room.cell.from;
            else if (shiftType == POS_SHIFT.LEAVE)
                dir = room.cell.to;

            if ((dir == DIRECTION.L || dir == DIRECTION.R) && rotateWithShiftType)
            {
                //長寬縮放倍率交換
                colX *= heightRatio;
                colY *= widthRatio;
            }
            else
            {
                colX *= widthRatio;
                colY *= heightRatio;
            }
        }

        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.size = new Vector3 (colX, colY, colY);
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTriggered == false)
        {
            //print("Player In !!");
            foreach (GameObject o in TriggerTargets)
            {
                if (o)
                    o.SendMessage("OnTG", other.gameObject);
            }
            if (triggerOnce)
            {
                isTriggered = true;
                enabled = false;
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(transform.position, new Vector3(Width, 2.0f, Height));
    //}
}
