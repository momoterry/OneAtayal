using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_Node : MonoBehaviour
{
    public enum POS_SHIFT
    {
        NONE,
        SIZE_ONLY,
        ENTER,
        LEAVE,
    }
    public POS_SHIFT shiftType = POS_SHIFT.SIZE_ONLY;
    public bool rotateWithShiftType = false;

    public const float ROOM_RELATIVE_SIZE = 10.0f;     //縮放等的基準
    protected float widthRatio = 1;
    protected float heightRatio = 1;
    virtual public void OnSetupByRoom(MazeGameManager.RoomInfo room)
    {
        //TODO: Local 位置校正
        widthRatio = room.width / ROOM_RELATIVE_SIZE;
        heightRatio = room.height / ROOM_RELATIVE_SIZE;

        if (shiftType != POS_SHIFT.NONE)
            transform.localPosition = new Vector3(transform.localPosition.x * widthRatio, transform.localPosition.y * heightRatio, transform.localPosition.z);

        float x = transform.localPosition.x;
        float y = transform.localPosition.y;
        float z = transform.localPosition.z;

        DIRECTION sDir = DIRECTION.NONE;
        if (shiftType == POS_SHIFT.ENTER)
            sDir = room.cell.from;
        else if (shiftType == POS_SHIFT.LEAVE)
            sDir = OneUtility.GetReverseDIR( room.cell.to );

        //if (this is MR_Node)
        //{
        //    print("room.cell.from : " + room.cell.from);
        //    print("room.cell.to : " + room.cell.to);
        //}

        float angle = 0;
        //以房間面向下方為基準調整位置 (入口在下、出口在上)
        switch (sDir)
        {
            case DIRECTION.D:
                transform.localPosition = new Vector3(x, y, z);
                angle = 0;
                break;
            case DIRECTION.R:
                transform.localPosition = new Vector3(-y, x, z);
                angle = 90;
                break;
            case DIRECTION.U:
                transform.localPosition = new Vector3(-x, -y, z);
                angle = 180;
                break;
            case DIRECTION.L:
                transform.localPosition = new Vector3(y, x, z);
                angle = -90;
                break;
        }
        if (rotateWithShiftType)
            transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
