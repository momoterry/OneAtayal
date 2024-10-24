using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t�X MazeGameManager �ϥΡA�Τ@��m�U�ж��γq�D�H���i�}�a���󪺨t��
//�S�O�O�w��i���� Doll ���H���\��

public class ObjectPlacementManager : MonoBehaviour
{
    public RoomObjectPlacement.ObjectInfo[] fixObjects;

    [System.Serializable]
    public class DollObjectInfo : RoomObjectPlacement.ObjectInfo
    {
        public float powerRatio = 1.0f; // ��ƭ� > 1.0f �ɡA�X�{�����v�|�ե��A�u�Φb forceRandomNum �����X
    }

    public DollObjectInfo[] randomObjects;
    public int randomSelectNum = 3;
    public bool randomObjectInPathOnly = false;
    public float forceRandomNum = 0;     //�p�G�����w > 0 ���ȡA�|�H�q���覡�Ϻ�^�H�����󪺾��v


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

        RoomObjectPlacement noRandomRop = null;
        if (randomObjectInPathOnly)
        {
            GameObject fo = new GameObject("Fix_ROP_Obj");
            fo.transform.SetParent(transform, false);
            noRandomRop = o.AddComponent<RoomObjectPlacement>();
            noRandomRop.objs = fixObjects;
        }

        if (forceRandomNum > 0)
        {
            int allBlockNum = 0;
            foreach (MazeGameManagerBase.RoomInfo ri in roomList)
            {
                if (!(randomObjectInPathOnly && !ri.cell.isPath))
                    allBlockNum += rop.GetRoomBlockNums(ri);
            }
            print("allBlockNum: " + allBlockNum);
            float allRondomPercent = 0;
            for (int i = 0; i < randomCount; i++)
            {
                allRondomPercent += randomObjects[i].placePercent;
            }
            float predictRandomNum = allRondomPercent * allBlockNum * 0.01f;
            print("�w�p�q: " +  predictRandomNum + "  �ݨD�q: " + forceRandomNum);
            float forceFixRatio = forceRandomNum / predictRandomNum;
            for (int i = 0; i < randomCount; i++)
            {
                randomObjects[i].placePercent *= forceFixRatio;
                if (randomObjects[i].powerRatio > 1.0f)
                    randomObjects[i].placePercent /= randomObjects[i].powerRatio;       //power ���j������ݭn���C�X�{�v
                print(randomObjects[i].objRef.name + " �ץ���: " + randomObjects[i].placePercent);
            }
        }

        foreach (MazeGameManagerBase.RoomInfo ri in roomList)
        {
            if (randomObjectInPathOnly && ri.cell.isPath == false)
            {
                print("���O Path�A�����ͥۤƧ��F");
                noRandomRop.Build(ri);
            }
            else
            {
                rop.Build(ri);
            }
        }

    }

}
