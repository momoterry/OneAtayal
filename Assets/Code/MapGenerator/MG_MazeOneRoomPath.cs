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

                    if (cell.isMain)
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

                        cell.isPath = cell.mainDeep % 2 == 0 ? true : false;
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
                        else if (cell.mainDeep == 0)
                        {
                            cell.value = CELL.INVALID;
                            print("�L�Ī�  Cell");
                        }
                        else
                        {
                            cell.isPath = cell.deep % 2 == 0 ? true : false;
                        }
                    }
                }
            }
        }

    }

    //===========================================================================================
    //�w�� RoomPath �� BackTrace �t��k
    //===========================================================================================
    //protected List<CELL> cellList = new List<CELL>();
    //protected int startDSU = 0;
    //protected bool gotFinal = false;

    public bool isRoomOnlyUp = false;
    public float roomOnlyUpBrachPercent = 50.0f;
    protected override void CreatMazeMap()
    {
        if (isRoomOnlyUp)
            CreateMazeByRoomPath();    //�@�ӨϥΦۤv�� BackTrace
        else
            base.CreatMazeMap();
    }

    protected void CreateMazeByRoomPath()
    {
        puzzleDSU.Init(puzzleHeight * puzzleWidth);

        startDSU = puzzleDSU.Find(GetCellID(puzzleStart.x, puzzleStart.y));
        cellList.Add(puzzleMap[puzzleStart.x][puzzleStart.y]);

        int mainDepth = 0;
        //int branchDepth = 0;

        while (cellList.Count > 0)
        {
            CELL cellToGo;
            if (gotFinal)
            {
                cellToGo = cellList[Random.Range(0, cellList.Count)];
            }
            else
            {
                cellToGo = cellList[cellList.Count - 1];
            }

            bool isRoomConnect = false;
            bool isForceStop = false;
            if (!gotFinal)
            {
                isRoomConnect = mainDepth % 2 == 0;
            }
            else
            {
                if (Random.Range(0, 100.0f) > roomOnlyUpBrachPercent)
                {
                    print("�o�������F: " + cellToGo.x + "-" + cellToGo.y);
                    isForceStop = true;
                }
            }
            CELL nextCell = isForceStop ? null : TryConnectRandomPath(cellToGo, isRoomConnect);
            if (nextCell != null)
            {
                //print("�����F�A++�M�� " + cellList.Count);
                if (!gotFinal)
                {
                    print("�{�b�[���O�D�u: " + nextCell.x + "-" + nextCell.y);
                    mainDepth++;
                    if (mainDepth == MaxMainDeep)
                    {
                        print("�D�u����̲`�F !!");
                        gotFinal = true;
                        cellList.Remove(cellToGo);  //�˼Ƴ̫�@�����n�A�s��u
                        if (puzzleEnd.x != nextCell.x || puzzleEnd.y != nextCell.y)
                        {
                            print("�󴫲��I: " + puzzleEnd + " >> " + nextCell.x + " - " + nextCell.y);
                            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.INVALID;
                            puzzleEnd.x = nextCell.x;
                            puzzleEnd.y = nextCell.y;
                        }
                    }
                    else 
                        cellList.Add(nextCell);
                }
                else
                {
                    //if (Random.Range(0, 100.0f) < roomOnlyUpBrachPercent)
                    {
                        print("�{�b�[���O��u: " + nextCell.x + "-" + nextCell.y);
                        cellList.Add(nextCell);
                    }
                    //else
                    //{
                    //    print("�o�������F: " + cellToGo.x + "-" + cellToGo.y);
                    //    cellList.Remove(cellToGo);
                    //}
                }
            }
            else
            {
                cellList.Remove(cellToGo);
            }
        }
    }

    protected CELL TryConnectRandomPath(CELL cell, bool onlyUp = false)
    {
        if (onlyUp)
            print("�u���W��: " + cell.x + " - " + cell.y);
        List<DIRECTION> choices = new List<DIRECTION>();
        if (!cell.L && cell.x > 0 && puzzleMap[cell.x - 1][cell.y].value != CELL.INVALID && !onlyUp)
        {
            if (puzzleDSU.Find(GetCellID(cell.x - 1, cell.y)) != startDSU)
                choices.Add(DIRECTION.L);

        }
        if (!cell.D && cell.y > 0 && puzzleMap[cell.x][cell.y - 1].value != CELL.INVALID && !onlyUp)
        {
            if (puzzleDSU.Find(GetCellID(cell.x, cell.y - 1)) != startDSU)
                choices.Add(DIRECTION.D);

        }
        if (!cell.R && cell.x < puzzleWidth - 1 && puzzleMap[cell.x + 1][cell.y].value != CELL.INVALID && !onlyUp)
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

    //protected bool DoOneRoomPathCycle()
    //{
        //CELL cellToGo;
        //if (gotFinal)
        //{
        //    cellToGo = cellList[Random.Range(0, cellList.Count)];
        //}
        //else
        //{
        //    cellToGo = cellList[cellList.Count - 1];
        //}

        //CELL nextCell = TryConnectRandomCell(cellToGo);
        //if (nextCell != null)
        //{
        //    //print("�����F�A++�M�� " + cellList.Count);
        //    if (!FinishAtDeepest && nextCell.x == puzzleEnd.x && nextCell.y == puzzleEnd.y)
        //    {
        //        gotFinal = true;
        //    }
        //    else
        //    {
        //        cellList.Add(nextCell);     //�L�צp����I���n�Q�[�� cellList�A���n�A�t�~�s�X�h
        //    }
        //    return true;
        //}
        //else
        //{
        //    cellList.Remove(cellToGo);
        //    return false;
        //}
    //}
}
