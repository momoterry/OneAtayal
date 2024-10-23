using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDungueonDataContainterBase : MonoBehaviour
{
    public virtual CDungeonDataBase[] GetDungeons() { return null;}
}

public class CDungueonHugeRoomContainter : CDungueonDataContainterBase
{
    [System.Serializable]
    public class HugeRoomDungeon
    {
        public string ID;
        public string name;     //�p�G�����w�W�١A�|�۰ʽվ� Stage �����W�� (�|��ܦb HUD ����)
        public ContinuousHugeRoomMazeData[] mazeLevelDatas;

        public CDungeonDataBase ToDungeonData()
        {
            CDungeonDataBase data = new CDungeonDataBase();
            data.ID = ID;
            data.name = name;
            data.battles = mazeLevelDatas;
            if (name != null && name != "")
            {
                for (int i = 0; i < data.battles.Length; i++)
                {
                    data.battles[i].name = name + " " + (i + 1) + "/" + data.battles.Length;
                }
            }
            return data;
        }
    }

    public HugeRoomDungeon[] dungeons;

    public override CDungeonDataBase[] GetDungeons()
    {
        CDungeonDataBase[] datas = new CDungeonDataBase[dungeons.Length];
        for (int i = 0;i< datas.Length; i++)
        {
            datas[i] = dungeons[i].ToDungeonData();
        }
        return datas;
    }
}
