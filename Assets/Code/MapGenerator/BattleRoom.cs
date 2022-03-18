using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BattleRoom
// Room �԰� Gameplay ����, ���� Trigger ��A�n�D�W�h (RoomController) �}��/�����԰���
//

public class BattleRoom : MonoBehaviour
{
    protected RoomController theRoomController = null; //�q�`�b Parent �W

    protected bool wallStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent)
        {
            theRoomController = transform.parent.GetComponent<RoomController>();
            //print("BattleRoom : theRoomController = " + theRoomController);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTG(GameObject whoTG)
    {
        if (theRoomController == null)
        {
            //print("ERROR!! BattleRoom did not get room controller!!");
            return;
        }

        if (!wallStart)
        {
            theRoomController.OnStartBattleWall();
            //print("StartBattleWall");
        }
        else
        {
            theRoomController.OnStopBattleWall();
            //print("StopBattleWall");
        }
        wallStart = !wallStart;
    }
}
