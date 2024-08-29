using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_HugeRoomMaze : MG_MazeOneBase
{
    [System.Serializable]
    public class BlockInfo
    {
        public enum BLOCK_TYPE
        {
            BIG_ROOM,
            MAZE,
        }
        public BLOCK_TYPE type;
        //public int width;  //�p�G�O�j Room �ɭԡA-1 ��ܥΥ��j�p
        public int blockHeight;
    }
    public BlockInfo[] blocks;


    protected override void PresetMapInfo()
    {
        int totalHeight = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            totalHeight += blocks[i].blockHeight;
        }

        puzzleHeight = totalHeight;
        mazeDir = MAZE_DIR.DONW_TO_TOP;

        base.PresetMapInfo();
    }

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();
        if (mazeDir == MAZE_DIR.DONW_TO_TOP)
        {
            puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
            puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);
            BattleSystem.GetInstance().initPlayerDirAngle = 0;
        }

    }
}
