using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MG_PuzzleDisjointSet : MG_ForestRD
{
    public int cellSize = 4;
    public int puzzleHeight = 6;
    public int puzzleWidth = 6;
    public bool allConnect = true;
    public bool extendTerminal = true;
    public GameObject finishPortalRef;
    public GameObject helperRef;
    public GameObject hintRef;

    public Text winMessage;

    protected int bufferX = 0;
    protected int bufferY = 0;

    protected int iStart;
    protected int iEnd;
    protected Vector3 startPos;
    protected Vector3 endPos;

    protected NavMeshAgent pcAgent;
    protected int pcNaveAgentToSleep = -1;

    List<Vector2Int> correctPathList = new List<Vector2Int>();

    protected class cellInfo
    {
        //public int ID;
        public bool U, D, L, R;
    }

    protected DisjointSetUnion puzzleDSU = new DisjointSetUnion();
    protected cellInfo[][] puzzleMap;

    protected class wallInfo
    {
        public wallInfo( int _id1, int _id2)
        {
            cell_ID_1 = _id1;
            cell_ID_2 = _id2;
        }
        public int cell_ID_1;
        public int cell_ID_2;
    }
    protected List<wallInfo> wallList = new List<wallInfo>();

    private void Update()
    {
        if (pcNaveAgentToSleep > 0)
        {
            pcNaveAgentToSleep--;
            if (pcNaveAgentToSleep <=0)
            {
                if (pcAgent)
                {
                    pcAgent.enabled = true;
                }
                pcNaveAgentToSleep = -1;
            }
        }
    }

    protected override void PreCreateMap()
    {
        //讀取大小
        int userSetSize = GameSystem.GetInstance().GetMazeUserSize();
        if (userSetSize > 0)
        {
            puzzleHeight = userSetSize;
            puzzleWidth = userSetSize;
        }
        if (winMessage)
        {
            winMessage.text = "恭喜你突破了\n" + puzzleWidth + " x " + puzzleHeight + "\n的迷宮挑戰";
        }

        if (extendTerminal)
        {
            bufferX = 0;
            bufferY = 1;
        }
        else
        {
            bufferY = 0;
            bufferX = 0;
        }
        mapHeight = (puzzleHeight + bufferY + bufferY) * cellSize;  //加入上下緩衝
        mapWidth = (puzzleWidth + bufferX + bufferX) * cellSize;
        mapCenter.y = puzzleHeight * cellSize / 2 - (cellSize / 2);
        if (extendTerminal)
            mapCenter.y += cellSize;

        if (puzzleWidth % 2 == 0)
        {
            mapCenter.x = -cellSize / 2;
        }
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        int x2 = x1 + width - 1;
        int y2 = y1 + height - 1;
        //FillSquareInMap((int)TILE_TYPE.GRASS, x1, y1, width, height);
        theMap.SetValue(x1, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x1, y2, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y2, (int)TILE_TYPE.BLOCK);
        if (!cell.D)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y1, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.U)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y2, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.L)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x1, y, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.R)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x2, y, (int)TILE_TYPE.BLOCK);
        }
    }

    protected void ConnectCellsByID( int id_1, int id_2)
    {
        cellInfo cell_1 = puzzleMap[GetCellX(id_1)][GetCellY(id_1)];
        cellInfo cell_2 = puzzleMap[GetCellX(id_2)][GetCellY(id_2)];
        if (id_1 + 1 == id_2) //左連到右
        {
            cell_1.R = true;
            cell_2.L = true;
        }
        else if ( id_1 + puzzleWidth == id_2) //下連到上
        {
            cell_1.U = true;
            cell_2.D = true;
        }
    }

    protected void MarkCellbyID( int _id)
    {
        int puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
        int puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);
        int x1 = GetCellX(_id) * cellSize + puzzleX1;
        int y1 = GetCellY(_id) * cellSize + puzzleY1;
        //FillSquareInMap((int)TILE_TYPE.DIRT, new Vector3Int(x1, y1, 0), cellSize, cellSize);
        FillSquareInMap((int)TILE_TYPE.DIRT, x1, y1, cellSize, cellSize);
    }

    protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    protected int GetCellX(int id) { return id % puzzleWidth; }
    protected int GetCellY(int id) { return id / puzzleWidth; }

    protected override void CreateForestMap()
    {
        //==== Init Puzzle Map
        puzzleDSU.Init(puzzleHeight * puzzleWidth);
        puzzleMap = new cellInfo[puzzleWidth][];
        for (int i=0; i<puzzleWidth; i++)
        {
            puzzleMap[i] = new cellInfo[puzzleHeight];
            for (int j=0; j<puzzleHeight; j++)
            {
                puzzleMap[i][j] = new cellInfo();
            }
        }
        //==== Init Connection Info
        for (int x=0; x < puzzleWidth-1; x++)
        {
            for (int y=0; y < puzzleHeight-1; y++)
            {
                wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x+1,y)));
                wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x, y+1)));
            }
        }

        //==== 開始隨機連結 !!
        iStart = GetCellID(puzzleWidth / 2, 0);
        iEnd = GetCellID(puzzleWidth / 2, puzzleHeight-1);

        int loop = 0;
        int wallTotal = wallList.Count;
        while (loop < wallTotal)
        {
            loop++;
            int rd = Random.Range(0, wallList.Count);
            wallInfo w = wallList[rd];
            if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //不要自體相連
            {
                ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
            }
            wallList.Remove(w);

            if (!allConnect && (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd)))
            {
                print("發現大祕寶啦 !! Loop = " + (loop + 1));
                break;
            }
        }

        //==== Set up all cells
        int puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
        int puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);

        MarkCellbyID(iStart);
        MarkCellbyID(iEnd);
        startPos = new Vector3(puzzleX1 + GetCellX(iStart) * cellSize + cellSize / 2, 1, puzzleY1 + GetCellY(iStart) * cellSize + cellSize / 2);
        endPos = new Vector3(puzzleX1 + GetCellX(iEnd) * cellSize + cellSize / 2, 1, puzzleY1 + GetCellY(iEnd)* cellSize + cellSize / 2);

        //== 緩衝區處理
        if (extendTerminal)
        {
            int bufferSizeY = bufferY * cellSize;
            int bufferSizeX = bufferX * cellSize;
            FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y - (mapHeight / 2), mapWidth, bufferSizeY);
            FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y + (mapHeight / 2) - bufferSizeY, mapWidth, bufferSizeY);
            FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y - (mapHeight / 2), bufferSizeX, mapHeight);
            FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x + (mapWidth / 2) - bufferSizeX, mapCenter.y - (mapHeight / 2), bufferSizeX, mapHeight);

            //起始區處理
            cellInfo cStart = new cellInfo();
            cellInfo cEnd = new cellInfo();
            cStart.U = true;
            cEnd.D = true;
            FillSquareInMap((int)TILE_TYPE.DIRT, puzzleX1 + GetCellX(iStart) * cellSize, puzzleY1 + (GetCellY(iStart) - 1) * cellSize, cellSize, cellSize);
            FillCell(cStart, puzzleX1 + GetCellX(iStart) * cellSize, puzzleY1 + (GetCellY(iStart) - 1) * cellSize, cellSize, cellSize);
            FillSquareInMap((int)TILE_TYPE.DIRT, puzzleX1 + GetCellX(iEnd) * cellSize, puzzleY1 + (GetCellY(iEnd) + 1) * cellSize, cellSize, cellSize);
            FillCell(cEnd, puzzleX1 + GetCellX(iEnd) * cellSize, puzzleY1 + (GetCellY(iEnd) + 1) * cellSize, cellSize, cellSize);
            puzzleMap[GetCellX(iStart)][GetCellY(iStart)].D = true;
            puzzleMap[GetCellX(iEnd)][GetCellY(iEnd)].U = true;

            startPos.z -= cellSize;
            endPos.z += cellSize;
        }

        for (int i=0; i< puzzleWidth; i++)
        {
            for (int j=0; j<puzzleHeight; j++)
            {
                int x1 = puzzleX1 + i * cellSize;
                int y1 = puzzleY1 + j * cellSize;
                FillCell(puzzleMap[i][j], x1, y1, cellSize, cellSize);
            }
        }

        //破關門
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);

    }
    
    //==========================================================================
    //      找出正確路徑 
    //==========================================================================

    //方向順序: 上(0)、左(1)、右(2)、下(3)
    protected bool SearchCell( cellInfo cell, int fromDir, int x, int y )
    {
        //print("Search: " + x + ", "+y);
        Vector2Int newNode = new Vector2Int(x, y);
        correctPathList.Add(newNode);

        if (GetCellID(x, y) == iEnd)
        {
            //print("找到終點啦 !!");
            return true;
        }

        bool result = false;
        if (fromDir != 0 && cell.U)
        {
            result = SearchCell(puzzleMap[x][y + 1], 3, x, y + 1);
            if (result)
                return true;
        }
        if (fromDir != 1 && cell.L)
        {
            result = SearchCell(puzzleMap[x - 1][y], 2, x - 1, y);
            if (result)
                return true;
        }
        if (fromDir != 2 && cell.R)
        {
            result = SearchCell(puzzleMap[x + 1][y], 1, x + 1, y);
            if (result)
                return true;
        }
        if (fromDir != 3 && cell.D)
        {
            result = SearchCell(puzzleMap[x][y - 1], 0, x, y - 1);
            if (result)
                return true;
        }

        correctPathList.Remove(newNode);
        return false;
    }

    protected void ShowCorrectPath()
    {
        if (correctPathList.Count > 0)
        {
            return;
        }
        int x = GetCellX(iStart);
        int y = GetCellY(iStart);
        bool result = SearchCell(puzzleMap[x][y], 3, x, y);
        if (result)
        {
            //print("=========把路徑印出來 !!=============");
            for (int i=0; i<correctPathList.Count-1; i++)
            {
                Vector2Int p = correctPathList[i];
                Vector2Int pNext = correctPathList[i+1];
                MarkPathWithNext(p.x, p.y, pNext.x, pNext.y);
            }
            foreach (Vector2Int p in correctPathList)
            {
                GeneratePathTile(p.x, p.y);
            }
        }

        if (winMessage)
        {
            winMessage.text = "到達出口......";
        }
    }

    protected void MarkPathWithNext(int x1, int y1, int x2, int y2)
    {
        int x = Mathf.Min(x1, x2);
        int y = Mathf.Min(y1, y2);
        int puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
        int puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);
        int x0 = x * cellSize + puzzleX1 + 1;
        int y0 = y * cellSize + puzzleY1 + 1;
        int width = Mathf.Abs(x1 - x2) * cellSize + cellSize - 2;
        int height = Mathf.Abs(y1 - y2) * cellSize + cellSize - 2;
        FillSquareInMap((int)TILE_TYPE.DIRT, x0, y0, width, height);
    }

    protected void GeneratePathTile(int x, int y)
    {
        int puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
        int puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);
        int x1 = x * cellSize + puzzleX1;
        int y1 = y * cellSize + puzzleY1;
        EdgeDetectInMap((int)TILE_TYPE.DIRT, (int)TILE_TYPE.DIRT_EDGE, x1, y1, cellSize, cellSize);
        GenerateTiles(x1, y1, cellSize, cellSize);
    }

    //==========================================================================
    //      連接操作介面用 
    //==========================================================================

    public void OnCallHelp()
    {
        SystemUI.ShowYesNoMessageBox(OnHelpMessageBoxResult, "使用提示嗎? ");
    }

    public void OnHelpMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES && helperRef)
        {
            ShowCorrectPath();
        }
    }

    public void OnReturnToStart()
    {
        SystemUI.ShowYesNoMessageBox(OnReturnMessageBoxResult, "你確定要回到起點嗎? ");
    }

    public void OnReturnMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            BattleSystem.GetPC().DoTeleport(startPos, 0);
            pcAgent = BattleSystem.GetPC().gameObject.GetComponentInChildren<NavMeshAgent>();
            if (pcAgent)
            {
                pcAgent.enabled = false;
                pcNaveAgentToSleep = 5;
            }
        }
    }

    public void OnExitMaze()
    {
        SystemUI.ShowYesNoMessageBox(OnExitMessageBoxResult, "你確定要離開迷宮嗎? ");
    }

    public void OnExitMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            BattleSystem.GetInstance().OnBackPrevScene();
        }
    }
}


public class DisjointSetUnion
{
    int size;
    int[] P;
    public void Init(int _size)
    {
        P = new int[_size];
        for (int i = 0; i < _size; i++)
            P[i] = i;
    }

    public int Find(int _id)
    {
        if (_id == P[_id])
            return _id;
        else
            return Find(P[_id]);
    }

    public void Union(int a, int b)
    {
        int Fa = Find(a);
        int Fb = Find(b);
        if (Fa != Fb)
        {
            P[Fb] = P[Fa];
        }
    }
}

