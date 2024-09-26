using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t�X MazeGameManager �ϥΡA�Τ@��m�U�ж��γq�D�H���i�}�a���󪺨t��
//�S�O�O�w��i���� Doll ���H���\��

public class ObjectPlacementManager : MonoBehaviour
{
    public RoomObjectPlacement.ObjectInfo[] fixObjects;

    public RoomObjectPlacement.ObjectInfo[] randomObjects;
    public int randomSelectNum = 3;

    protected List<MazeGameManagerBase.RoomInfo> roomList = new List<MazeGameManagerBase.RoomInfo>();

    public void AddRooms(List<MazeGameManagerBase.RoomInfo> _list)
    {
        roomList.AddRange(_list);
    }

    public void BuildAll()
    {
        //print("ObjectPlacementManager BuildAll �`�@ Room ��: " + roomList.Count);

        int randomCount = randomSelectNum > randomObjects.Length ? randomObjects.Length : randomSelectNum;

        RoomObjectPlacement.ObjectInfo[] allObject = new RoomObjectPlacement.ObjectInfo[fixObjects.Length + randomCount];
        for (int i = 0; i < fixObjects.Length; i++)
        {
            allObject[i] = fixObjects[i];
        }

        if (randomCount > 0)
        {
            OneUtility.Shuffle(randomObjects);
            for (int i = 0; i< randomCount; i++)
            {
                allObject[fixObjects.Length + i] = randomObjects[i];
            }
        }

        GameObject o = new GameObject("ROP_Obj");
        o.transform.SetParent(transform, false);
        RoomObjectPlacement rop = o.AddComponent<RoomObjectPlacement>();
        rop.objs = allObject;

        foreach (MazeGameManagerBase.RoomInfo ri in roomList)
        {
            rop.Build(ri);
        }

    }

}
