using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_MazeOneRoomPath : MG_MazeOneEx
{
    public int MaxMainDeep = 6;
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


    override protected void PresetByContinuousBattle()
    {
        ContinuousBattleDataBase cBase = ContinuousBattleManager.GetCurrBattleData();
        if (cBase != null)
        {
            if (cBase is ContinuousMORoomPathData)
            {
                ContinuousMORoomPathData cData = (ContinuousMORoomPathData)cBase;
                puzzleWidth = cData.puzzleWidth;
                puzzleHeight = cData.puzzleHeight;
                print("�ھ� ---RoomPathData-- ��ƭץ��F�g�c�j�p: " + puzzleWidth + " - " + puzzleHeight);
                roomWidth = cData.roomWidth;
                roomHeight = cData.roomHeight;
                pathWidth = cData.pathWidth;
                pathHeight = cData.pathHeight;

                mazeDir = cData.mazeDir;

                MaxMainDeep = cData.MaxMainDeep;
                MaxBranchDeep = cData.MaxBranchDeep;

                //if (cData.gameManagerRef)
                //{
                //    GameObject o = Instantiate(cData.gameManagerRef.gameObject);
                //    gameManager = o.GetComponent<MazeGameManagerBase>();
                //}

                if (gameManager && cData.gameManagerData != null)
                {
                    gameManager.SetupData(cData.gameManagerData);
                }

                //if (cData.initGameplayRef)
                //{
                //    initGampleyRef = cData.initGameplayRef;
                //}

                if (cData.levelID != null)
                {
                    BattleSystem.GetInstance().levelID = cData.levelID;
                }
            }
            else
            {
                One.LOG("ERROR!! ContinuousBattle ���~�A���d��Ƥ��O ContinuousMazeData !!");
            }
        }
    }

    protected override void CalculateRoomPath()
    {
        CheckCellDeep(puzzleStart.x, puzzleStart.y, DIRECTION.NONE, 0);

        puzzleMap[puzzleStart.x][puzzleStart.y].value = CELL.TERNIMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.TERNIMAL;

        int _maxMainDeep = Mathf.Max(puzzleMap[puzzleEnd.x][puzzleEnd.y].deep, 1);
        CheckMainPathDeep(puzzleEnd.x, puzzleEnd.y, DIRECTION.NONE, true, _maxMainDeep);

        puzzleMap[puzzleStart.x][puzzleStart.y].isPath = true;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].isPath = true;

        if (_maxMainDeep < MaxMainDeep)
        {
            print("�Y�u Main: " + MaxMainDeep + " >> " + _maxMainDeep);
            MaxMainDeep = _maxMainDeep;
        }

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
                        if (cell.mainDeep > MaxMainDeep)
                        {
                            cell.value = CELL.INVALID;
                            if (cell.mainDeep == (MaxMainDeep + 1))
                            {
                                DisConnectCellByDir(cell, cell.from);
                            }
                        }
                        else if (cell.mainDeep == MaxMainDeep)
                        {
                            cell.isPath = true;
                            if (MaxMainDeep < _maxMainDeep)
                            {
                                Vector2Int newPuzzleEnd = new Vector2Int(cell.x, cell.y);
                                print("�󴫲��I: " + puzzleEnd + " >> " + newPuzzleEnd);
                                puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.INVALID;
                                puzzleEnd = newPuzzleEnd;
                            }
                        }
                        else
                            cell.isPath = cell.mainDeep % 2 == 0 ? true : false;
                        //if (cell.mainDeep == MaxMainDeep - 1)
                        //{
                        //    cell.isPath = false;    //�����e�̫�@�楲�w�� Room
                        //    //TODO: �j���_�}�䥦�s��
                        //}
                    }
                    else
                    {
                        int branchDeep = cell.deep - cell.mainDeep;
                        if (cell.mainDeep >= (MaxMainDeep - 1))
                        {
                            cell.value = CELL.INVALID;
                            if (branchDeep == 1)    //�_�}����
                            {
                                DisConnectCellByDir(cell, cell.from);
                            }
                        }
                        else if (branchDeep > MaxBranchDeep)
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
