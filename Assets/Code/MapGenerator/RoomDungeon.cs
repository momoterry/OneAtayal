using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct doorInfo
{
    public Transform pos;
    public GameObject closeObj;
    public GameObject openObj;
}

public class RoomDungeon : MonoBehaviour
{
    public enum DoorDir
    {
        N, S, W1, W2, E1, E2
    }

    public doorInfo N;
    public doorInfo S;
    public doorInfo W1;
    public doorInfo W2;
    public doorInfo E1;
    public doorInfo E2;

    public void SetDoorStatus( DoorDir doorDir, bool isOpen)
    {

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
