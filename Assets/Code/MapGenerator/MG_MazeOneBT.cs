using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用 BackTracing 演算法來製造迷宮，可能可以有更長的路徑

public class MG_MazeOneBT : MG_MazeOneBase
{
    override protected void CreatMazeMap()
    {
        DisjointSetUnion puzzleDSU = new DisjointSetUnion();
        puzzleDSU.Init(puzzleHeight * puzzleWidth);
        //base.PreCalculateGameplayInfo();
        RunTraceAtCell(puzzleMap[puzzleStart.x][puzzleStart.y]);
    }

    protected override void PreCalculateGameplayInfo()
    {

    }


    protected void RunTraceAtCell(cellInfo cell)
    {
        if (cell.x > 0)
            ConnectCells(cell, puzzleMap[cell.x - 1][cell.y], DIRECTION.L);
        if (cell.x < puzzleWidth - 1)
            ConnectCells(cell, puzzleMap[cell.x + 1][cell.y], DIRECTION.R);
        if (cell.y > 0)
            ConnectCells(cell, puzzleMap[cell.x][cell.y - 1], DIRECTION.D);
        if (cell.y < puzzleHeight - 1)
            ConnectCells(cell, puzzleMap[cell.x][cell.y + 1], DIRECTION.U);
    }

}
