using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;


public class DungeonGeneratorAlpha : MapGeneratorBase
{
    public Tile ground;
    public Tile wall;
    public Tile hole;
    public NavMeshSurface2d theSurface2D;
    public Tilemap tmGround;
    public Tilemap tmWall;
    public GameObject wallBlocker;

    public GameObject enemyNormal;
    public GameObject enemyStrong;
    public GameObject enemyRanger;
    public GameObject enemyBoss;

    public GameObject endGate;  //TODO: 暴力法，待改

    private int doorSize = 2;
    private int currLevel;

    private List<GameObject> wallList = new List<GameObject>();
    private Vector3Int temp = new Vector3Int(0, 0, 0);

    struct EnemyRateAll
    {
        public float nomal;
        public float strong;
        public float ranger;
    }

    enum MAP_DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        BOSS_UP,
        NONE,
    }

    struct enemyInfo
    {
        public int coorX;
        public int coorY;
        public int ID;
    }
    private List<enemyInfo> enemyInfoList = new List<enemyInfo>();

    struct RoomGameplyInfo
    {
        public int wallCount;
        public int holeCount;
        public int e1Count;
        public int e2Count;
        public int e3Count;
    }

    void Start()
    {
        //TEST

    }


    private void BuildLinearDungeon()
    {
        int pathLen = 5;
        int[] sizeArrayX = new int[] { 8, 8, 8 };
        int[] sizeArrayY = new int[] { 5, 6, 7 };
        int currX = 0;
        int currY = 0;
        GenerateInitArea(currX, currY);
        for (int i = 0; i < sizeArrayY.Length; i++)
        {
            GenerateVerticalPath(currX, currY, currY + pathLen - 1);
            currY += pathLen;
            currY += sizeArrayY[i] + 1;
            GenerateRoom(currX, currY, sizeArrayX[i], sizeArrayY[i], (i < sizeArrayY.Length - 1), true, false, false);
            currY += sizeArrayY[i] + 1;
        }

        endGate.transform.position = new Vector3((float)currX, (float)currY - 0.5f, 0);
        endGate.SetActive(true);    //TODO: 只是測試
    }

    private MAP_DIRECTION[] GetRandomRoomDirections()
    {
        MAP_DIRECTION[][] rdMap = new MAP_DIRECTION[][]
        {
            new MAP_DIRECTION[] {MAP_DIRECTION.UP, MAP_DIRECTION.RIGHT, MAP_DIRECTION.UP, MAP_DIRECTION.BOSS_UP },
            new MAP_DIRECTION[] {MAP_DIRECTION.UP, MAP_DIRECTION.LEFT, MAP_DIRECTION.UP, MAP_DIRECTION.BOSS_UP },
            new MAP_DIRECTION[] {MAP_DIRECTION.UP, MAP_DIRECTION.UP, MAP_DIRECTION.RIGHT, MAP_DIRECTION.BOSS_UP },
            new MAP_DIRECTION[] {MAP_DIRECTION.UP, MAP_DIRECTION.UP, MAP_DIRECTION.LEFT, MAP_DIRECTION.BOSS_UP },
            new MAP_DIRECTION[] {MAP_DIRECTION.UP, MAP_DIRECTION.UP, MAP_DIRECTION.UP, MAP_DIRECTION.BOSS_UP },
        };
        int rd = Random.Range(1, rdMap.Length);
        return rdMap[rd];
    }

    private void BuildDungeonAlpha()
    {
        int pathLen = 4;

        MAP_DIRECTION[] roomDirections = GetRandomRoomDirections();

        //====== TODO: 以下正式開始建置，以上的參數之後希望外部化

        int currX = 0;
        int currY = 0;
        //固定向上的開頭 TODO: 加入開頭方向的改動
        GenerateInitArea(currX, currY);

        for (int i = 0; i<roomDirections.Length; i++)
        {
            bool isFinalRoom = (i == (roomDirections.Length - 1));
            MAP_DIRECTION curr = roomDirections[i];
            MAP_DIRECTION next = isFinalRoom ? MAP_DIRECTION.NONE : roomDirections[i + 1];

            //房間隨機大小
            int sizeX;
            int sizeY;
            if (curr == MAP_DIRECTION.BOSS_UP)
            {
                sizeX = 10;
                sizeY = 10;
            }
            else
            {
                sizeX = Random.Range(5, 8);
                sizeY = Random.Range(5, 8);
            }

            bool doorUp = false, doorDown = false, doorLeft = false, doorRight = false;
            //跟據方向產生通道，並跟據通道的方向確認房間中間位置
            switch (curr)
            {
                case MAP_DIRECTION.UP:
                case MAP_DIRECTION.BOSS_UP:
                    GenerateVerticalPath(currX, currY, currY + pathLen -1);
                    currY += pathLen + sizeY + 1;
                    doorDown = true;
                    break;
                case MAP_DIRECTION.LEFT:
                    doorRight = true;
                    GenerateHorizontalPath(currY, currX - pathLen, currX-1);
                    currX -= pathLen + sizeX + 1;
                    break;
                case MAP_DIRECTION.RIGHT:
                    doorLeft = true;
                    GenerateHorizontalPath(currY, currX, currX + pathLen - 1);
                    currX += pathLen + sizeX + 1;
                    break;
                case MAP_DIRECTION.DOWN:
                    GenerateVerticalPath(currX, currY - pathLen, currY-1);
                    currY -= pathLen + sizeY + 1;
                    doorUp = true;
                    break;
            }

            //根據下一個房間的方向，決定門是否開啟，以及下一個游標位置
            int addX = 0;
            int addY = 0;
            switch (next)
            {
                case MAP_DIRECTION.UP:
                case MAP_DIRECTION.BOSS_UP:
                    if (!isFinalRoom)
                        doorUp = true;
                    addY = sizeY + 1;
                    break;
                case MAP_DIRECTION.DOWN:
                    if (!isFinalRoom)
                        doorDown = true;
                    addY = -sizeY - 1;
                    break;
                case MAP_DIRECTION.RIGHT:
                    if (!isFinalRoom)
                        doorRight = true;
                    addX = sizeX + 1;
                    break;
                case MAP_DIRECTION.LEFT:
                    if (!isFinalRoom)
                        doorLeft = true;
                    addX = -sizeX - 1;
                    break;
            }

            //產生基本房間架構
            GenerateRoom(currX, currY, sizeX, sizeY, doorUp, doorDown, doorLeft, doorRight);
            RoomGameplyInfo gInfo = new RoomGameplyInfo();
            if (curr != MAP_DIRECTION.BOSS_UP)
            {
                //隨生隨機房間內容
                float gArea = ((float)sizeX * (float)sizeY) * 4.0f;
                gInfo.wallCount = (int)(gArea * 0.05f);
                gInfo.holeCount = (int)(gArea * 0.05f);
                gInfo.e1Count = 3 + i * 2;
                if (i>=2)
                    gInfo.e2Count = 1 + currLevel;
                else
                    gInfo.e2Count = 0;
                gInfo.e3Count = i * 2;

                GenRandomRoomGameplay(currX, currY, sizeX - 1, sizeY - 1, gInfo);
            }
            else
            {
                //Boss Room
                gInfo.wallCount = 0;
                gInfo.holeCount = 0;
                gInfo.e1Count = (currLevel-1) * 3;
                gInfo.e2Count = (currLevel - 1) * 2;
                gInfo.e3Count = (currLevel - 1);
                GenBossGameplay(currX, currY, sizeX - 1, sizeY - 1, gInfo);
            }


            //移動游標，準備下一步
            currX += addX;
            currY += addY;

            //如果是最後一個房間，直接根據這個房間的方向，配置 End Gate 的位置
            if (isFinalRoom)
            {
                switch (curr)
                {
                    case MAP_DIRECTION.UP:
                    case MAP_DIRECTION.BOSS_UP:
                        addY = sizeY;
                        break;
                    case MAP_DIRECTION.DOWN:
                        addY = -sizeY;
                        break;
                    case MAP_DIRECTION.LEFT:
                        addX = -sizeX;
                        break;
                    case MAP_DIRECTION.RIGHT:
                        addX = sizeX;
                        break;
                }
                currX += addX;
                currY += addY;
                endGate.transform.position = new Vector3((float)currX, (float)currY, 0);
                //endGate.SetActive(true);    //TODO: 只是測試
            }
        }

    }

    public override void BuildAll( int buildLevel = 1)
    {

        if (buildLevel >= 10)
            buildLevel = 10; //暫定最大 10 難度

        currLevel = buildLevel;

        ClearAll();

        // ================== 產生 Room ==================

        //BuildLinearDungeon();
        BuildDungeonAlpha();

        // ================== 產生 NavMesh ==================

        theSurface2D.BuildNavMesh();

        // ================== 敵人生成 ==================
        foreach (enemyInfo o in enemyInfoList)
        {
            Vector3Int coor = new Vector3Int(o.coorX, o.coorY, 0);
            Vector3 pos = tmWall.CellToWorld(coor);
            pos.x += 0.5f;
            pos.y += 0.5f;
            //print("Enemy Spawn Position : " + pos);
            GameObject enemyObj = null;
            switch (o.ID)
            {
                case 1001:
                    enemyObj = Instantiate(enemyNormal, pos, Quaternion.identity, null);
                    break;
                case 2001:
                    enemyObj = Instantiate(enemyStrong, pos, Quaternion.identity, null);
                    break;
                case 1002:
                    enemyObj = Instantiate(enemyRanger, pos, Quaternion.identity, null);
                    break;
                case 3001:
                    enemyObj = Instantiate(enemyBoss, pos, Quaternion.identity, null);
                    break;
                default:
                    print("Invalid Enenmy ID: " + o.ID);
                    //Instantiate(enemyNormal, pos, Quaternion.identity, null);
                    break;
            }
            if (enemyObj)
            {
                Enemy e = enemyObj.GetComponent<Enemy>();
                if (e)
                    e.SetUpLevel(buildLevel);
            }
        }

        print("====== Enemy Total : " + enemyInfoList.Count);
        enemyInfoList.Clear();

    }


    private void GenOneWall(Vector3Int coor)
    {
        tmWall.SetTile(coor, wall);
        Vector3 pos = tmWall.CellToWorld(coor);
        pos += new Vector3(0.5f, 0.5f, 0.0f);
        GameObject w = Instantiate(wallBlocker, pos, Quaternion.identity, null);
        wallList.Add(w);
    }

    private void GenerateInitArea( int centerX, int centerY)
    {
        int areaDepth = 2;
        GenerateVerticalPath(centerX, centerY - areaDepth, centerY);
        GenHorizontalWall(centerY - areaDepth - 1, centerX - doorSize - 1, centerX + doorSize);

    }

    private void GenHorizontalWall(int y, int xStart, int xEnd)
    {
        temp.y = y;
        for ( int i=xStart; i<= xEnd; i++)
        {
            temp.x = i;
            tmWall.SetTile(temp, wall);
        }

        //直接產生一長條 Collider
        Vector3 v1 = tmWall.CellToWorld(new Vector3Int(xStart, y, 0));
        Vector3 v2 = tmWall.CellToWorld(new Vector3Int(xEnd, y, 0));
        Vector3 v = (v1 + v2) * 0.5f + new Vector3(0.5f, 0.5f, 0);
        float scale = (float)(xEnd - xStart + 1);
        GameObject w = Instantiate(wallBlocker, v, Quaternion.identity, null);
        w.transform.localScale = new Vector3(scale, 1.0f, 1.0f);
        wallList.Add(w);
    }

    private void GenVerticalWall(int x, int yStart, int yEnd)
    {
        temp.x = x;
        for (int i = yStart; i <= yEnd; i++)
        {
            temp.y = i;
            tmWall.SetTile(temp, wall);
        }

        //直接產生一長條 Collider
        Vector3 v1 = tmWall.CellToWorld(new Vector3Int(x, yStart, 0));
        Vector3 v2 = tmWall.CellToWorld(new Vector3Int(x, yEnd, 0));
        Vector3 v = (v1 + v2) * 0.5f + new Vector3(0.5f, 0.5f, 0);
        float scale = (float)(yEnd - yStart + 1);
        GameObject w = Instantiate(wallBlocker, v, Quaternion.identity, null);
        w.transform.localScale = new Vector3(1.0f, scale, 1.0f);
        wallList.Add(w);
    }

    private void GenerateVerticalPath(int centerX, int startY, int endY)
    {
        //地板
        for (int j = startY; j<= endY; j++)
        {
            temp.y = j;
            for (int i = centerX-doorSize-1; i<=centerX+doorSize; i++)
            {
                temp.x = i;
                tmGround.SetTile(temp, ground);
            }
        }

        //側牆
        GenVerticalWall(centerX - doorSize - 1, startY, endY);
        GenVerticalWall(centerX + doorSize, startY, endY);
    }

    private void GenerateHorizontalPath(int centerY, int startX, int endX)
    {
        for (int i = startX; i<=endX; i++)
        {
            temp.x = i;
            for (int j = centerY -doorSize-1;j <= centerY+doorSize; j++)
            {
                temp.y = j;
                tmGround.SetTile(temp, ground);
            }
        }

        //側牆
        GenHorizontalWall(centerY - doorSize - 1, startX, endX);
        GenHorizontalWall(centerY + doorSize, startX, endX);
    }

    private void GenerateRoom(int centerX, int centerY, int sizeX, int sizeY, bool doorUp, bool doorDown, bool doorLeft, bool doorRight)
    {
        //地板
        for (int i = centerX - sizeX -1; i <= centerX + sizeX; i++)
        {
            for (int j = centerY - sizeY -1; j <= centerY + sizeY; j++)
            {
                temp.x = i;
                temp.y = j;
                tmGround.SetTile(temp, ground);
            }
        }

        int x1 = centerX - sizeX - 1;
        int x4 = centerX + sizeX;
        int x2 = doorUp ? centerX - doorSize-1: centerX-1;
        int x3 = doorUp ? centerX + doorSize: centerX;
        // 水平外牆 1
        GenHorizontalWall(centerY + sizeY, x1, x2);
        GenHorizontalWall(centerY + sizeY, x3, x4);
        x2 = doorDown ? centerX - doorSize - 1 : centerX - 1;
        x3 = doorDown ? centerX + doorSize : centerX;
        // 水平外牆 2
        GenHorizontalWall(centerY - sizeY - 1, x1, x2);
        GenHorizontalWall(centerY - sizeY - 1, x3, x4);

        int y1 = centerY - sizeY - 1;
        int y4 = centerY + sizeY;
        int y2 = doorRight ? centerY - doorSize - 1 : centerY - 1;
        int y3 = doorRight ? centerY + doorSize : centerY;
        // 垂直外牆 1
        GenVerticalWall(centerX + sizeX, y1, y2);
        GenVerticalWall(centerX + sizeX, y3, y4);
        y2 = doorLeft ? centerY - doorSize - 1 : centerY - 1;
        y3 = doorLeft ? centerY + doorSize : centerY;
        // 垂直外牆 2
        GenVerticalWall(centerX - sizeX -1, y1, y2);
        GenVerticalWall(centerX - sizeX -1, y3, y4);
    }

    private void ClearAll()
    {
        tmGround.ClearAllTiles();
        tmWall.ClearAllTiles();

        foreach (GameObject w in wallList)
        {
            Destroy(w);
        }
        wallList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void GenRandomRoomGameplay(int centerX, int centerY, int sizeX, int sizeY, RoomGameplyInfo gInfo)
    {
        int rdAll = gInfo.wallCount + gInfo.holeCount + gInfo.e1Count + gInfo.e2Count + gInfo.e3Count;
        Vector3Int[] allCell = new Vector3Int[ sizeX * sizeY * 4 ];

        int ind = 0;
        for (int i=centerX-sizeX;i<centerX+sizeX; i++)
        {
            for (int j=centerY-sizeY;j<centerY+sizeY; j++)
            {
                allCell[ind].x = i;
                allCell[ind].y = j;
                allCell[ind].z = 0;
                ind++;
            }
        }

        int total = ind;
        for (int i=0; i<rdAll; i++)
        {
            int rd = Random.Range(i + 1, total - 1);
            Vector3Int temp = allCell[i];
            allCell[i] = allCell[rd];
            allCell[rd] = temp;
        }

        print("--GenRandomRoomGameplay-- rdAll: " + rdAll + " , total Cell: " + total);

        for (int i = 0; i < rdAll; i++)
        {
            enemyInfo newEneInfo;
            newEneInfo.coorX = allCell[i].x;
            newEneInfo.coorY = allCell[i].y;

            if (i< gInfo.wallCount)
                GenOneWall(allCell[i]);
            else if (i < gInfo.wallCount + gInfo.holeCount)
            {
                tmWall.SetTile(allCell[i], hole);
            }
            else if (i < gInfo.wallCount + gInfo.holeCount + gInfo.e1Count)
            {
                newEneInfo.ID = 1001;
                enemyInfoList.Add(newEneInfo);
            }
            else if (i < gInfo.wallCount + gInfo.holeCount + gInfo.e1Count + gInfo.e2Count)
            {
                newEneInfo.ID = 2001;
                enemyInfoList.Add(newEneInfo);
            }
            else if (i < gInfo.wallCount + gInfo.holeCount + gInfo.e1Count + gInfo.e2Count + gInfo.e3Count)
            {
                newEneInfo.ID = 1002;
                enemyInfoList.Add(newEneInfo);
            }
        }

    }

    void GenBossGameplay(int centerX, int centerY, int sizeX, int sizeY, RoomGameplyInfo gInfo)
    {
        //如果有小兵，產在 Boss 後方
        GenRandomRoomGameplay(centerX, centerY + sizeY / 2 + 1, sizeX, sizeY / 2 - 2, gInfo);
        enemyInfo newEneInfo;
        newEneInfo.coorX = centerX;
        newEneInfo.coorY = centerY;
        newEneInfo.ID = 3001;
        enemyInfoList.Add(newEneInfo);
    }

    //private void GenerateRandomWalls(float wallRate = 15.0f, float holeRate = 15.0f)
    //{
    //    for (int i = -10; i < 10; i++)
    //    {
    //        for (int j = -5; j < 5; j++)
    //        {
    //            float rd = Random.Range(0.0f, 100.0f);
    //            Vector3Int tileCoordinate = new Vector3Int(i, j, 0);
    //            if (rd < wallRate)
    //            {
    //                tmWall.SetTile(tileCoordinate, wall);
    //                Vector3 pos = tmWall.CellToWorld(tileCoordinate);
    //                pos += new Vector3(0.5f, 0.5f, 0.0f);
    //                GameObject w = Instantiate(wallBlocker, pos, Quaternion.identity, null);
    //                wallList.Add(w);
    //            }
    //            else if (rd < wallRate + holeRate)
    //            {
    //                tmWall.SetTile(tileCoordinate, hole);
    //            }
    //            else
    //                tmWall.SetTile(tileCoordinate, null);
    //        }
    //    }
    //}

    //private void GenerateRandomEnemies(EnemyRateAll enemyInfo)
    //{
    //    int enemyCount = 0;
    //    for (int i = -10; i < 10; i++)
    //    {
    //        for (int j = -2; j < 5; j++)
    //        {
    //            Vector3Int tileCoordinate = new Vector3Int(i, j, 0);
    //            if (tmWall.GetTile(tileCoordinate) == null)
    //            {
    //                Vector3 pos = tmWall.CellToWorld(tileCoordinate);
    //                pos += new Vector3(0.5f, 0.5f, 0.0f);                  
    //                float rd = Random.Range(0.0f, 100.0f);
    //                if (rd < enemyInfo.nomal)
    //                {
    //                    Instantiate(enemyNormal, pos, Quaternion.identity, null);
    //                    enemyCount++;
    //                }
    //                else if (rd < enemyInfo.nomal + enemyInfo.strong)
    //                {
    //                    Instantiate(enemyStrong, pos, Quaternion.identity, null);
    //                    enemyCount++;
    //                }
    //                else if (rd < enemyInfo.nomal + enemyInfo.strong + enemyInfo.ranger)
    //                {
    //                    Instantiate(enemyRanger, pos, Quaternion.identity, null);
    //                    enemyCount++;
    //                }
    //            }

    //        }
    //    }
    //    print("GenerateRandomEnemies() Total Enemy = " + enemyCount);

    //    //TODO: 暴力法
    //    if (enemyCount == 0)
    //    {
    //        GenerateRandomEnemies(enemyInfo);
    //    }
    //}
}
