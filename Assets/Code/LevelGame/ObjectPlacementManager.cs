using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t�X MazeGameManager �ϥΡA�Τ@��m�U�ж��γq�D�H���i�}�a���󪺨t��
//�S�O�O�w��i���� Doll ���H���\��

public class ObjectPlacementManager : MonoBehaviour
{
    List<MazeGameManagerBase.RoomInfo> roomList = new List<MazeGameManagerBase.RoomInfo>();

    public void AddRooms(List<MazeGameManagerBase.RoomInfo> _list)
    {
        roomList.AddRange(_list);
    }

    public void BuildAll()
    {
        print("ObjectPlacementManager BuildAll �`�@ Room ��: " + roomList.Count);
    }

}
