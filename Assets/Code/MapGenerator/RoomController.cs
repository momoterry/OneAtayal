using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform southDoor;
    public Transform northDoor;

    public GameObject battleWall;   //¾Ô°«¾×Àð

    //protected the

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartBattleWall()
    {
        battleWall.SetActive(true);
        //BattleSystem.GetInstance().GetMapGenerator().RebuildNavmesh();
    }

    public void OnStopBattleWall()
    {
        battleWall.SetActive(false);
        //BattleSystem.GetInstance().GetMapGenerator().RebuildNavmesh();
    }
}
