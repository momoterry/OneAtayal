using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class MapSaveMazeOne : MapSaveDataBase
{
    public int puzzleWidth = 6;
    public int puzzleHeight = 6;

    public int roomWidth = 12;
    public int roomHeight = 16;
    public int wallWidth = 4;
    public int wallHeight = 4;

    public int pathWidth = 4;
    public int pathHeight = 4;

    //public bool extendTerminal = true;
    public bool portalAfterFirstRoomGamplay = false;

    public Vector2Int puzzleStart;
    public Vector2Int puzzleEnd;
    //public Vector3 startPos;
    //public Vector3 endPos;
    public string puzzleMapData = null;
    //public string mapMask64 = null;             //TODO: ��� MapSaveDataBase ?

}


//�C�@�� Cell ���ABuild Gameplay �һݭn���򥻤��e
public class CELL_BASE
{
    public int x, y;
    public bool U, D, L, R;
    public DIRECTION from;  //���_�I�̪񪺤�V
    public DIRECTION to;    //�����I����V�A�p�G�O����A�w�]���_�I���۹��V

    //public int deep;    //�Z���X�o�I���`�סA�̤p�Ȭ� 1�A0 ��ܥ��B�z
    //public bool isMain; //�O�_�D�F�D
    //public int mainDeep; //�D�F�D�W���`��
    //public bool isPath;
}


public class MG_MazeOneBase : MapGeneratorBase
{
    //public string mapName;              //�Ψ��ѧO�s��
                                        //�a�Ϧs�ɸ��
    protected MapSaveMazeOne loadedMapData = null;  //�p�G�����ܡA�O���J�s�ɪ��Φ�

    public int puzzleWidth = 6;
    public int puzzleHeight = 6;

    public int roomWidth = 12;
    public int roomHeight = 16;
    public int wallWidth = 4;
    public int wallHeight = 4;

    public int pathWidth = 4;
    public int pathHeight = 4;

    protected int cellWidth = 4;
    protected int cellHeight = 4;

    public float pathRate = 0;
    public bool startAsPath = false;
    public bool finishAsPath = false;

    public enum MAZE_DIR
    {
        DONW_TO_TOP,
        TOP_TO_DOWN,
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT,

        INSIDE_OUT,     //�q�������~���A�O�d�� Cave ���g�c��
    }
    public MAZE_DIR mazeDir = MAZE_DIR.DONW_TO_TOP;
    public bool FinishAtDeepest = false;       //�����J�f�̻����ж��@�����I

    public GameObject finishPortalRef;
    public bool portalAfterFirstRoomGamplay = false;
    public bool createWallCollider = true;
    protected const float wallBuffer = 0.25f;

    // Tile ��Ƭ���
    public TileGroupDataBase groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public TileEdgeGroupDataBase groundOutEdgeTileGroup;
    public TileGroupDataBase blockTileGroup;                //�Φb�D��ɪ� Block �ϰ�
    public TileEdgeGroupDataBase blockTileEdgeGroup;
    public TileGroupDataBase defautTileGroup;               //�Φb�a�Ϫ��~���
    public TileGroupDataBase roomGroundTileGroup;
    public TileEdgeGroupDataBase roomGroundTileEdgeGroup;
    public bool showMainPath = false;
    public int mainPathBuff = 1;    //�D���|���ܸ���|�e���w�ĶZ��
    public TileGroupDataBase mainPathTileGroup;       //�ΨӴ��ܥD���|
    public TileEdgeGroupDataBase mainPathTileEdgeGroup;
    public Tilemap groundTM;
    public Tilemap blockTM;


    //Gameplay ��
    //[Header("Gameplay �]�w")]
    public DungeonEnemyManagerBase dungeonEnemyManager;        //TODO: �ثe�٨S�Ψ�
    public MazeGameManagerBase gameManager;

    protected float dungeonEnemyDifficulty = 1.0f;

    public GameObject initGampleyRef;

    //�˹�����
    public MapDecadeGenerator decadeGenerator;

    protected MAP_TYPE MapInitValue = MAP_TYPE.BLOCK;   //�w�]�|�Q�񺡪���
    //�򩳦a�Ϭ��� TODO: �Ʊ�W�ߥX�h
    protected int mapWidth = 0;
    protected int mapHeight = 0;
    protected int borderWidth = 4;
    protected Vector3Int mapCenter;
    protected OneMap theMap = new OneMap();
    public enum MAP_TYPE
    {
        DEFAULT = OneMap.DEFAULT_VALUE,
        GROUND = 4,
        ROOM = 5,
        BLOCK = 6,
        PATH = 8,
    }

    protected int puzzleX1;
    protected int puzzleY1;

    //protected int iStart;
    //protected int iEnd;
    protected Vector3 startPos;
    protected Vector3 endPos;

    public class CELL : CELL_BASE
    {
        //public bool U, D, L, R;
        public int deep;    //�Z���X�o�I���`�סA�̤p�Ȭ� 1�A0 ��ܥ��B�z
        public bool isMain; //�O�_�D�F�D
        public int mainDeep; //�D�F�D�W���`��
        //public DIRECTION from;  //���_�I�̪񪺤�V
        public bool isPath;

        public int value = NORMAL;
        public const int NORMAL = 0;
        public const int ROOM = 1;
        public const int TERNIMAL = 2;  //�X�B�J�f
        public const int INVALID = 7;

        public int Encode()
        {
            int iDoor = 0;
            iDoor += U ? 8 : 0;
            iDoor += D ? 4 : 0;
            iDoor += L ? 2 : 0;
            iDoor += R ? 1 : 0;
            int iAll = value * 16 + iDoor;
            iAll += isPath ? 128 : 0;
            if (iAll > 255)
            {
                One.ERROR("CELL.Encode > 255!!");
            }
            return iAll;
        }

        public void Decode(int code)
        {
            //print(code);
            R = (code % 2) == 1 ? true : false;
            code = code >> 1;
            L = (code % 2) == 1 ? true : false;
            code = code >> 1;
            D = (code % 2) == 1 ? true : false;
            code = code >> 1;
            U = (code % 2) == 1 ? true : false;
            code = code >> 1;

            if (code >= 8)
            {
                isPath = true;
                code -= 8;
            }
            else
                isPath = false;

            value = code;
        }
    }

    protected CELL[][] puzzleMap;

    // ===================== �ж��s������

    protected void ConnectCells( CELL cFrom, CELL cTo, DIRECTION toDir)
    {
        switch (toDir)
        {
            case DIRECTION.U:
                cFrom.U = cTo.D = true;
                break;
            case DIRECTION.D:
                cFrom.D = cTo.U = true;
                break;
            case DIRECTION.L:
                cFrom.L = cTo.R = true;
                break;
            case DIRECTION.R:
                cFrom.R = cTo.L = true;
                break;
        }
    }

    protected void ConnectCellsByID(int id_1, int id_2)
    {
        CELL cell_1 = puzzleMap[GetCellX(id_1)][GetCellY(id_1)];
        CELL cell_2 = puzzleMap[GetCellX(id_2)][GetCellY(id_2)];
        if (id_1 + puzzleWidth == id_2) //�U�s��W�A���ˬd�U�s��W�����p�A�H�K���� puzzleWidth == 1 �����p
        {
            cell_1.U = true;
            cell_2.D = true;
        }
        else if (id_1 + 1 == id_2) //���s��k
        {
            cell_1.R = true;
            cell_2.L = true;
        }
    }

    protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    protected int GetCellX(int id) { return id % puzzleWidth; }
    protected int GetCellY(int id) { return id / puzzleWidth; }


    public override void BuildAll(int buildLevel = 1)
    {

        PresetByContinuousBattle();

        if (mapName != null && mapName != "")
            LoadMap();  //�����ո��J�s�ɡA�����ܧ�s�a�ϰѼƵ�

        PresetMapInfo();    //�U�ؤ����Ѽƪ���l�ơA�]�t�w�]�_�I���I�A�����bŪ���i�שγs��԰���T���T�{�Ѽƫ�i��

        InitPuzzleMap();    //��l�� OneMap �M CellMap �����e���w�]��


        //============================= �H�U�]�m�C�� Cell �� Layout ���e ===========================================
        if (loadedMapData != null)
        {
            LoadMazeMap();
        }
        else
        {
            CreatMazeMap();
            CalculateRoomPath();
        }

        //=========================== �]�w�_�I�M���I�� Gameplay �A�åB��]�w�D���X���I
        ProcessInitFinish();

        //=========================== �U�� Cell �� Gameplay �θ�T���p��
        PreCalculateGameplayInfo();

        //=========================== �� Cell �����e��� OneMap ��
        ProcessNormalCells();

        //BattleSystem.GetInstance().SetInitPosition(startPos);

        //============================= �H�U�}�l�E�] Tiles ===========================================
        FillAllTiles();

        GenerateNavMesh(theSurface2D);

        //====================== �˹�����إ� =====================================
        //MapDecadeGeneratorBase dGen = GetComponent<MapDecadeGeneratorBase>();
        if (decadeGenerator)
        {
            DecadeGenerateParameter p = new DecadeGenerateParameter();
            p.mapValue = (int)MAP_TYPE.BLOCK;
            decadeGenerator.BuildAll(theMap, p);
        }

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMiniMap(theMap);
        }

        //============== �b�p�a�ϲ��ͧ�����A�}�l�i�� Gameplay ���󪺥ͦ� (����ܦb�p�a�ϤW������)
        BuildGameplay();

        //���J�w��������T
        //LoadExploreMap();

        //�a�Ϧs��
        //SaveMap();
    }

    virtual protected void PresetByContinuousBattle()
    {
        ContinuousBattleDataBase cBase = ContinuousBattleManager.GetCurrBattleData();
        if (cBase != null)
        {
            if (cBase is ContinuousMOData)
            {
                ContinuousMOData cData = (ContinuousMOData)cBase;
                puzzleWidth = cData.puzzleWidth;
                puzzleHeight = cData.puzzleHeight;
                print("�ھڸ�ƭץ��F�g�c�j�p: " + puzzleWidth + " - " + puzzleHeight);
                pathRate = cData.pathRate;
                roomWidth = cData.roomWidth;
                roomHeight = cData.roomHeight;
                pathWidth = cData.pathWidth;
                pathHeight = cData.pathHeight;

                mazeDir = cData.mazeDir;
                FinishAtDeepest = cData.finishAtDeepest;

                if (cData.gameManagerRef)
                {
                    GameObject o = Instantiate(cData.gameManagerRef.gameObject);
                    gameManager = o.GetComponent<MazeGameManagerBase>();
                }
                
                if (gameManager && cData.gameManagerData!=null)
                {
                    gameManager.Init(cData.gameManagerData);
                }

                if (cData.initGameplayRef)
                {
                    initGampleyRef = cData.initGameplayRef;
                }

                if (cData.levelID != null)
                {
                    BattleSystem.GetInstance().levelID = cData.levelID;
                }
            }
            else
            {
                One.LOG("ERROR!! ContinuousBattle ���~�A�U�����d��Ƥ��O ContinuousMazeData !!");
            }
        }
    }

    virtual protected void PresetMapInfo()
    {
        // =============== �U�ؤ����ѼƳ]�w
        cellWidth = roomWidth + wallWidth + wallWidth;
        cellHeight = roomHeight + wallHeight + wallHeight;
        pathWidth = pathWidth > roomWidth ? roomWidth : pathWidth;
        pathHeight = pathHeight > roomHeight ? roomHeight : pathHeight;
        if ((pathWidth) % 2 != (roomWidth % 2))
            pathWidth++;
        if ((pathHeight) % 2 != (roomHeight % 2))
            pathHeight++;

        mapHeight = puzzleHeight * cellHeight;
        mapWidth = puzzleWidth * cellWidth;

        //mapCenter.y = puzzleHeight * cellHeight / 2 - (cellHeight / 2);
        if (puzzleHeight % 2 == 0)
        {
            mapCenter.y = cellHeight / 2;
        }

        if (puzzleWidth % 2 == 0)
        {
            mapCenter.x = cellWidth / 2;
        }

        puzzleX1 = mapCenter.x - (puzzleWidth * cellWidth / 2);
        puzzleY1 = mapCenter.y - (puzzleHeight * cellHeight / 2);

        if (gameManager)
            gameManager.SetDefaultRoomLayout(roomWidth, roomHeight, pathWidth, pathHeight, wallWidth, wallHeight);
    }

    virtual protected void InitPuzzleMap()
    {
        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth, (int)MapInitValue);

        // =============== ��l�� PuzzleMap
        puzzleMap = new CELL[puzzleWidth][];
        for (int i = 0; i < puzzleWidth; i++)
        {
            puzzleMap[i] = new CELL[puzzleHeight];
            for (int j = 0; j < puzzleHeight; j++)
            {
                puzzleMap[i][j] = new CELL();
                puzzleMap[i][j].x = i;
                puzzleMap[i][j].y = j;
            }
        }

        puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
        puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);
    }

    virtual protected void CreatMazeMap()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                puzzleMap[i][j].value = CELL.NORMAL;
            }
        }
    }

    virtual protected void CalculateRoomPath()
    {
        puzzleMap[puzzleStart.x][puzzleStart.y].value = CELL.TERNIMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.TERNIMAL;
        if (startAsPath)
        {
            puzzleMap[puzzleStart.x][puzzleStart.y].isPath = true;
        }
        if (finishAsPath)
        {
            puzzleMap[puzzleEnd.x][puzzleEnd.y].isPath = true;
        }

        if (pathRate > 0)
        {
            List<CELL> allCells = new List<CELL>();
            for (int x = 0; x < puzzleWidth; x++)
            {
                for (int y = 0; y < puzzleHeight; y++)
                {
                    if (puzzleMap[x][y].value == CELL.NORMAL)
                    {
                        allCells.Add(puzzleMap[x][y]);
                    }
                }
            }
            OneUtility.Shuffle<CELL>(allCells);
            int pathNum = Mathf.RoundToInt(pathRate * allCells.Count);
            for (int i = 0; i < pathNum; i++)
            {
                allCells[i].isPath = true;
            }
        }
    }

    protected void FillBlock(float x1, float y1, float width, float height)
    {
        if (!createWallCollider)
            return;
        GameObject newObject = new GameObject("MyBoxObj");
        newObject.transform.position = new Vector3(x1 + width * 0.5f, 0, y1 + height * 0.5f);
        BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, 2.0f, height);
        //boxCollider.l
        newObject.transform.parent = colliderRoot.transform;//gameObject.transform;
        newObject.isStatic = true;
        newObject.layer = LayerMask.NameToLayer("Wall");
    }

    protected void FillCell(CELL cell, int x1, int y1, int width, int height)
    {
        //����
        int wallWidth = this.wallWidth;
        int wallHeight = this.wallHeight;
        int roomWidth = this.roomWidth;
        int roomHeight = this.roomHeight;
        //bool isPath = Random.Range(0, 1.0f) < pathRate;
        //if (cell.value == CELL.TERNIMAL)
        //    isPath = true;
        if (cell.isPath)
        {
            wallWidth = (roomWidth - pathWidth) / 2 + wallWidth;
            wallHeight = (roomHeight - pathHeight) / 2 + wallHeight;
            roomWidth = pathWidth;
            roomHeight = pathHeight;
        }

        int x2 = x1 + width - wallWidth;
        int y2 = y1 + height - wallHeight;

        int wallGapWidth = (roomWidth - pathWidth) / 2;
        int wallGapHeight = (roomHeight - pathHeight) / 2;


        if (cell.value == CELL.INVALID)
        {
            theMap.FillValue(x1, y1, width, height, (int)MAP_TYPE.BLOCK);
            return;
        }
        theMap.FillValue(x1, y1, width, height, (int)MAP_TYPE.GROUND);

        theMap.FillValue(x1, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x1, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);

        bool isFillBlock = createWallCollider && (cell.value != CELL.ROOM);

        if (!cell.D)
        {
            theMap.FillValue(x1 + wallWidth, y1, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1, y1, width - wallWidth + wallBuffer, wallHeight - wallBuffer);   //���U�M�U
        }
        else
        {
            if (wallGapWidth > 0)
            {
                theMap.FillValue(x1 + wallWidth, y1, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathWidth, y1, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x1, y1, wallWidth + wallGapWidth - wallBuffer, wallHeight - wallBuffer);   // ���U
                if (wallGapWidth > 0)
                {
                    FillBlock(x1 + wallWidth + wallGapWidth + pathWidth + wallBuffer, y1, wallGapWidth, wallHeight - wallBuffer);   // ���U
                }
            }
        }
        if (!cell.U)
        {
            theMap.FillValue(x1 + wallWidth, y2, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1 + wallWidth - wallBuffer, y2 + wallBuffer, width - wallWidth + wallBuffer, wallHeight - wallBuffer);   //�k�W�M�W

        }
        else
        {
            if (wallGapWidth > 0)
            {
                theMap.FillValue(x1 + wallWidth, y2, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathWidth, y2, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x2 - wallGapWidth + wallBuffer, y2 + wallBuffer, wallWidth + wallGapWidth - wallBuffer, wallHeight - wallBuffer);   // �k�W
                if (wallGapWidth > 0)
                {
                    FillBlock(x1 + wallWidth - wallBuffer, y2 + wallBuffer, wallGapWidth, wallHeight - wallBuffer);   // �k�W
                }
            }
        }

        if (!cell.L)
        {
            theMap.FillValue(x1, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1, y1 + wallHeight - wallBuffer, wallWidth - wallBuffer, height - wallHeight + wallBuffer); //���W�M��
        }
        else
        {
            if (wallGapHeight > 0)
            {
                theMap.FillValue(x1, y1 + wallHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1, y1 + wallHeight + wallGapHeight + pathHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x1, y2 + wallBuffer - wallGapHeight, wallWidth - wallBuffer, wallHeight + wallGapHeight - wallBuffer);    //���W
                if (wallGapHeight > 0)
                {
                    FillBlock(x1, y1 + wallHeight - wallBuffer, wallWidth - wallBuffer, wallGapHeight);    //���W
                }
            }
        }
        if (!cell.R)
        {
            theMap.FillValue(x2, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, height - wallHeight + wallBuffer);  //�k�U�M�k
        }
        else
        {
            if (wallGapHeight > 0)
            {
                theMap.FillValue(x2, y1 + wallHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x2, y1 + wallHeight + wallGapHeight + pathHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, wallHeight + wallGapHeight - wallBuffer);   //�k�U
                if (wallGapHeight > 0)
                {
                    FillBlock(x2 + wallBuffer, y1 + wallHeight + wallGapHeight + pathHeight + wallBuffer, wallWidth - wallBuffer, wallGapHeight);   //�k�U
                }
            }
        }

        //Path
        if (cell.isMain && showMainPath)
        {
            int pathBuffer = mainPathBuff;
            int path2 = mainPathBuff + mainPathBuff;
            if (cell.from == DIRECTION.L || cell.to == DIRECTION.L)
                theMap.FillValue(x1, y1 + wallHeight + wallGapHeight + pathBuffer, 
                    wallWidth + wallGapWidth + pathWidth - pathBuffer, pathHeight - path2, (int)MAP_TYPE.PATH);
            if (cell.from == DIRECTION.R || cell.to == DIRECTION.R)
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathBuffer, y1 + wallHeight + wallGapHeight + pathBuffer, 
                    wallWidth + wallGapWidth + pathWidth - pathBuffer, pathHeight - path2, (int)MAP_TYPE.PATH);
            if (cell.from == DIRECTION.U || cell.to == DIRECTION.U)
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathBuffer, y1 + wallHeight + wallGapHeight + pathBuffer, 
                    pathWidth - path2, pathHeight + wallGapHeight + wallHeight - pathBuffer, (int)MAP_TYPE.PATH);
            if (cell.from == DIRECTION.D || cell.to == DIRECTION.D)
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathBuffer, y1, 
                    pathWidth - path2, pathHeight + wallGapHeight + wallHeight - pathBuffer, (int)MAP_TYPE.PATH);

        }
    }


    protected Vector3 GetCellCenterPos(int x, int y)
    {
        return new Vector3(puzzleX1 + cellWidth * (x + 0.5f), 0, puzzleY1 + cellHeight * (y + 0.5f));
    }


    //protected List<RectInt> rectList;

    protected Vector2Int puzzleStart, puzzleEnd;


    protected void CheckCellDeep(int x, int y, DIRECTION from, int deep)
    {
        //print("CheckCellDeep:  " + x + ", " + y );
        if (x < 0 || x >= puzzleWidth || y < 0 || y >= puzzleHeight)
            return;
        if (puzzleMap[x][y].value == CELL.INVALID)
            return;
        //print("CheckCellDeep Value:  " + puzzleMap[x][y].value);
        if (puzzleMap[x][y].deep > 0 && puzzleMap[x][y].deep <= deep)        //�w�g����u���|
        {
            print("�ثe�����Ӷ]��o�� ....... ");
            return;
        }

        puzzleMap[x][y].deep = deep;
        puzzleMap[x][y].from = from;
        if (puzzleMap[x][y].U && from != DIRECTION.U)
            CheckCellDeep(x, y + 1, DIRECTION.D, deep + 1);
        if (puzzleMap[x][y].D && from != DIRECTION.D)
            CheckCellDeep(x, y - 1, DIRECTION.U, deep + 1);
        if (puzzleMap[x][y].L && from != DIRECTION.L)
            CheckCellDeep(x - 1, y, DIRECTION.R, deep + 1);
        if (puzzleMap[x][y].R && from != DIRECTION.R)
            CheckCellDeep(x + 1, y, DIRECTION.L, deep + 1);
    }

    protected void CheckMainPathDeep(int x, int y, DIRECTION from, bool isMain, int mainDeep)
    {
        if (x < 0 || x >= puzzleWidth || y < 0 || y >= puzzleHeight)
            return;
        if (puzzleMap[x][y].value == CELL.INVALID)
            return;
        puzzleMap[x][y].isMain = isMain;
        puzzleMap[x][y].mainDeep = mainDeep;

        if (isMain)
        {
            puzzleMap[x][y].to = from;      //���`�X�f����V
        }
        else
        {
            puzzleMap[x][y].to = OneUtility.GetReverseDIR(from);
        }

        DIRECTION mainFrom = puzzleMap[x][y].from;
        if (puzzleMap[x][y].U && from != DIRECTION.U)
        {
            bool mainCheck = isMain && (mainFrom == DIRECTION.U);
            CheckMainPathDeep(x, y + 1, DIRECTION.D, mainCheck, mainDeep + (mainCheck ? -1 : 0));
        }
        if (puzzleMap[x][y].D && from != DIRECTION.D)
        {
            bool mainCheck = isMain && (mainFrom == DIRECTION.D);
            CheckMainPathDeep(x, y - 1, DIRECTION.U, mainCheck, mainDeep + (mainCheck ? -1 : 0));
        }
        if (puzzleMap[x][y].L && from != DIRECTION.L)
        {
            bool mainCheck = isMain && (mainFrom == DIRECTION.L);
            CheckMainPathDeep(x - 1, y, DIRECTION.R, mainCheck, mainDeep + (mainCheck ? -1 : 0));
        }
        if (puzzleMap[x][y].R && from != DIRECTION.R)
        {
            bool mainCheck = isMain && (mainFrom == DIRECTION.R);
            CheckMainPathDeep(x + 1, y, DIRECTION.L, mainCheck, mainDeep + (mainCheck ? -1 : 0));
        }
    }

    virtual protected void PreCalculateGameplayInfo()
    {

        int maxMainDeep = Mathf.Max(puzzleMap[puzzleEnd.x][puzzleEnd.y].deep, 1);
        CheckMainPathDeep(puzzleEnd.x, puzzleEnd.y, DIRECTION.NONE, true, maxMainDeep);

        if (gameManager)
        {
            //gameManager.SetDefaultRoomLayout(roomWidth, roomHeight, pathWidth, pathHeight, wallWidth, wallHeight);
            for (int x = 0; x < puzzleWidth; x++)
            {
                for (int y = 0; y < puzzleHeight; y++)
                {
                    CELL cell = puzzleMap[x][y];
                    if (cell.value == CELL.NORMAL)
                    {
                        float mainRatio = (float)cell.mainDeep / (float)maxMainDeep;

                        //gameManager.AddRoom(GetCellCenterPos(x, y), roomWidth, roomHeight, cell, mainRatio, pathWidth, pathHeight);
                        gameManager.AddRoom(GetCellCenterPos(x, y), cell, mainRatio);
                    }
                }
            }
        }

        //���յ۪����ӳ]�w Gameplay
        if (dungeonEnemyManager)
        {
            float roomEdgeBuffer = 0.5f;
            for (int x = 0; x < puzzleWidth; x++)
            {
                for (int y = 0; y < puzzleHeight; y++)
                {
                    if (puzzleMap[x][y].value == CELL.NORMAL)
                    {
                        DungeonEnemyManagerBase.PosData pData = new DungeonEnemyManagerBase.PosData();
                        pData.pos = GetCellCenterPos(x, y);
                        pData.diffAdd = (float)puzzleMap[x][y].deep / (float)maxMainDeep;
                        if (puzzleMap[x][y].isPath)
                            pData.area = new Vector2(pathWidth - roomEdgeBuffer - roomEdgeBuffer, pathHeight - roomEdgeBuffer - roomEdgeBuffer);
                        else
                            pData.area = new Vector2(roomWidth - roomEdgeBuffer - roomEdgeBuffer, roomHeight - roomEdgeBuffer - roomEdgeBuffer);
                        dungeonEnemyManager.AddNormalPosition(pData);
                    }
                }
            }
        }


    }


    protected void ProcessInitFinish()
    {
        CheckCellDeep(puzzleStart.x, puzzleStart.y, DIRECTION.NONE, 0);
        if (FinishAtDeepest)
        {
            int deepMax = -1;
            CELL mostDeepCell = null;
            for (int x = 0; x < puzzleWidth; x++)
            {
                for (int y = 0; y < puzzleHeight; y++)
                {
                    if (puzzleMap[x][y].deep > deepMax)
                    {
                        deepMax = puzzleMap[x][y].deep;
                        mostDeepCell = puzzleMap[x][y];
                    }
                }
            }
            if (mostDeepCell != null)
            {
                //print("�̻����| = " + mostDeepCell.deep);
                puzzleEnd.x = mostDeepCell.x;
                puzzleEnd.y = mostDeepCell.y;
            }
        }

        startPos = GetCellCenterPos(puzzleStart.x, puzzleStart.y);
        endPos = GetCellCenterPos(puzzleEnd.x, puzzleEnd.y);

        //��l Gameplay
        if (initGampleyRef)
        {
            BattleSystem.SpawnGameObj(initGampleyRef, startPos);
        }

        //�}����
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);

        BattleSystem.GetInstance().SetInitPosition(startPos);
    }

    //==== �@��q�D�B�z
    protected GameObject colliderRoot = null;
    protected void ProcessNormalCells()
    {
        if (createWallCollider && colliderRoot == null)
        {
            colliderRoot = new GameObject("Root_Colliders");
            colliderRoot.transform.parent = transform;
        }

        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                if (puzzleMap[i][j].value == CELL.ROOM)
                    continue;
                int x1 = puzzleX1 + i * cellWidth;
                int y1 = puzzleY1 + j * cellHeight;
                FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);
            }
        }
    }

    virtual protected void BuildGameplay()
    {
        if (dungeonEnemyManager)
        {
            dungeonEnemyManager.BuildAllGameplay();
        }

        if (gameManager)
        {
            gameManager.BuildAll();
        }
    }

    virtual protected void FillAllTiles()
    {
        if (defautTileGroup)
            theMap.FillTileAll(OneMap.DEFAULT_VALUE, blockTM, defautTileGroup.GetTileGroup());

        if (groundEdgeTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false, (int)MAP_TYPE.BLOCK);
        else
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTileGroup.GetTileGroup());

        if (roomGroundTileGroup != null)
        {
            if (roomGroundTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.ROOM, groundTM, groundTM, roomGroundTileGroup.GetTileGroup(), roomGroundTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.ROOM, groundTM, roomGroundTileGroup.GetTileGroup());
        }

        if (mainPathTileGroup != null)
        {
            if (mainPathTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.PATH, groundTM, groundTM, mainPathTileGroup.GetTileGroup(), mainPathTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.PATH, groundTM, mainPathTileGroup.GetTileGroup());
        }

        if (blockTileGroup)
        {
            if (blockTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.BLOCK, blockTM, blockTM, blockTileGroup.GetTileGroup(), blockTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.BLOCK, blockTM, blockTileGroup.GetTileGroup());
        }


        if (groundOutEdgeTileGroup && !blockTileEdgeGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, null, blockTM, null, groundOutEdgeTileGroup.GetTileEdgeGroup(), true, (int)MAP_TYPE.BLOCK);

    }

    //=========================== �sŪ�ɬ���
    override protected void CreateMapSaveData()
    {
        mapDataBase = new MapSaveMazeOne();
    }

    override protected void SaveMap()
    {
        //print("================= Save MO Map");
        //if (mapName == null || mapName == "")
        //    return;

        //MapSaveMazeOne mapData = mapDataBase == null ? new MapSaveMazeOne() : (MapSaveMazeOne)mapDataBase;
        //mapDataBase = mapData;
        base.SaveMap();
        if (mapDataBase == null)
            return;
        MapSaveMazeOne mapData = (MapSaveMazeOne)mapDataBase;
        mapData.className = "MG_MazeOne";
        mapData.mapName = mapName;
        //mapData.cellWidth = cellWidth;
       // mapData.cellHeight = cellHeight;
        mapData.puzzleWidth = puzzleWidth;
        mapData.puzzleHeight = puzzleHeight;
        mapData.wallWidth = wallWidth;
        mapData.wallHeight = wallHeight;
        mapData.roomWidth = roomWidth;
        mapData.roomHeight = roomHeight;
        mapData.pathWidth = pathWidth;
        mapData.pathHeight = pathHeight;

        //mapData.extendTerminal = extendTerminal;
        mapData.portalAfterFirstRoomGamplay = portalAfterFirstRoomGamplay;
        mapData.puzzleStart = puzzleStart;
        mapData.puzzleEnd = puzzleEnd;
        //mapData.startPos = startPos;
        //mapData.endPos = endPos;

        int i = 0;
        byte[] bData = new byte[puzzleHeight * puzzleWidth];
        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                //int ec = EncodeCell(puzzleMap[x][y]);
                int ec = puzzleMap[x][y].Encode();
                bData[i] = (byte)ec;
                i++;
            }
        }
        mapData.puzzleMapData = System.Convert.ToBase64String(bData);
        One.LOG("SaveMap �s�X���G!!" + mapData.puzzleMapData);

        GameSystem.GetPlayerData().SaveMap(mapName, mapData);

    }
    override protected void LoadMap()
    {
        //mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        base.LoadMap();
        if (mapDataBase == null || mapDataBase.GetType() != typeof(MapSaveMazeOne))
        {
            One.LOG("MG_MazeOneBase.LoadMap: �S���s�ɸ��");
            return;
        }

        print("MG_MazeOneBase.LoadMap: ���s�ɸ�� !!!!");
        loadedMapData = (MapSaveMazeOne)mapDataBase;

        mapName = loadedMapData.mapName;
        //cellWidth = loadedMapData.cellWidth;
        //cellHeight = loadedMapData.cellHeight;
        puzzleWidth = loadedMapData.puzzleWidth;
        puzzleHeight = loadedMapData.puzzleHeight;
        wallWidth = loadedMapData.wallWidth;
        wallHeight = loadedMapData.wallHeight;
        roomWidth = loadedMapData.roomWidth;
        roomHeight = loadedMapData.roomHeight;
        pathWidth = loadedMapData.pathWidth;
        pathHeight = loadedMapData.pathHeight;

        //extendTerminal = loadedMapData.extendTerminal;
        portalAfterFirstRoomGamplay = loadedMapData.portalAfterFirstRoomGamplay;
        puzzleStart = loadedMapData.puzzleStart;
        puzzleEnd = loadedMapData.puzzleEnd;
        //startPos = loadedMapData.startPos;
        //endPos = loadedMapData.endPos;
    }

    protected void LoadMazeMap()
    {
        print("���J PuzzleMap ���!!" + loadedMapData.puzzleMapData);

        byte[] bData = System.Convert.FromBase64String(loadedMapData.puzzleMapData);
        if (bData.Length != puzzleWidth * puzzleHeight)
        {
            One.ERROR("Size ���� !!");
        }

        int i = 0;
        puzzleMap = new CELL[puzzleWidth][];
        for (int x = 0; x < puzzleWidth; x++)
        {
            puzzleMap[x] = new CELL[puzzleHeight];
            for (int y = 0; y < puzzleHeight; y++)
            {
                puzzleMap[x][y] = new CELL();
                puzzleMap[x][y].Decode((int)bData[i]);
                i++;
            }
        }

    }

    //// ======= ���������a�Ϫ��O���M�^�_ TODO: �o�Ǧ��G�i�H��� MapGen �� Base �ӳB�z =======================
    //public override void OnEixtMap()
    //{
    //    SaveExploreMap();
    //}

    //protected void SaveExploreMap()
    //{
    //    MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
    //    if (mapDataBase == null || mapDataBase.GetType() != typeof(MapSaveMazeOne))
    //    {
    //        print("SaveExploreMap: �S���s�ɸ�� MapSaveData�A���B�z");
    //        return;
    //    }

    //    MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
    //    if (theMiniMap)
    //    {
    //        MapSaveMazeOne mapData = (MapSaveMazeOne)mapDataBase;
    //        mapData.mapMask64 = theMiniMap.EncodeMaskTexture();
    //    }
    //}

    //protected void LoadExploreMap()
    //{
    //    MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
    //    if (!theMiniMap)
    //    {
    //        return;
    //    }
    //    MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
    //    if (mapDataBase == null || mapDataBase.GetType() != typeof(MapSaveMazeOne))
    //    {
    //        print("LoadExploreMap : �S���s�ɸ�ơA���B�z");
    //        return;
    //    }

    //    MapSaveMazeOne mapData = (MapSaveMazeOne)mapDataBase;
    //    if (mapData.mapMask64 == null || mapData.mapMask64 == "")
    //    {
    //        print("�Ū��a�ϱ�����T: " + mapData.mapMask64);
    //        return;
    //    }

    //    print("LoadExploreMap: ��쪺��r���Y��ơAByte �`�q: " + mapData.mapMask64.Length);
    //    //print("��쪺��r���Y��Ƥ��e: " + mapData.mapMask64);

    //    theMiniMap.DecodeMaskTexture(mapData.mapMask64);
    //}

}
