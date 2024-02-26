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
    }
    public POS_SHIFT shiftType = POS_SHIFT.SIZE_ONLY;

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

        if (shiftType == POS_SHIFT.ENTER)
        {
            switch (room.cell.from)
            {
                case DIRECTION.D:
                    transform.localPosition = new Vector3(x, y, z);
                    break;
                case DIRECTION.R:
                    transform.localPosition = new Vector3(-y, x, z);
                    break;
                case DIRECTION.U:
                    transform.localPosition = new Vector3(-x, -y, z);
                    break;
                case DIRECTION.L:
                    transform.localPosition = new Vector3(y, x, z);
                    break;
            }
        }
    }
}
