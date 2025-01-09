using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MR_Node;

// ====================================================
//           �ઽ���B�z�������j���԰�
// ====================================================

public class RoomMassiveBattle : RoomGameplayBase
{
    public bool CloseDoolBattle = true;
    [System.Serializable]
    public class EnemyGroupAreaInfo
    {
        public Rect area;                   // �H�����I�y�Ь� (0,0) �ж��j�p�� 10 ���۹�d��ӫ��w
        public EnemyGroupInfo eInfo;
        //public bool spawnWithoutConnect;    //���۳s����
        //public bool diffToSingle;           //���d���פ��v�T�j�צӤ��O�ƶq (Boss �ΩT�w�ƶq�����M��)
        public bool spawnOnStart;             //�@�}�l�N�ͦ�
        //public EnemyGroup.FINISH_TYPE groupFinishType = EnemyGroup.FINISH_TYPE.ON_ENGAGED;
    }

    public EnemyGroupAreaInfo[] areaEnemyInfos;

    //public int RandomBlockNum = 0;
    [System.Serializable]
    public class RandomBlockInfo
    {
        public Rect area;               // �H���շ������d��A�i�H�׶} EnemyArea�A�H�����I�y�Ь� (0,0) �ж��j�p�� 10 ���۹�d��ӫ��w
        public Vector2 blockSizeMin;
        public Vector2 blockSizeMax;
    }
    public RandomBlockInfo[] randomBlocks;
    public int blockAlignment = 1;
    public RandomBlockInfo[] randomHoles;       //�Ъ`�N�A�ثe Block �� Hole ������|�A�_�h�|�X�{ Hole �W��]�����z���𪺲{�H 
    public int holeAlignment = 1;

    public GameObject doorObj;

    [System.Serializable]
    public class ObjectPlaceInfo
    {
        public Vector3 objPosShift;
        public GameObject objRef;
    }
    public ObjectPlaceInfo[] entranceObjects;  // ��m�b�J�f�B������

    protected MazeGameManagerBase.RoomInfo room;
    protected float widthRatio;
    protected float heightRatio;

    public override void Build(MazeGameManagerBase.RoomInfo _room)
    {
        base.Build(_room);

        room = _room;
        widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;


        // == Ĳ�o��  (RoomMassiveBattleController) == 

        GameObject theObj = new GameObject(name + "_" + room.cell.x + "_" + room.cell.y);
        theObj.transform.position = room.vCenter;
        theObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        RoomMassiveBattleController rc = theObj.AddComponent<RoomMassiveBattleController>();
        if (CloseDoolBattle)
            rc.doorObj = doorObj;
        else
            rc.doorObj = null;
        BoxCollider trigCollider = theObj.AddComponent<BoxCollider>();
        trigCollider.size = new Vector3(room.width, room.height, 2.0f);
        trigCollider.isTrigger = true;


        foreach (EnemyGroupAreaInfo ea in areaEnemyInfos)
        {
            Vector3 sPos = new Vector3(ea.area.center.x, 0, ea.area.center.y);
            GameObject o = new GameObject("EnemyMR");
            o.transform.position = room.vCenter + sPos;
            o.transform.rotation = Quaternion.Euler(90, 0, 0);
            MR_EnemyGroup me = o.AddComponent<MR_EnemyGroup>();

            me.eInfo = ea.eInfo;
            me.width = Mathf.RoundToInt(ea.area.width);
            me.height = Mathf.RoundToInt(ea.area.height);
            me.shiftType = MR_Node.POS_SHIFT.ENTER;
            me.rotateWithShiftType = true;
            me.triggerTargetWhenAllKilled = new GameObject[1];
            me.triggerTargetWhenAllKilled[0] = theObj;
            //me.diffToSingle = ea.diffToSingle;
            me.spawnOnStart = ea.spawnOnStart;
            //me.spawnWithoutConnect = ea.spawnWithoutConnect;
            //me.groupFinishType = ea.groupFinishType;

            //me.spawnOnStart = true; //TODO: �u�O����

            o.transform.parent = theObj.transform;
        }

        //�J�f����
        Vector3 posEntrance = room.vCenter + new Vector3(0, 0, MR_Node.ROOM_RELATIVE_SIZE * -0.5f);
        foreach (ObjectPlaceInfo oInfo in entranceObjects)
        {
            GameObject o = BattleSystem.SpawnGameObj(oInfo.objRef, posEntrance + oInfo.objPosShift);
            MR_Node n = o.AddComponent<MR_Node>();
            n.shiftType = MR_Node.POS_SHIFT.ENTER;
            o.transform.parent = theObj.transform;
            //print("O Position: " + o.transform.localPosition);
        }


        rc.Init(room);      //���l���󳣳Ыا��~�i�H���T Init

        foreach (MR_Node node in theObj.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }

    }

    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap)
    {
        base.BuildLayout(room, oMap);

        BuildBlocks(room, oMap, false);
        BuildBlocks(room, oMap, true);

        //float widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        //float heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;
        //float blockBufferWidth = 0.5f;
        //DIRECTION sDir = sDir = room.cell.from;

        //int roomX1 = (room.mapRect.width - (int)room.width) / 2 + room.mapRect.xMin;
        //int roomY1 = (room.mapRect.height - (int)room.height) / 2 + room.mapRect.yMin;

        ////for (int i = 0; i < RandomBlockNum; i++)

        //int alignRatioInt = blockAlignment > 1 ? blockAlignment : 1;
        //float alignDivFloat = 1.0f / (float)alignRatioInt;

        //foreach (RandomBlockInfo ri in randomBlocks)
        //{
        //    float rWidthOriginal = Random.Range(ri.blockSizeMin.x, ri.blockSizeMax.x);
        //    float rHeightOriginal = Random.Range(ri.blockSizeMin.y, ri.blockSizeMax.y);
        //    float rXOriginal = Random.Range(ri.area.xMin + rWidthOriginal * 0.5f, ri.area.xMax - rWidthOriginal * 0.5f);
        //    float rYOriginal = Random.Range(ri.area.yMin + rHeightOriginal * 0.5f, ri.area.yMax - rHeightOriginal * 0.5f);
        //    //Rect blockRect = new Rect(rXMin, rYMin, rWidth, rHeight);
        //    float rWidth = rWidthOriginal;
        //    float rHeight = rHeightOriginal;
        //    float rX = rXOriginal;
        //    float rY = rYOriginal;
        //    switch (sDir)
        //    {
        //        case DIRECTION.U:
        //            rX = -rXOriginal;
        //            rY = -rYOriginal;
        //            break;
        //        case DIRECTION.L:
        //            rWidth = rHeightOriginal;
        //            rHeight = rWidthOriginal;
        //            rX = rYOriginal;
        //            rY = -rXOriginal;
        //            break;
        //        case DIRECTION.R:
        //            rWidth = rHeightOriginal;
        //            rHeight = rWidthOriginal;
        //            rX = -rYOriginal;
        //            rY = rXOriginal;
        //            break;
        //    }

        //    //print("RoomMassiveBattle ���� Block");
        //    int x = roomX1 + Mathf.RoundToInt((rX + 5.0f - rWidth * 0.5f) * widthRatio * alignDivFloat) * alignRatioInt;     //���U�y��
        //    int y = roomY1 + Mathf.RoundToInt((rY + 5.0f - rHeight * 0.5f) * heightRatio * alignDivFloat) * alignRatioInt;    //���U�y��
        //    int w = Mathf.RoundToInt(rWidth * widthRatio * alignDivFloat) * alignRatioInt;
        //    int h = Mathf.RoundToInt(rHeight * heightRatio * alignDivFloat) * alignRatioInt;
        //    //print("To Block :" + new RectInt(x, y, w, h));
        //    //oMap.FillValue(x, y, w, h, (int)MG_MazeOneBase.MAP_TYPE.BLOCK);
        //    oMap.FillValue(x, y, w, h, (int)MG_MazeOneBase.MAP_TYPE.HOLE);

        //    GameObject newObject = new GameObject("RoomMassiveBattle_Block");
        //    newObject.transform.position = new Vector3(x + w * 0.5f, 0, y + h * 0.5f);
        //    BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
        //    boxCollider.size = new Vector3(w - blockBufferWidth * 2, 2.0f, h - blockBufferWidth * 2);
        //    newObject.layer = LayerMask.NameToLayer("Wall");
        //}
    }

    protected void BuildBlocks(MazeGameManagerBase.RoomInfo room, OneMap oMap, bool isHole)
    {
        float widthRatio = room.width / MR_Node.ROOM_RELATIVE_SIZE;
        float heightRatio = room.height / MR_Node.ROOM_RELATIVE_SIZE;
        float blockBufferWidth = 0.5f;
        DIRECTION sDir = sDir = room.cell.from;

        int roomX1 = (room.mapRect.width - (int)room.width) / 2 + room.mapRect.xMin;
        int roomY1 = (room.mapRect.height - (int)room.height) / 2 + room.mapRect.yMin;

        //for (int i = 0; i < RandomBlockNum; i++)

        int alignRatioInt = isHole? (holeAlignment > 1? holeAlignment:1) : (blockAlignment > 1 ? blockAlignment : 1);
        float alignDivFloat = 1.0f / alignRatioInt;

        RandomBlockInfo[] blocks = isHole ? randomHoles : randomBlocks;
        foreach (RandomBlockInfo ri in blocks)
        {
            float rWidthOriginal = Random.Range(ri.blockSizeMin.x, ri.blockSizeMax.x);
            float rHeightOriginal = Random.Range(ri.blockSizeMin.y, ri.blockSizeMax.y);
            float rXOriginal = Random.Range(ri.area.xMin + rWidthOriginal * 0.5f, ri.area.xMax - rWidthOriginal * 0.5f);
            float rYOriginal = Random.Range(ri.area.yMin + rHeightOriginal * 0.5f, ri.area.yMax - rHeightOriginal * 0.5f);
            //Rect blockRect = new Rect(rXMin, rYMin, rWidth, rHeight);
            float rWidth = rWidthOriginal;
            float rHeight = rHeightOriginal;
            float rX = rXOriginal;
            float rY = rYOriginal;
            switch (sDir)
            {
                case DIRECTION.U:
                    rX = -rXOriginal;
                    rY = -rYOriginal;
                    break;
                case DIRECTION.L:
                    rWidth = rHeightOriginal;
                    rHeight = rWidthOriginal;
                    rX = rYOriginal;
                    rY = -rXOriginal;
                    break;
                case DIRECTION.R:
                    rWidth = rHeightOriginal;
                    rHeight = rWidthOriginal;
                    rX = -rYOriginal;
                    rY = rXOriginal;
                    break;
            }

            //print("RoomMassiveBattle ���� Block");
            int x = roomX1 + Mathf.RoundToInt((rX + 5.0f - rWidth * 0.5f) * widthRatio * alignDivFloat) * alignRatioInt;     //���U�y��
            int y = roomY1 + Mathf.RoundToInt((rY + 5.0f - rHeight * 0.5f) * heightRatio * alignDivFloat) * alignRatioInt;    //���U�y��
            int w = Mathf.RoundToInt(rWidth * widthRatio * alignDivFloat) * alignRatioInt;
            int h = Mathf.RoundToInt(rHeight * heightRatio * alignDivFloat) * alignRatioInt;
            //print("To Block :" + new RectInt(x, y, w, h));
            //oMap.FillValue(x, y, w, h, (int)MG_MazeOneBase.MAP_TYPE.BLOCK);
            oMap.FillValue(x, y, w, h, (int)(isHole ? MG_MazeOneBase.MAP_TYPE.HOLE : MG_MazeOneBase.MAP_TYPE.BLOCK));

            if (!isHole)
            {
                GameObject newObject = new GameObject("RoomMassiveBattle_Block");
                newObject.transform.position = new Vector3(x + w * 0.5f, 0, y + h * 0.5f);
                BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(w - blockBufferWidth * 2, 2.0f, h - blockBufferWidth * 2);
                newObject.layer = LayerMask.NameToLayer("Wall");
            }
        }
    }

}



// ====================================================
//        �ઽ���B�z�������j���԰�
// ====================================================

public class RoomMassiveBattleController : MonoBehaviour
{
    public GameObject doorObj;
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
    protected MR_RoomDoorTrigger doorTrigger;
    protected int enemyGroupNum = 0;

    protected int finishEnemyGroupCount = 0;

    public void Init(MazeGameManagerBase.RoomInfo room)
    {
        //trigCollider = gameObject.AddComponent<BoxCollider>();
        //trigCollider.size = new Vector3(room.width, 2.0f, room.height);
        //trigCollider.isTrigger = true;
        trigCollider = gameObject.GetComponent<BoxCollider>();

        // == �� ==
        if (doorObj != null)
        {
            GameObject o = new GameObject(name + "_DOOR");
            doorTrigger = o.AddComponent<MR_RoomDoorTrigger>();
            o.transform.position = transform.position;
            o.transform.parent = transform;
            doorTrigger.doorObj = doorObj;
            doorTrigger.doorShiftBack = 2.0f;
            doorTrigger.fadeOutAnimationTime = 0.5f;
        }
        else
            doorTrigger = null;

        enemyGroupNum = gameObject.GetComponentsInChildren<MR_EnemyGroup>().Length;
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
            //print("�}�l !!");
            foreach (MR_EnemyGroup eg in gameObject.GetComponentsInChildren<MR_EnemyGroup>())
            {
                eg.SendMessage("OnTG", gameObject);
            }
            nextPhase = PHASE.BATTLE;

            if (trigCollider)
                Destroy(trigCollider);

            if (doorTrigger)
                doorTrigger.OnTG(gameObject);
        }
    }

    public void OnTG(GameObject whoTG)
    {
        if (currPhase == PHASE.BATTLE)
        {
            finishEnemyGroupCount++;
            //print("RoomMassiveBattleController:OnTG  -- Count" + finishEnemyGroupCount);
            if (finishEnemyGroupCount >= enemyGroupNum)
            {
                if (doorTrigger != null)
                    doorTrigger.OnTG(gameObject);
                nextPhase = PHASE.FINISH;
            }
        }
    }
}