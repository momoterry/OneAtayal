using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ====================================================
//           能直接處理關門的大型戰鬥
// ====================================================

public class RoomMassiveBattle : RoomGameplayBase
{
    [System.Serializable]
    public class EnemyGroupAreaInfo
    {
        public Rect area;               // 以中心點座標為 (0,0) 房間大小為 10 的相對範圍來指定
        public EnemyGroupInfo eInfo;
    }

    public EnemyGroupAreaInfo[] eInfos;

    public int RandomBlockNum = 0;
    public Rect RandomBlockArea;        // 隨機組當的分布範圍，可以避開 EnemyArea
    public Vector2 blockSizeMin;
    public Vector2 blockSizeMax;

    public override void Build(MazeGameManagerBase.RoomInfo room)
    {
        base.Build(room);

        float widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        float heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;

        GameObject theObj = new GameObject(name + "_" + room.cell.x + "_" + room.cell.y);
        theObj.transform.position = room.vCenter;
        RoomMassiveBattleController rc = theObj.AddComponent<RoomMassiveBattleController>();
        rc.Init(room);

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

            //me.spawnOnStart = true; //TODO: 只是測試

            o.transform.parent = theObj.transform;
        }

        foreach (MR_Node node in theObj.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }

    }

    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap)
    {
        base.BuildLayout(room, oMap);

        float widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        float heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;
        float blockBufferWidth = 0.5f;

        int roomX1 = (room.mapRect.width - (int)room.width) / 2 + room.mapRect.x;
        int roomY1 = (room.mapRect.height - (int)room.height) / 2 + room.mapRect.y;

        for (int i = 0; i < RandomBlockNum; i++)
        {
            float rWidth = Random.Range(blockSizeMin.x, blockSizeMax.x);
            float rHeight = Random.Range(blockSizeMin.y, blockSizeMax.y);
            float rXMin = Random.Range(RandomBlockArea.xMin, RandomBlockArea.xMax - rWidth);
            float rYMin = Random.Range(RandomBlockArea.yMin, RandomBlockArea.yMax - rHeight);
            //Rect blockRect = new Rect(rXMin, rYMin, rWidth, rHeight);

            print("RoomMassiveBattle 產生 Block");
            int x = roomX1 + Mathf.RoundToInt((rXMin + 5.0f) * 0.1f * room.width);
            int y = roomY1 + Mathf.RoundToInt((rYMin + 5.0f) * 0.1f * room.height);
            int w = Mathf.RoundToInt(rWidth * 0.1f * room.width);
            int h = Mathf.RoundToInt(rHeight * 0.1f * room.height);
            //print("To Block :" + new RectInt(x, y, w, h));
            oMap.FillValue(x, y, w, h, (int)MG_MazeOneBase.MAP_TYPE.BLOCK);

            GameObject newObject = new GameObject("RoomMassiveBattle_Block");
            newObject.transform.position = new Vector3(x + w * 0.5f, 0, y + h * 0.5f);
            BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(w - blockBufferWidth * 2, 2.0f, h - blockBufferWidth * 2);
            newObject.layer = LayerMask.NameToLayer("Wall");
            //newObject.transform.parent = transform;
        }
    }

}


// ====================================================
//        能直接處理關門的大型戰鬥
// ====================================================

public class RoomMassiveBattleController : MonoBehaviour
{
    public enum PHASE
    {
        NONE,
        WAIT,
        BATTLE,
        FINISH,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;

    protected BoxCollider trigCollider;

    public void Init(MazeGameManagerBase.RoomInfo room)
    {
        trigCollider = gameObject.AddComponent<BoxCollider>();
        trigCollider.size = new Vector3(room.width, 2.0f, room.height);
        trigCollider.isTrigger = true;
    }

    void Start()
    {
        nextPhase = PHASE.WAIT;
    }

    void Update()
    {
        currPhase = nextPhase;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && currPhase == PHASE.WAIT)
        {
            //print("開始 !!");
            foreach (MR_EnemyGroup eg in gameObject.GetComponentsInChildren<MR_EnemyGroup>())
            {
                eg.SendMessage("OnTG", gameObject);
            }
            nextPhase = PHASE.BATTLE;

            if (trigCollider)
                Destroy(trigCollider);
        }
    }
}