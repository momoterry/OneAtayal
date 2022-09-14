using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorInfo
{
    public Transform pos;
    public GameObject closeObj;
    public GameObject openObj;
}

public enum DoorDir
{
    N, S, W1, W2, E1, E2
}

public class RoomDungeon : MonoBehaviour
{


    public DoorInfo N;
    public DoorInfo S;
    public DoorInfo W1;
    public DoorInfo W2;
    public DoorInfo E1;
    public DoorInfo E2;

    protected DoorInfo GetDoorInfo( DoorDir d)
    {
        DoorInfo theDoor = null;
        switch (d)
        {
            case DoorDir.N:
                theDoor = N;
                break;
            case DoorDir.S:
                theDoor = S;
                break;
            case DoorDir.W1:
                theDoor = W1;
                break;
            case DoorDir.W2:
                theDoor = W2;
                break;
            case DoorDir.E1:
                theDoor = E1;
                break;
            case DoorDir.E2:
                theDoor = E2;
                break;
        }

        return theDoor;
    }

    public Vector3 GetDoorPos( DoorDir doorDir)
    {
        Transform dT = GetDoorInfo(doorDir).pos;
        if (dT == null)
            dT = transform;
        return dT.position;
    }

    public void SetDoorStatus( DoorDir doorDir, bool isOpen)
    {
        DoorInfo theDoor = GetDoorInfo(doorDir);
        if (theDoor != null)
        {
            if (theDoor.openObj)
            {
                theDoor.openObj.SetActive(isOpen);
            }
            if (theDoor.closeObj)
            {
                theDoor.closeObj.SetActive(!isOpen);
            }
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
