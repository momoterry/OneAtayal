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

    //public int RandomBlockNum = 0;
    [System.Serializable]
    public class RandomBlockInfo
    {
        public Rect area;               // 隨機組當的分布範圍，可以避開 EnemyArea，以中心點座標為 (0,0) 房間大小為 10 的相對範圍來指定
        public Vector2 blockSizeMin;
        public Vector2 blockSizeMax;
    }
    public RandomBlockInfo[] randomBlocks;

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
            Vector3 sPos = new Vector3(ea.area.center.x * widthRatio, 0, ea.area.center.y * heightRatio);
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

        int roomX1 = (room.mapRect.width - (int)room.width) / 2 + room.mapRect.xMin;
        int roomY1 = (room.mapRect.height - (int)room.height) / 2 + room.mapRect.yMin;

        //for (int i = 0; i < RandomBlockNum; i++)
        foreach (RandomBlockInfo ri in randomBlocks)
        {
            float rWidth = Random.Range(ri.blockSizeMin.x, ri.blockSizeMax.x);
            float rHeight = Random.Range(ri.blockSizeMin.y, ri.blockSizeMax.y);
            float rXMin = Random.Range(ri.area.xMin, ri.area.xMax - rWidth);
            float rYMin = Random.Range(ri.area.yMin, ri.area.yMax - rHeight);
            //Rect blockRect = new Rect(rXMin, rYMin, rWidth, rHeight);

            print("RoomMassiveBattle 產生 Block");
            int x = roomX1 + Mathf.RoundToInt((rXMin + 5.0f) * widthRatio);     //左下座標
            int y = roomY1 + Mathf.RoundToInt((rYMin + 5.0f) * heightRatio);    //左下座標
            int w = Mathf.RoundToInt(rWidth * widthRatio);
            int h = Mathf.RoundToInt(rHeight * heightRatio);
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