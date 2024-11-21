using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_MazeOneRoomPath : MG_MazeOneEx
{
    public int MaxBranchDeep = 2;


    protected void DisConnectCellByDir(CELL cell, DIRECTION toDir)
    {
        CELL cTo;

        switch (toDir)
        {
            case DIRECTION.U:
                cTo = puzzleMap[cell.x][cell.y+1];
                cell.U = cTo.D = false;
                break;
            case DIRECTION.D:
                cTo = puzzleMap[cell.x][cell.y-1];
                cell.D = cTo.U = false;
                break;
            case DIRECTION.L:
                cTo = puzzleMap[cell.x-1][cell.y];
                cell.L = cTo.R = false;
                break;
            case DIRECTION.R:
                cTo = puzzleMap[cell.x+1][cell.y];
                cell.R = cTo.L = false;
                break;
        }
    }

    protected override void CalculateRoomPath()
    {
        CheckCellDeep(puzzleStart.x, puzzleStart.y, DIRECTION.NONE, 0);

        puzzleMap[puzzleStart.x][puzzleStart.y].value = CELL.TERNIMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.TERNIMAL;

        int maxMainDeep = Mathf.Max(puzzleMap[puzzleEnd.x][puzzleEnd.y].deep, 1);
        CheckMainPathDeep(puzzleEnd.x, puzzleEnd.y, DIRECTION.NONE, true, maxMainDeep);

        puzzleMap[puzzleStart.x][puzzleStart.y].isPath = true;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].isPath = true;

        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                CELL cell = puzzleMap[x][y];
                if (cell.value == CELL.NORMAL)
                {
                    
                    if ( cell.isMain)
                    {
                        //print("Cell is Main? " + cell.isMain + "Maind Deep: " + cell.mainDeep);
                        cell.isPath = cell.mainDeep % 2 == 0 ? true : false;
                        if (cell.mainDeep == maxMainDeep - 1)
                        {
                            cell.isPath = false;    //結束前最後一格必定為 Room
                        }
                    }
                    else
                    {
                        int branchDeep = cell.deep - cell.mainDeep;
                        if (branchDeep > MaxBranchDeep)
                        {
                            cell.value = CELL.INVALID;
                            if (branchDeep == (MaxBranchDeep + 1))
                            {
                                DisConnectCellByDir(cell, cell.from);
                            }
                        }
                        else
                        {
                            cell.isPath = cell.deep % 2 == 0 ? true : false;
                        }
                    }
                }
            }
        }



        //base.CalculateRoomPath();
    }

}
