using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �ΨӴy�z�@�� MazeOneBase �g�c�Ѽƪ���Ƶ��c
// �ثe�D�n�ΨӤ䴩 CSV ���ɮ״y�z
// �H��i�H�� CMazeJsonData �]���Y�ص{�פW����X




//�@�� MazeOne Dungeion ���y�z
public class MODungeonData
{
    public string DungeonID;
    public List<MODungeonStageData> stageList = new List<MODungeonStageData>();      //�򥻤W���Ӫ��W�����ǱƦC
}

//MazeOne Dungeion ���C�@�u�h�v���y�z

public class MODungeonStageData
{
    public string DungeonID;
    public int Level;           //�ĴX�h
    public string SceneName;
    public int PuzzleWidth;
    public int PuzzleHeight;
    public float PathRate;
}

