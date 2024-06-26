using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用 BackTracing 演算法來製造迷宮，可能可以有更長的路徑

public class MG_MazeOneBT : MG_MazeOneBase
{
    public bool isDebug = true;

    protected OneUtility.DisjointSetUnion puzzleDSU = new OneUtility.DisjointSetUnion();
    protected List<CELL> cellList = new List<CELL>();
    protected int startDSU = 0;
    override protected void CreatMazeMap()
    {
        puzzleDSU.Init(puzzleHeight * puzzleWidth);
        cellList.Add(puzzleMap[puzzleStart.x][puzzleStart.y]);
        startDSU = puzzleDSU.Find(GetCellID(puzzleStart.x, puzzleStart.y));

        if (isDebug)
        {
            IEnumerator theC = BuildMapIterator();
            StartCoroutine(theC);
        }
        else
        {
            BuildMapImmediate();
        }

        //CheckCellDeep(puzzleStart.x, puzzleStart.y, DIRECTION.NONE, 0);
        //int deepMax = -1;
        //CELL mostDeepCell = null;
        //for (int x=0; x<puzzleWidth; x++)
        //{
        //    for (int y=0; y<puzzleHeight; y++)
        //    {
        //        if (puzzleMap[x][y].deep > deepMax)
        //        {
        //            deepMax = puzzleMap[x][y].deep;
        //            mostDeepCell = puzzleMap[x][y];
        //        }
        //    }
        //}
        //if (mostDeepCell != null)
        //{
        //    print("最遠路徑 = " + mostDeepCell.deep);
        //    puzzleEnd.x = mostDeepCell.x;
        //    puzzleEnd.y = mostDeepCell.y;
        //}
    }

    protected override void PreCalculateGameplayInfo()
    {
        if (!isDebug)
            base.PreCalculateGameplayInfo();
    }


    protected CELL TryConnectRandomCell(CELL cell)
    {
        List<DIRECTION> choices = new List<DIRECTION>();
        if (!cell.L && cell.x > 0 && puzzleMap[cell.x-1][cell.y].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x - 1, cell.y)) != startDSU)
                choices.Add(DIRECTION.L);

        }
        if (!cell.D && cell.y > 0 && puzzleMap[cell.x][cell.y - 1].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x, cell.y - 1)) != startDSU)
                choices.Add(DIRECTION.D);

        }
        if (!cell.R && cell.x < puzzleWidth - 1 && puzzleMap[cell.x + 1][cell.y].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x + 1, cell.y)) != startDSU)
                choices.Add(DIRECTION.R);

        }
        if (!cell.U && cell.y < puzzleHeight - 1 && puzzleMap[cell.x][cell.y + 1].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x, cell.y + 1)) != startDSU)
                choices.Add(DIRECTION.U);
        }

        if (choices.Count == 0)
        {
            //print("找到底囉!!");
            return null;
        }

        DIRECTION dir = choices[Random.Range(0, choices.Count)];
        CELL toCell = null;
        int toDSU = -1;
        switch (dir)
        {
            case DIRECTION.U:
                toCell = puzzleMap[cell.x][cell.y + 1];
                toDSU = GetCellID(cell.x, cell.y + 1);
                break;
            case DIRECTION.D:
                toCell = puzzleMap[cell.x][cell.y - 1];
                toDSU = GetCellID(cell.x, cell.y - 1);

                break;
            case DIRECTION.L:
                toCell = puzzleMap[cell.x - 1][cell.y];
                toDSU = GetCellID(cell.x - 1, cell.y);

                break;
            case DIRECTION.R:
                toCell = puzzleMap[cell.x + 1][cell.y];
                toDSU = GetCellID(cell.x + 1, cell.y);

                break;
        }
        ConnectCells(cell, toCell, dir);
        puzzleDSU.Union(startDSU, toDSU);

        return toCell;
    }

    protected bool gotFinal = false;
    protected bool DoOneCycle()
    {
        CELL cellToGo;
        if (gotFinal)
        {
            //cellToGo = cellList[0];
            cellToGo = cellList[Random.Range(0, cellList.Count)];
            //cellToGo = cellList[cellList.Count - 1];
        }
        else
        {
            //cellToGo = cellList[0];
            //cellToGo = cellList[Random.Range(0, cellList.Count)];
            cellToGo = cellList[cellList.Count - 1];
        }

        CELL nextCell = TryConnectRandomCell(cellToGo);
        if (nextCell != null)
        {
            //print("找到路了，++清單 " + cellList.Count);
            if ( !FinishAtDeepest && nextCell.x == puzzleEnd.x && nextCell.y == puzzleEnd.y)
            {
                gotFinal = true;
                print("連到終點了，改變 gotFinal => " + gotFinal);
                //cellList = List<CELL>.re cellList
            }
            else
            {
                cellList.Add(nextCell);     //無論如何終點不要被加到 cellList，不要再另外連出去
            }
            return true;
        }
        else
        {
            //print("沒路可走，--清單 " + cellList.Count);
            cellList.Remove(cellToGo);
            return false;
        }
    }

    protected void BuildMapImmediate()
    {
        while (cellList.Count > 0)
        {
            DoOneCycle();
        }
    }

    protected IEnumerator BuildMapIterator()
    {
        while (cellList.Count > 0)
        {
            if (DoOneCycle())
            {
                ProcessNormalCells();
                FillAllTiles();
                yield return new WaitForSeconds(0.2f);
            }
        }

        print("好好好，差不多跑完了");
        yield return new WaitForSeconds(0.1f);
    }

    protected override void FillAllTiles()
    {
        if (!isDebug)
        {
            base.FillAllTiles();
            return;
        }

        theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTileGroup.GetTileGroup());
    }
}
