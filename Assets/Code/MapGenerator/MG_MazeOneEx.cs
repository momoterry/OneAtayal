using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MG_MazeOne : MG_MazeOneBase
{
    //protected bool allConnect = true;

    protected class wallInfo
    {
        public wallInfo(int _id1, int _id2)
        {
            cell_ID_1 = _id1;
            cell_ID_2 = _id2;
        }
        public int cell_ID_1;
        public int cell_ID_2;
    }

    override protected void CreatMazeMap()
    {
        //==== Init
        DisjointSetUnion puzzleDSU = new DisjointSetUnion();
        puzzleDSU.Init(puzzleHeight * puzzleWidth);

        List<wallInfo> wallList = new List<wallInfo>();
        //puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
        //puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);

        //==== Init Connection Info
        wallInfo[,] lrWalls = new wallInfo[puzzleWidth - 1, puzzleHeight];
        wallInfo[,] udWalls = new wallInfo[puzzleWidth, puzzleHeight + 1];

        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                bool addToWallList = true;
                if (puzzleMap[x][y].value == cellInfo.INVALID)
                    addToWallList = false;

                if (x < puzzleWidth - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x + 1, y));
                    if (addToWallList && puzzleMap[x + 1][y].value != cellInfo.INVALID)
                        wallList.Add(w);
                    lrWalls[x, y] = w;
                }
                if (y < puzzleHeight - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x, y + 1));
                    if (addToWallList && puzzleMap[x][y + 1].value != cellInfo.INVALID)
                        wallList.Add(w);
                    udWalls[x, y] = w;
                }
            }
        }

        //==== �}�l�H���s�� !!
        //iStart = GetCellID(puzzleStart.x, puzzleStart.y);
        //iEnd = GetCellID(puzzleEnd.x, puzzleEnd.y);
        //�ϥ��H���Ƨ�
        OneUtility.Shuffle(wallList);
        foreach (wallInfo w in wallList)
        {
            if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //���n����۳s
            {
                ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
                //if (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd))
                //{
                //    //print("�o�{�j���_�� !! Loop = " + (loop + 1));
                //    break;
                //}
            }
        }
        //if (allConnect)
        //{
        //    //�ϥ��H���Ƨ�
        //    OneUtility.Shuffle(wallList);
        //    foreach (wallInfo w in wallList)
        //    {
        //        if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //���n����۳s
        //        {
        //            ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
        //            puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
        //        }
        //    }
        //}
        //else
        //{
        //    //�ϥ��H���Ƨ�
        //    OneUtility.Shuffle(wallList);
        //    foreach (wallInfo w in wallList)
        //    {
        //        if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //���n����۳s
        //        {
        //            ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
        //            puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
        //            if (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd))
        //            {
        //                //print("�o�{�j���_�� !! Loop = " + (loop + 1));
        //                break;
        //            }
        //        }
        //    }
        //    //��ѤU�� Cell �Ц� Invalid
        //    int startValue = puzzleDSU.Find(iStart);
        //    for (int i = 0; i < puzzleWidth; i++)
        //    {
        //        for (int j = 0; j < puzzleHeight; j++)
        //        {
        //            if (puzzleDSU.Find(GetCellID(i, j)) != startValue)
        //                puzzleMap[i][j].value = cellInfo.INVALID;
        //        }
        //    }
        //}

        //startPos = new Vector3(puzzleX1 + GetCellX(iStart) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iStart) * cellHeight + cellHeight / 2);
        //endPos = new Vector3(puzzleX1 + GetCellX(iEnd) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iEnd) * cellHeight + cellHeight / 2);

        //BattleSystem.GetInstance().SetInitPosition(startPos);
    }

    //protected void ConnectCellsByID(int id_1, int id_2)
    //{
    //    cellInfo cell_1 = puzzleMap[GetCellX(id_1)][GetCellY(id_1)];
    //    cellInfo cell_2 = puzzleMap[GetCellX(id_2)][GetCellY(id_2)];
    //    if (id_1 + 1 == id_2) //���s��k
    //    {
    //        cell_1.R = true;
    //        cell_2.L = true;
    //    }
    //    else if (id_1 + puzzleWidth == id_2) //�U�s��W
    //    {
    //        cell_1.U = true;
    //        cell_2.D = true;
    //    }
    //}

    //protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    //protected int GetCellX(int id) { return id % puzzleWidth; }
    //protected int GetCellY(int id) { return id / puzzleWidth; }

}


public class MG_MazeOneEx : MG_MazeOne
{
    public bool extendTerminal = true;

    protected override void PresetMapInfo()
    {
        if (extendTerminal)
        {
            puzzleHeight += 2;
        }
        base.PresetMapInfo();
    }

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();
        if (extendTerminal)
        {
            for (int i = 0; i < puzzleWidth; i++)
            {
                puzzleMap[i][0].value = cellInfo.INVALID;
                puzzleMap[i][puzzleHeight - 1].value = cellInfo.INVALID;
            }
            puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.NORMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.NORMAL;
        }
    }
}