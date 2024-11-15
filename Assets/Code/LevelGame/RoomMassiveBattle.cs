using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============= 能直接處理關門的大型戰鬥

public class RoomMassiveBattle : RoomGameplayBase
{
    [System.Serializable]
    public class EnemyGroupAreaInfo
    {
        public Rect area;               // 以中心點座標為 (0,0) 房間大小為 10 的相對範圍來指定
        public EnemyGroupInfo eInfo;
    }

    public EnemyGroupAreaInfo[] eInfos;


    public override void Build(MazeGameManagerBase.RoomInfo room)
    {
        base.Build(room);

        float widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        float heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;

        GameObject theObj = new GameObject(name + "_" + room.cell.x + "_" + room.cell.y);
        theObj.transform.position = room.vCenter;



        foreach (EnemyGroupAreaInfo ea in eInfos)
        {
            Vector3 sPos = new Vector3(ea.area.x * widthRatio, 0, ea.area.y * heightRatio);
            GameObject o = new GameObject("EnemyMR");
            o.transform.position = room.vCenter + sPos;
            MR_EnemyGroup me = o.AddComponent<MR_EnemyGroup>();

            me.eInfo = ea.eInfo;
            me.width = Mathf.RoundToInt(ea.area.width);
            me.height = Mathf.RoundToInt(ea.area.height);
            me.shiftType = MR_Node.POS_SHIFT.ENTER;

            me.spawnOnStart = true; //TODO: 只是測試

            o.transform.parent = theObj.transform;
        }

        foreach (MR_Node node in theObj.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }

    }
}
